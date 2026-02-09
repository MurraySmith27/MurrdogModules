using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


public delegate void SDFDepthTextureChangedEvent(Texture2D depthTexture);
public class SDFMaskCameraDepthWriter : Singleton<SDFMaskCameraDepthWriter>
{
    public SDFDepthTextureChangedEvent m_onSDFDepthTextureChanged;

    [SerializeField] private RenderTexture temp;
    
    private Texture2D m_depthTexture;
    
    [SerializeField] private RenderTexture m_destinationDepthRenderTexture;

    private float _timeSinceLastDepthUpdate = 0f;

    [FormerlySerializedAs("_timeBetweenDepthUpdatesSeconds")] [SerializeField]
    private Vector3 _timeBetweenDepthUpdatesPerGraphicsSettings = Vector3.zero;

    [SerializeField] private RenderTexture m_dummyTempRenderTexture;

    [SerializeField] private bool _updateOverTime = false;

    private Camera _camera;

    private float _timeBetweenDepthUpdates;
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        // _camera.orthographicSize = transform.parent.localScale.x * 5;
        
        _camera.targetTexture = m_destinationDepthRenderTexture;
        
        SetTimeBetweenDepthUpdate(GlobalSettings.GraphicsQuality);

        GlobalSettings.OnGraphicsQualitySettingsChanged -= SetTimeBetweenDepthUpdate;
        GlobalSettings.OnGraphicsQualitySettingsChanged += SetTimeBetweenDepthUpdate;

        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;
        
        FogOfWarSystem.Instance.OnRevealedTilesUpdated -= OnFogOfWarUpdated;
        FogOfWarSystem.Instance.OnRevealedTilesUpdated += OnFogOfWarUpdated;
    }

    private void OnDestroy()
    {
        GlobalSettings.OnGraphicsQualitySettingsChanged -= SetTimeBetweenDepthUpdate;

        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        }

        if (FogOfWarSystem.IsAvailable)
        {
            FogOfWarSystem.Instance.OnRevealedTilesUpdated -= OnFogOfWarUpdated;
        }
    }

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
            default:
                break;
        }
    }
    
    private void Update()
    {
        if (_updateOverTime)
        {
            _timeSinceLastDepthUpdate += Time.deltaTime;
            if (_timeBetweenDepthUpdates == 0f)
            {
                _camera.enabled = true;
            }
            else if (_timeSinceLastDepthUpdate > _timeBetweenDepthUpdates)
            {
                _timeSinceLastDepthUpdate = 0f;
                _camera.enabled = true;
            }
        }
    }

    private void OnMapChunkGenerated(int row, int col, int width, int height, int layer)
    {
        RenderSDFTextureThisFrame();
    }

    private void OnFogOfWarUpdated(List<Vector2Int> updatedPositions, Guid player)
    {
        if (player == PlayerSystem.Instance.GetActivePlayerId())
            RenderSDFTextureThisFrame();
    }

    public void RenderSDFTextureThisFrame()
    {
        _camera.enabled = true;
    }
 
    [Obsolete]
    public void PreRender(ScriptableRenderContext _context, Camera _camera)
    {
        // UniversalRenderPipeline.RenderSingleCamera(_context, _camera); // NOTE: This is the obsolete method
    }
 
    public void PostRender(ScriptableRenderContext _context, Camera _camera)
    {
        _camera.enabled = false;
        
        //store depthTexure in prevDepthTexture:



        Texture _camDepthTexture = _camera.targetTexture;
        if (!m_depthTexture)
        {
            m_depthTexture = new Texture2D(_camDepthTexture.width, _camDepthTexture.height, TextureFormat.RGBA32, false);
        }
        
        Graphics.Blit(_camDepthTexture, temp);
        
        RenderTexture.active = temp;
        m_depthTexture.ReadPixels(new Rect(0, 0, _camDepthTexture.width, _camDepthTexture.height), 0, 0);
        m_depthTexture.Apply();

        if (m_onSDFDepthTextureChanged != null && m_onSDFDepthTextureChanged.GetInvocationList().Length > 0)
        {
            m_onSDFDepthTextureChanged(m_depthTexture);
        }
        
        
    }
}