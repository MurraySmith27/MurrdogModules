using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using UnityEngine;
using MEC;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private OrpheusUIInputChannel inputChannel;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform cameraFollowRoot;
    
    [Space(10)]
    
    [Header("Camera Controls")] 
    [SerializeField] private float cameraZoomSensitivity = 0.1f;
    [SerializeField] private float cameraMoveSensitivity = 0.1f;
    [SerializeField] private Vector3 horizontalWorldSpaceDirection = new Vector3(-1f, 0, 1f);
    [SerializeField] private Vector3 verticalWorldSpaceDirection = new Vector3(1f, 0, 1f);
    [SerializeField] private AnimationCurve cameraMoveAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float cameraMoveAnimTime = 0.5f;

    private CoroutineHandle _moveCoroutineHandle;

    private CinemachineTrackedDolly _trackedDolly;

    private bool _isDragging;

    private Vector3 _dragStartWorldPosition;
    private Vector2 _dragStartScreenSpace;

    private Vector3 _targetPosition;
    
    public void Start()
    {
        inputChannel.LeftMouseDownEvent -= OnDragStarted;
        inputChannel.LeftMouseDownEvent += OnDragStarted;

        inputChannel.LeftMouseUpEvent -= OnDragFinished;
        inputChannel.LeftMouseUpEvent += OnDragFinished;

        inputChannel.MouseMoveEvent -= OnDragMove;
        inputChannel.MouseMoveEvent += OnDragMove;
        
        inputChannel.MouseVerticalScrollEvent -= OnZoomAction;
        inputChannel.MouseVerticalScrollEvent += OnZoomAction;
        
        _trackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _trackedDolly.m_PathPosition = 1f;
        _trackedDolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
    }

    private void OnDestroy()
    {
        inputChannel.LeftMouseDownEvent -= OnDragStarted;
        inputChannel.LeftMouseUpEvent -= OnDragFinished;
        inputChannel.MouseMoveEvent -= OnDragMove;
        
        inputChannel.MouseVerticalScrollEvent -= OnZoomAction;
    }

    private void OnDragStarted(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        _isDragging = true;
        _dragStartWorldPosition =
            CameraUtils.GetCameraCenterPointOnPlane(mainCamera);
        _dragStartScreenSpace = args.vector2Arg.GetValueOrDefault();
        
        Debug.LogError($"DragStartScreenSpace: {_dragStartScreenSpace}");
    }

    private void OnDragFinished(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        _isDragging = false;
    }

    private void OnDragMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (_isDragging)
        {
            Vector2 mousePos = args.vector2Arg.GetValueOrDefault();
            Vector2 dragDiff = _dragStartScreenSpace - mousePos;
            
            Debug.LogError($"mousepos: {mousePos}");

            float aspectRatio = Screen.width / (float)Screen.height;
            
            Vector3 cumulativeDragDiffWorldSpace = horizontalWorldSpaceDirection.normalized * dragDiff.x * cameraMoveSensitivity + verticalWorldSpaceDirection.normalized * dragDiff.y * cameraMoveSensitivity * aspectRatio;

            FocusPosition(cumulativeDragDiffWorldSpace);
        }
    }

    public void FocusPosition(Vector3 worldPosition)
    {
        cameraFollowRoot.position = worldPosition;
        // _targetPosition = worldPosition;
        // Timing.RunCoroutineSingleton(_MoveAnimationCoroutine(worldPosition), _moveCoroutineHandle,
        //     SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> _MoveAnimationCoroutine()
    {
        // Vector3 initialPos = cameraFollowRoot.position;
        // for (float t = 0f; t <= cameraMoveAnimTime; t += Time.deltaTime)
        // {
        //     cameraFollowRoot.position = Vector3.Lerp(initialPos, targetPosition, cameraMoveAnimCurve.Evaluate(t / cameraMoveAnimTime));
        //     yield return 0;
        // }
        yield break;
    }

    private void OnZoomAction(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        AdjustZoom(-(args.floatArg.GetValueOrDefault()) / Screen.height);
    }

    public void AdjustZoom(float delta)
    {
        _trackedDolly.m_PathPosition = Mathf.Clamp(_trackedDolly.m_PathPosition + delta * cameraZoomSensitivity, 0f, 1f);
    }
}
