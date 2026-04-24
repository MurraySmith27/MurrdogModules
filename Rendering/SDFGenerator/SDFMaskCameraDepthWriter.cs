using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


public delegate void SDFDepthTextureChangedEvent(Texture depthTexture);

public delegate void SDFDepthTextureChangedPartialEvent(Texture depthTexture, Vector2Int regionCenter, int regionSize);

public class SDFMaskCameraDepthWriter : MonoBehaviour
{
    public SDFDepthTextureChangedEvent m_onSDFDepthTextureChanged;
    public SDFDepthTextureChangedPartialEvent m_onSDFDepthTextureChangedPartial;

    [SerializeField] private RenderTexture m_destinationDepthRenderTexture;

    [FormerlySerializedAs("_timeBetweenDepthUpdatesSeconds")] [SerializeField]
    private Vector3 _timeBetweenDepthUpdatesPerGraphicsSettings = Vector3.zero;

    [SerializeField] private RenderTexture m_dummyTempRenderTexture;

    [SerializeField] private bool _updateOverTime = false;

    // World-space extent covered by the full texture (width and height in world units).
    // Divide a world-space XZ position by this to get a [0,1] UV into the texture.
    [SerializeField] private float _worldToTextureScalingFactor = 100f;

    // Side length (in SDF texture pixels) of the square patch updated on a partial render.
    [SerializeField] private int _patchSizePixels = 64;

    [SerializeField] private int rendererIndex = 3;

    // ── Tile cameras ─────────────────────────────────────────────────────────

    private const int GridDim = 3; // 3×3 grid → 9 cameras

    private Camera _mainCamera; // original camera, disabled; used only to copy frustum settings
    private Camera[,] _tileCameras; // [i, j] where i=column (right), j=row (up)
    private RenderTexture[,] _tileRTs;

    // ── State ─────────────────────────────────────────────────────────────────

    private float _timeSinceLastDepthUpdate;
    private float _timeBetweenDepthUpdates;

    // Partial-update batch state: all affected tile cameras share the same computed center/size.
    // The partial event fires once after all cameras in the batch have post-rendered.
    private bool[] _cameraInPartialBatch; // which cameras belong to the current partial batch
    private Vector2Int _partialBatchCenter;
    private int _partialBatchSize;
    private int _pendingPartialCount; // cameras still to post-render before firing the partial event
    private int _pendingFullCount; // remaining tile cameras to post-render for a full update

    // Wrap-batch state: cameras for the seam-wrapped counterpart of a partial update.
    // Fires a second partial event so the SDF generator processes each side of the seam separately.
    private bool[] _cameraInWrapBatch;
    private Vector2Int _wrapBatchCenter;
    private int _wrapBatchSize;
    private int _pendingWrapCount;

    [SerializeField] private int updateFrameDelay = 0;

    // ─────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        _mainCamera = GetComponent<Camera>();
        _mainCamera.enabled = false; // tile cameras take over all rendering

        SetTimeBetweenDepthUpdate(GlobalSettings.GraphicsQuality);
        GlobalSettings.OnGraphicsQualitySettingsChanged -= SetTimeBetweenDepthUpdate;
        GlobalSettings.OnGraphicsQualitySettingsChanged += SetTimeBetweenDepthUpdate;

        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;

        FogOfWarSystem.Instance.OnRevealedTilesUpdated -= OnFogOfWarUpdated;
        FogOfWarSystem.Instance.OnRevealedTilesUpdated += OnFogOfWarUpdated;

