using UnityEngine;
using UnityEngine.Rendering;
 
public class CameraRenderStepsInjectionLayer : MonoBehaviour
{
    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += PreRender;
        RenderPipelineManager.endCameraRendering += PostRender;
    }
 
    private void PreRender(ScriptableRenderContext _context, Camera _camera)
    {
        if (_camera.TryGetComponent<SDFMaskCameraDepthWriter>(out SDFMaskCameraDepthWriter _cameraRenderControl))
        {
            _cameraRenderControl.PreRender(_context, _camera);
        }
    }
 
    private void PostRender(ScriptableRenderContext _context, Camera _camera)
    {
        if (_camera.TryGetComponent<SDFMaskCameraDepthWriter>(out SDFMaskCameraDepthWriter _cameraRenderControl))
        {
            _cameraRenderControl.PostRender(_context, _camera);
        }
    }
 
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PreRender;
        RenderPipelineManager.endCameraRendering -= PostRender;
    }
}