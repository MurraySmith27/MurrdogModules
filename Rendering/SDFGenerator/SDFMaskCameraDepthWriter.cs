using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

    // ── Tile cameras ─────────────────────────────────────────────────────────

    private const int GridDim = 3; // 3×3 grid → 9 cameras

    private Camera _mainCamera;      // original camera, disabled; used only to copy frustum settings
    private Camera[,] _tileCameras;  // [i, j] where i=column (right), j=row (up)
    private RenderTexture[,] _tileRTs;

    // ── State ─────────────────────────────────────────────────────────────────

    private float _timeSinceLastDepthUpdate;
    private float _timeBetweenDepthUpdates;

    // Per-camera partial-update state: if a camera's entry is set, it fires the partial event on PostRender.
    private Vector2Int?[] _pendingPartialCenter; // null = not pending partial; non-null = fire partial event
    private int _pendingFullCount;               // remaining tile cameras to post-render for a full update

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
                if (rt != null) rt.Release();
    }

    // ── Tile camera setup ─────────────────────────────────────────────────────

    private void CreateTileCameras()
    {
        int fullW = m_destinationDepthRenderTexture.width;
        int fullH = m_destinationDepthRenderTexture.height;
        int tileW = fullW / GridDim;
        int tileH = fullH / GridDim;

        _tileCameras         = new Camera[GridDim, GridDim];
        _tileRTs             = new RenderTexture[GridDim, GridDim];
        _pendingPartialCenter = new Vector2Int?[GridDim * GridDim];

        float tileOrthoSize  = _mainCamera.orthographicSize / GridDim;
        float tileSizeRight  = _mainCamera.orthographicSize * _mainCamera.aspect * 2f / GridDim;
        float tileSizeUp     = _mainCamera.orthographicSize * 2f / GridDim;

        var tileRTDesc = m_destinationDepthRenderTexture.descriptor;
        tileRTDesc.width  = tileW;
        tileRTDesc.height = tileH;

        for (int j = 0; j < GridDim; j++)
        {
            for (int i = 0; i < GridDim; i++)
            {
                Vector3 rightOffset = _mainCamera.transform.right * (i - (GridDim - 1) * 0.5f) * tileSizeRight;
                Vector3 upOffset    = _mainCamera.transform.up   * (j - (GridDim - 1) * 0.5f) * tileSizeUp;

                var go = new GameObject($"TileCamera_{i}_{j}");
                go.transform.SetParent(transform, worldPositionStays: false);
                go.transform.position = _mainCamera.transform.position + rightOffset + upOffset;
                go.transform.rotation = _mainCamera.transform.rotation;

                var cam = go.AddComponent<Camera>();
                cam.orthographic     = true;
                cam.orthographicSize = tileOrthoSize;
                cam.cullingMask      = _mainCamera.cullingMask;
                cam.clearFlags       = _mainCamera.clearFlags;
                cam.backgroundColor  = _mainCamera.backgroundColor;
                cam.nearClipPlane    = _mainCamera.nearClipPlane;
                cam.farClipPlane     = _mainCamera.farClipPlane;
                cam.depth            = _mainCamera.depth;
                cam.allowHDR         = _mainCamera.allowHDR;
                cam.allowMSAA        = _mainCamera.allowMSAA;

                var rt = new RenderTexture(tileRTDesc);
                rt.name = $"TileRT_{i}_{j}";
                rt.Create();

                cam.targetTexture = rt;
                cam.enabled = false;

                var injectionLayer = go.AddComponent<CameraRenderStepsInjectionLayer>();
                injectionLayer.Init(this);

                _tileCameras[i, j] = cam;
                _tileRTs[i, j]     = rt;
            }
        }
    }

    // ── Camera lookup helpers ─────────────────────────────────────────────────

    public bool IsTileCamera(Camera cam)
    {
        if (_tileCameras == null) return false;
        foreach (var c in _tileCameras)
            if (c == cam) return true;
        return false;
    }

    private bool TryGetTileIndex(Camera cam, out int tileI, out int tileJ)
    {
        if (_tileCameras != null)
        {
            for (int j = 0; j < GridDim; j++)
            for (int i = 0; i < GridDim; i++)
            {
                if (_tileCameras[i, j] == cam) { tileI = i; tileJ = j; return true; }
            }
        }
        tileI = tileJ = -1;
        return false;
    }

    private void WorldPosToTileIndex(Vector3 worldPos, out int tileI, out int tileJ)
    {
        Vector3 delta = worldPos - _mainCamera.transform.position;
        float rightProj = Vector3.Dot(delta, _mainCamera.transform.right);
        float upProj    = Vector3.Dot(delta, _mainCamera.transform.up);

        float halfRight = _mainCamera.orthographicSize * _mainCamera.aspect;
        float halfUp    = _mainCamera.orthographicSize;

        float u = (rightProj + halfRight) / (halfRight * 2f);
        float v = (upProj    + halfUp)    / (halfUp    * 2f);

        tileI = Mathf.Clamp(Mathf.FloorToInt(u * GridDim), 0, GridDim - 1);
        tileJ = Mathf.Clamp(Mathf.FloorToInt(v * GridDim), 0, GridDim - 1);
    }

    // ── Trigger methods ───────────────────────────────────────────────────────

    private void SetTimeBetweenDepthUpdate(GlobalSettings.GraphicsQualitySettings setting)
    {
        switch (setting)
        {
            case GlobalSettings.GraphicsQualitySettings.Low:    _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.x; break;
            case GlobalSettings.GraphicsQualitySettings.Medium: _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.y; break;
            case GlobalSettings.GraphicsQualitySettings.High:   _timeBetweenDepthUpdates = _timeBetweenDepthUpdatesPerGraphicsSettings.z; break;
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
        if (player == PlayerSystem.Instance.GetActivePlayerId() && updatedPositions.Count > 0)
            RenderSDFTextureThisFrame(updatedPositions[0]);
    }

    public void RenderSDFTextureThisFrame(Vector2Int gridPosition)
    {
        Vector3 worldPos = MapUtils.GetTileWorldPositionFromGridPosition(gridPosition, 0);

        int pixelX = Mathf.RoundToInt(worldPos.x / _worldToTextureScalingFactor * m_destinationDepthRenderTexture.width);
        int pixelY = Mathf.RoundToInt(worldPos.z / _worldToTextureScalingFactor * m_destinationDepthRenderTexture.height);

        WorldPosToTileIndex(worldPos, out int tileI, out int tileJ);
        _pendingPartialCenter[tileJ * GridDim + tileI] = new Vector2Int(pixelX, pixelY);
        _tileCameras[tileI, tileJ].enabled = true;
    }

    public void RenderSDFTextureThisFrame()
    {
        _pendingFullCount = GridDim * GridDim;
        // Clear any pending partial state so full-update cameras don't double-fire.
        for (int k = 0; k < _pendingPartialCenter.Length; k++)
            _pendingPartialCenter[k] = null;

        foreach (var cam in _tileCameras)
            cam.enabled = true;
    }

    // ── Post-render callback ──────────────────────────────────────────────────

    [Obsolete]
    public void PreRender(ScriptableRenderContext context, Camera camera) { }

    public void PostRender(ScriptableRenderContext context, Camera camera)
    {
        camera.enabled = false;

        if (!TryGetTileIndex(camera, out int tileI, out int tileJ))
            return;

        int fullW  = m_destinationDepthRenderTexture.width;
        int fullH  = m_destinationDepthRenderTexture.height;
        int tileW  = fullW / GridDim;
        int tileH  = fullH / GridDim;
        int offsetX = tileI * tileW;
        int offsetY = tileJ * tileH;

        // Copy tile render directly into the matching region of the combined RT (pure GPU copy).
        Graphics.CopyTexture(
            _tileRTs[tileI, tileJ], 0, 0, 0, 0, tileW, tileH,
            m_destinationDepthRenderTexture, 0, 0, offsetX, offsetY);

        int cameraIndex = tileJ * GridDim + tileI;
        Vector2Int? pendingCenter = _pendingPartialCenter[cameraIndex];

        if (pendingCenter.HasValue)
        {
            _pendingPartialCenter[cameraIndex] = null;
            if (m_onSDFDepthTextureChangedPartial != null && m_onSDFDepthTextureChangedPartial.GetInvocationList().Length > 0)
                m_onSDFDepthTextureChangedPartial(m_destinationDepthRenderTexture, pendingCenter.Value, _patchSizePixels);
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