        CreateTileCameras();
    }

    private void OnDestroy()
    {
        GlobalSettings.OnGraphicsQualitySettingsChanged -= SetTimeBetweenDepthUpdate;

        if (MapSystem.IsAvailable)
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;

        if (FogOfWarSystem.IsAvailable)
            FogOfWarSystem.Instance.OnRevealedTilesUpdated -= OnFogOfWarUpdated;

        if (_tileRTs != null)
            foreach (var rt in _tileRTs)
                if (rt != null)
                    rt.Release();
    }

    // ── Tile camera setup ─────────────────────────────────────────────────────

    private void CreateTileCameras()
    {
        int fullW = m_destinationDepthRenderTexture.width;
        int fullH = m_destinationDepthRenderTexture.height;
        int tileW = fullW / GridDim;
        int tileH = fullH / GridDim;

        _tileCameras = new Camera[GridDim, GridDim];
        _tileRTs = new RenderTexture[GridDim, GridDim];
        _cameraInPartialBatch = new bool[GridDim * GridDim];
        _cameraInWrapBatch = new bool[GridDim * GridDim];

        float tileOrthoSize = _mainCamera.orthographicSize / GridDim;
        float tileSizeRight = _mainCamera.orthographicSize * _mainCamera.aspect * 2f / GridDim;
        float tileSizeUp = _mainCamera.orthographicSize * 2f / GridDim;

        var tileRTDesc = m_destinationDepthRenderTexture.descriptor;
        tileRTDesc.width = tileW;
        tileRTDesc.height = tileH;
        
        for (int j = 0; j < GridDim; j++)
        {
            for (int i = 0; i < GridDim; i++)
            {
                Vector3 rightOffset = _mainCamera.transform.right * (i - (GridDim - 1) * 0.5f) * tileSizeRight;
                Vector3 upOffset = _mainCamera.transform.up * (j - (GridDim - 1) * 0.5f) * tileSizeUp;

                var go = new GameObject($"TileCamera_{i}_{j}");
                go.transform.SetParent(transform, worldPositionStays: false);
                go.transform.position = _mainCamera.transform.position + rightOffset + upOffset;
                go.transform.rotation = _mainCamera.transform.rotation;

                var cam = go.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = tileOrthoSize;
                cam.cullingMask = _mainCamera.cullingMask;
                cam.clearFlags = _mainCamera.clearFlags;
                cam.backgroundColor = _mainCamera.backgroundColor;
                cam.nearClipPlane = _mainCamera.nearClipPlane;
                cam.farClipPlane = _mainCamera.farClipPlane;
                cam.depth = _mainCamera.depth;
                cam.allowHDR = _mainCamera.allowHDR;
                cam.allowMSAA = _mainCamera.allowMSAA;
                cam.useOcclusionCulling = _mainCamera.useOcclusionCulling;

                var additionalData = cam.GetUniversalAdditionalCameraData();
                additionalData.SetRenderer(rendererIndex);
                
                var rt = new RenderTexture(tileRTDesc);
                rt.name = $"TileRT_{i}_{j}";
                rt.Create();

                cam.targetTexture = rt;
                cam.enabled = false;

                var injectionLayer = go.AddComponent<CameraRenderStepsInjectionLayer>();
                injectionLayer.Init(this);

                _tileCameras[i, j] = cam;
                _tileRTs[i, j] = rt;
            }
        }
    }

    // ── Camera lookup helpers ─────────────────────────────────────────────────

    public bool IsTileCamera(Camera cam)
    {
        if (_tileCameras == null) return false;
        foreach (var c in _tileCameras)
            if (c == cam)
                return true;
        return false;
    }

    private bool TryGetTileIndex(Camera cam, out int tileI, out int tileJ)
    {
        if (_tileCameras != null)
        {
            for (int j = 0; j < GridDim; j++)
            for (int i = 0; i < GridDim; i++)
            {
                if (_tileCameras[i, j] == cam)
                {
                    tileI = i;
                    tileJ = j;
                    return true;
                }
            }
        }

        tileI = tileJ = -1;
        return false;
    }

    private void WorldPosToUV(Vector3 worldPos, out float u, out float v)
    {
        Vector3 delta = worldPos - _mainCamera.transform.position;
        float rightProj = Vector3.Dot(delta, _mainCamera.transform.right);
        float upProj = Vector3.Dot(delta, _mainCamera.transform.up);
        float halfRight = _mainCamera.orthographicSize * _mainCamera.aspect;
        float halfUp = _mainCamera.orthographicSize;
        u = (rightProj + halfRight) / (halfRight * 2f);
        v = (upProj + halfUp) / (halfUp * 2f);
    }

    private void WorldPosToTileIndex(Vector3 worldPos, out int tileI, out int tileJ)
    {
        WorldPosToUV(worldPos, out float u, out float v);
        tileI = Mathf.Clamp(Mathf.FloorToInt(u * GridDim), 0, GridDim - 1);
        tileJ = Mathf.Clamp(Mathf.FloorToInt(v * GridDim), 0, GridDim - 1);
    }

    // ── Trigger methods ───────────────────────────────────────────────────────

    private void SetTimeBetweenDepthUpdate(GlobalSettings.GraphicsQualitySettings setting)
    {
        switch (setting)
        {
            case GlobalSettings.GraphicsQualitySettings.Low:
                _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.x;
                break;
            case GlobalSettings.GraphicsQualitySettings.Medium:
                _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.y;
                break;
            case GlobalSettings.GraphicsQualitySettings.High:
                _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.z;
                break;
        }
    }

    private void Update()
    {
        if (!_updateOverTime) return;

        _timeSinceLastDepthUpdate += Time.deltaTime;
        if (_timeBetweenDepthUpdates == 0f || _timeSinceLastDepthUpdate > _timeBetweenDepthUpdates)
        {
            _timeSinceLastDepthUpdate = 0f;
            RenderSDFTextureThisFrame();
        }
    }

    private void OnMapChunkGenerated(int row, int col, int width, int height, int layer)
    {
        RenderSDFTextureThisFrame(new Vector2Int(col, row));
    }

    private void OnFogOfWarUpdated(List<Vector2Int> updatedPositions, Guid player)
    {
        if (player != PlayerSystem.Instance.GetActivePlayerId() || updatedPositions.Count == 0)
            return;
        if (updateFrameDelay > 0)
        {
            Timing.RunCoroutineSingleton(RunSDFUpdateAfterDelay(updateFrameDelay, updatedPositions), gameObject,
                SingletonBehavior.Overwrite);
        }
        else
        {
            DoPartialSDFUpdate(updatedPositions);
        }
    }

    public void RenderSDFTextureThisFrame(Vector2Int gridPosition)
    {
        if (updateFrameDelay > 0)
        {
            Timing.RunCoroutineSingleton(RunSDFUpdateAfterDelay(updateFrameDelay, true, gridPosition), gameObject,
                SingletonBehavior.Overwrite);
        }
        else
        {
            DoPartialSDFUpdate(gridPosition);
        }
    }
    
    

    public void RenderSDFTextureThisFrame()
    {
        if (updateFrameDelay > 0)
        {
            Timing.RunCoroutineSingleton(RunSDFUpdateAfterDelay(updateFrameDelay, false).CancelWith(gameObject),
                gameObject,
                SingletonBehavior.Overwrite);
        }
        else
        {
            DoFullSDFUpdate();
        }
    }
    private IEnumerator<float> RunSDFUpdateAfterDelay(int frameDelay, List<Vector2Int> gridPositions)
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return Timing.WaitForOneFrame;
        }

        DoPartialSDFUpdate(gridPositions);
    }
    
    private IEnumerator<float> RunSDFUpdateAfterDelay(int frameDelay, bool isPartial, Vector2Int gridPosition = default)
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return Timing.WaitForOneFrame;
        }

        if (isPartial)
        {
            DoPartialSDFUpdate(gridPosition);
        }
        else
        {
            DoFullSDFUpdate();
        }
    }

    private void DoFullSDFUpdate()
    {
        _pendingFullCount = GridDim * GridDim;
        // Clear any pending partial/wrap state so full-update cameras don't double-fire.
        for (int k = 0; k < _cameraInPartialBatch.Length; k++)
            _cameraInPartialBatch[k] = false;
        for (int k = 0; k < _cameraInWrapBatch.Length; k++)
            _cameraInWrapBatch[k] = false;
        _pendingPartialCount = 0;
        _pendingWrapCount = 0;

        foreach (var cam in _tileCameras)
            cam.enabled = true;
    }
    
    public void DoPartialSDFUpdate(List<Vector2Int> updatedPositions)
    {
        // Expand with in-frustum wrapped counterparts (map rotates around X axis).
        int mapSize = GameConstants.STARTING_MAP_SIZE.x;
        var allPositions = new List<Vector2Int>(updatedPositions);
        foreach (var gridPos in updatedPositions)
        {
            foreach (int offset in new[] { mapSize, -mapSize })
            {
                var wrapped = MapUtils.GetPosWrapAroundBufferedTilePosition(new Vector2Int(gridPos.x, gridPos.y + offset));
                Vector3 wrappedWorld = MapUtils.GetTileWorldPositionFromGridPosition(wrapped, 0);
                WorldPosToUV(wrappedWorld, out float wu, out float wv);
                if (wu >= 0f && wu <= 1f && wv >= 0f && wv <= 1f)
                    allPositions.Add(wrapped);
            }
        }

        // Compute bounding box in pixel space across all positions.
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;
        foreach (var gridPos in allPositions)
        {
            Vector3 worldPos = MapUtils.GetTileWorldPositionFromGridPosition(gridPos, 0);
            WorldPosToUV(worldPos, out float u, out float v);
            int px = Mathf.RoundToInt(u * m_destinationDepthRenderTexture.width);
            int py = Mathf.RoundToInt(v * m_destinationDepthRenderTexture.height);
            if (px < minX) minX = px;
            if (px > maxX) maxX = px;
            if (py < minY) minY = py;
            if (py > maxY) maxY = py;
        }

        // Reset batch state and store the computed center/size.
        for (int k = 0; k < _cameraInPartialBatch.Length; k++)
            _cameraInPartialBatch[k] = false;
        for (int k = 0; k < _cameraInWrapBatch.Length; k++)
            _cameraInWrapBatch[k] = false;
        _pendingPartialCount = 0;
        _pendingWrapCount = 0;
        _partialBatchCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);
        _partialBatchSize = Mathf.Max(maxX - minX + 1, maxY - minY + 1, _patchSizePixels);

        // Enable every tile camera that covers at least one position.
        foreach (var gridPos in allPositions)
        {
            Vector3 worldPos = MapUtils.GetTileWorldPositionFromGridPosition(gridPos, 0);
            WorldPosToTileIndex(worldPos, out int tileI, out int tileJ);
            int idx = tileJ * GridDim + tileI;
            if (!_cameraInPartialBatch[idx])
            {
                _cameraInPartialBatch[idx] = true;
                _pendingPartialCount++;
                _tileCameras[tileI, tileJ].enabled = true;
            }
        }
    }

    private void DoPartialSDFUpdate(Vector2Int gridPosition)
    {
        Vector3 worldPos = MapUtils.GetTileWorldPositionFromGridPosition(gridPosition, 0);
        WorldPosToUV(worldPos, out float u, out float v);
        int pixelX = Mathf.RoundToInt(u * m_destinationDepthRenderTexture.width);
        int pixelY = Mathf.RoundToInt(v * m_destinationDepthRenderTexture.height);

        for (int k = 0; k < _cameraInPartialBatch.Length; k++)
            _cameraInPartialBatch[k] = false;
        for (int k = 0; k < _cameraInWrapBatch.Length; k++)
            _cameraInWrapBatch[k] = false;
        _pendingPartialCount = 0;
        _pendingWrapCount = 0;

        _partialBatchCenter = new Vector2Int(pixelX, pixelY);
        _partialBatchSize = _patchSizePixels;

        WorldPosToTileIndex(worldPos, out int tileI, out int tileJ);
        int camIdx = tileJ * GridDim + tileI;
        _cameraInPartialBatch[camIdx] = true;
        _pendingPartialCount = 1;
        _tileCameras[tileI, tileJ].enabled = true;

        // Check wrapped counterpart (map rotates around X axis: grid.y wraps by STARTING_MAP_SIZE.x).
        int mapSize = GameConstants.STARTING_MAP_SIZE.x;
        foreach (int offset in new[] { mapSize, -mapSize })
        {
            Vector3 wrappedWorld = MapUtils.GetTileWorldPositionFromGridPosition(
                new Vector2Int(gridPosition.x, gridPosition.y + offset), 0);
            WorldPosToUV(wrappedWorld, out float wu, out float wv);
            if (wu < 0f || wu > 1f || wv < 0f || wv > 1f) continue;

            int wpx = Mathf.RoundToInt(wu * m_destinationDepthRenderTexture.width);
            int wpy = Mathf.RoundToInt(wv * m_destinationDepthRenderTexture.height);
            WorldPosToTileIndex(wrappedWorld, out int wI, out int wJ);
            int wIdx = wJ * GridDim + wI;

            if (_cameraInPartialBatch[wIdx] || _cameraInWrapBatch[wIdx]) continue;
            _cameraInWrapBatch[wIdx] = true;
            _pendingWrapCount++;
            _wrapBatchCenter = new Vector2Int(wpx, wpy);
            _wrapBatchSize = _patchSizePixels;
            _tileCameras[wI, wJ].enabled = true;
        }
    }

    // ── Post-render callback ──────────────────────────────────────────────────

    [Obsolete]
    public void PreRender(ScriptableRenderContext context, Camera camera)
    {
    }

    public void PostRender(ScriptableRenderContext context, Camera camera)
    {
        camera.enabled = false;

        if (!TryGetTileIndex(camera, out int tileI, out int tileJ))
            return;

        int fullW = m_destinationDepthRenderTexture.width;
        int fullH = m_destinationDepthRenderTexture.height;
        int tileW = fullW / GridDim;
        int tileH = fullH / GridDim;
        int offsetX = tileI * tileW;
        int offsetY = tileJ * tileH;

        // Copy tile render directly into the matching region of the combined RT (pure GPU copy).
        Graphics.CopyTexture(
            _tileRTs[tileI, tileJ], 0, 0, 0, 0, tileW, tileH,
            m_destinationDepthRenderTexture, 0, 0, offsetX, offsetY);

        int cameraIndex = tileJ * GridDim + tileI;

        if (_cameraInPartialBatch[cameraIndex])
        {
            _cameraInPartialBatch[cameraIndex] = false;
            _pendingPartialCount--;
            if (_pendingPartialCount <= 0)
            {
                if (m_onSDFDepthTextureChangedPartial != null &&
                    m_onSDFDepthTextureChangedPartial.GetInvocationList().Length > 0)
                    m_onSDFDepthTextureChangedPartial(m_destinationDepthRenderTexture, _partialBatchCenter,
                        _partialBatchSize);
            }
        }
        else if (_cameraInWrapBatch[cameraIndex])
        {
            _cameraInWrapBatch[cameraIndex] = false;
            _pendingWrapCount--;
            if (_pendingWrapCount <= 0)
            {
                if (m_onSDFDepthTextureChangedPartial != null &&
                    m_onSDFDepthTextureChangedPartial.GetInvocationList().Length > 0)
                    m_onSDFDepthTextureChangedPartial(m_destinationDepthRenderTexture, _wrapBatchCenter,
                        _wrapBatchSize);
            }
        }
        else if (_pendingFullCount > 0)
        {
            _pendingFullCount--;
            if (_pendingFullCount <= 0)
            {
                if (m_onSDFDepthTextureChanged != null && m_onSDFDepthTextureChanged.GetInvocationList().Length > 0)
                    m_onSDFDepthTextureChanged(m_destinationDepthRenderTexture);
            }
        }
    }
}