using UnityEngine;
using UnityEngine.Rendering;
 
public class CameraRenderStepsInjectionLayer : MonoBehaviour
{

    [SerializeField] private SDFMaskCameraDepthWriter _cameraDepthWriter;
    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += PreRender;
        RenderPipelineManager.endCameraRendering += PostRender;
    }
 
    private void PreRender(ScriptableRenderContext _context, Camera _camera)
    {
        if (_camera.gameObject == _cameraDepthWriter.gameObject)
            _cameraDepthWriter.PreRender(_context, _camera);
    }
 
    private void PostRender(ScriptableRenderContext _context, Camera _camera)
    {
        
        if (_camera.gameObject == _cameraDepthWriter.gameObject)
            _cameraDepthWriter.PostRender(_context, _camera);
    }
 
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= PreRender;
        RenderPipelineManager.endCameraRendering -= PostRender;
    }
}