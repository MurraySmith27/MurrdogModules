using UnityEngine;
using UnityEngine.Rendering;

public class CameraRenderStepsInjectionLayer : MonoBehaviour
{
    [SerializeField] private SDFMaskCameraDepthWriter _cameraDepthWriter;

    private Camera _myCamera;

    /// <summary>
    /// Called when creating this component programmatically (e.g. on tile camera child GameObjects).
    /// Must be called after AddComponent so _myCamera can resolve from the same GameObject.
    /// </summary>
    public void Init(SDFMaskCameraDepthWriter writer)
    {
        _cameraDepthWriter = writer;
        _myCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (_myCamera == null) _myCamera = GetComponent<Camera>();
        RenderPipelineManager.beginCameraRendering += PreRender;
        RenderPipelineManager.endCameraRendering   += PostRender;
    }

    private void PreRender(ScriptableRenderContext context, Camera camera)
    {
        if (_cameraDepthWriter != null && camera == _myCamera)
            _cameraDepthWriter.PreRender(context, camera);
    }

    private void PostRender(ScriptableRenderContext context, Camera camera)
    {
        if (_cameraDepthWriter != null && camera == _myCamera)
            _cameraDepthWriter.PostRender(context, camera);
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PreRender;
        RenderPipelineManager.endCameraRendering   -= PostRender;
    }
}
