using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using UnityEngine;
using MEC;
using UnityEngine.Serialization;

public class CameraController : Singleton<CameraController>
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
    [SerializeField] private float cameraDecceleration = 0.1f;
    [SerializeField] private float cameraHardStopThreshold = 0.01f;
    [SerializeField] private float maxCameraVelocity = 5f;
    
    [Space(10)]
    
    [Header("Camera Focus On Tile Controls")]
    [SerializeField] private AnimationCurve cameraMoveAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float cameraMoveAnimTime = 0.5f;
    
    [Space(10)]
    
    [Header("Camera On Edge Of Screen Settings")]
    [SerializeField] private Vector4 edgeOfScreenMoveThresholds = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector2 edgeOfScreenCameraVelocityMinMax = new Vector2(0.1f, 0.2f);

    [Header("Camera Culling Update Settings")] 
    [SerializeField] private float cameraCullingUpdateDistance = 8f;
    [SerializeField] private float cameraCullingUpdateNormalizedZoomDistance = 0.1f;

    private CoroutineHandle _moveCoroutineHandle;

    private CinemachineTrackedDolly _trackedDolly;

    private bool _isDragging;

    private bool _isInEdgeOfScreen;

    private bool _isMovingOnScreenEdge;

    private Vector2 _mousePos;
    
    private Vector3 _dragStartWorldPosition;
    private Vector2 _dragStartScreenSpace;
    
    private bool _focusingPosition = false;
    
    private Vector2 _cameraVelocity = Vector2.zero;
    
    private Vector3 _cameraLastPosition = Vector3.zero;

    private Vector3 _cameraLastCullingUpdatePosition = Vector3.zero;

    private float _cameraLastCullingZoomNormalizedValue = 1f;
    
    public void Start()
    {
        inputChannel.LeftMouseDownEvent -= OnDragStarted;
        inputChannel.LeftMouseDownEvent += OnDragStarted;
        
        inputChannel.LeftMouseDoubleClickEvent -= OnDoubleClick;
        inputChannel.LeftMouseDoubleClickEvent += OnDoubleClick;

        inputChannel.LeftMouseUpEvent -= OnLeftMouseUp;
        inputChannel.LeftMouseUpEvent += OnLeftMouseUp;

        inputChannel.MouseMoveEvent -= OnMouseMove;
        inputChannel.MouseMoveEvent += OnMouseMove;
        
        inputChannel.MouseVerticalScrollEvent -= OnZoomAction;
        inputChannel.MouseVerticalScrollEvent += OnZoomAction;
        
        _trackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _trackedDolly.m_PathPosition = 1f;
        _trackedDolly.m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;
        
        horizontalWorldSpaceDirection = horizontalWorldSpaceDirection.normalized;
        verticalWorldSpaceDirection = verticalWorldSpaceDirection.normalized;
    }

    private void OnDestroy()
    {
        inputChannel.LeftMouseDownEvent -= OnDragStarted;
        inputChannel.LeftMouseUpEvent -= OnLeftMouseUp;
        inputChannel.MouseMoveEvent -= OnMouseMove;
        
        inputChannel.MouseVerticalScrollEvent -= OnZoomAction;
    }

    private void Update()
    {
        if (_isMovingOnScreenEdge)
        {
            bool inScreen = Screen.width > _mousePos.x && _mousePos.x > 0 && Screen.height > _mousePos.y && _mousePos.y > 0;
            bool inLeftEdge = inScreen && _mousePos.x < edgeOfScreenMoveThresholds.x * Screen.width;
            bool inRightEdge = inScreen && _mousePos.x > Screen.width - (edgeOfScreenMoveThresholds.y * Screen.width);
            bool inBottomEdge = inScreen && _mousePos.y < edgeOfScreenMoveThresholds.z * Screen.height;
            bool inTopEdge = inScreen && _mousePos.y > Screen.height - (edgeOfScreenMoveThresholds.w * Screen.height);
            
            float horizontalVelocity = 0f;
            if (inLeftEdge)
            {
                float leftThreshold = (edgeOfScreenMoveThresholds.x * Screen.width);
                horizontalVelocity = -Mathf.Lerp(
                    edgeOfScreenCameraVelocityMinMax.x,
                    edgeOfScreenCameraVelocityMinMax.y, 
                    1-(_mousePos.x / leftThreshold));
            }
            else if (inRightEdge)
            {
                float rightThresholdWidth = (edgeOfScreenMoveThresholds.y * Screen.width);
                float rightThreshold = Screen.width - rightThresholdWidth;
                horizontalVelocity = Mathf.Lerp(
                    edgeOfScreenCameraVelocityMinMax.x,
                    edgeOfScreenCameraVelocityMinMax.y, 
                    (_mousePos.x - rightThreshold) / rightThresholdWidth);
            }
            
            float verticalVelocity = 0f;
            if (inBottomEdge)
            {
                float bottomThreshold = (edgeOfScreenMoveThresholds.z * Screen.height);
                verticalVelocity = -Mathf.Lerp(
                    edgeOfScreenCameraVelocityMinMax.x,
                    edgeOfScreenCameraVelocityMinMax.y, 
                    1f - (_mousePos.y / bottomThreshold));
            }
            else if (inTopEdge)
            {
                float topThresholdHeight = (edgeOfScreenMoveThresholds.w * Screen.height);
                float topThreshold = Screen.height - topThresholdHeight;
                verticalVelocity = Mathf.Lerp(
                    edgeOfScreenCameraVelocityMinMax.x,
                    edgeOfScreenCameraVelocityMinMax.y, 
                    (_mousePos.y - topThreshold) / topThresholdHeight);
            }
            
            Vector3 newVelocityWorldSpace = horizontalVelocity * horizontalWorldSpaceDirection +
                                  verticalVelocity * verticalWorldSpaceDirection;
            
            if (Mathf.Abs(newVelocityWorldSpace.x) > Mathf.Abs(_cameraVelocity.x))
            {
                _cameraVelocity = new Vector2(newVelocityWorldSpace.x, _cameraVelocity.y);
            }
            
            if (Mathf.Abs(newVelocityWorldSpace.z) > Mathf.Abs(_cameraVelocity.y))
            {
                _cameraVelocity = new Vector2(_cameraVelocity.x, newVelocityWorldSpace.z);
            }
        }
        
        if (!_focusingPosition && !_isDragging)
        {
            float cameraSpeed = _cameraVelocity.magnitude;
            _cameraVelocity = _cameraVelocity.normalized * Mathf.Clamp(cameraSpeed - cameraDecceleration * Time.deltaTime, 0f, maxCameraVelocity);

            if (_cameraVelocity.magnitude > cameraHardStopThreshold)
            {
                SetCameraPosition(cameraFollowRoot.position + new Vector3(_cameraVelocity.x, 0, _cameraVelocity.y) * Time.deltaTime);
            }
        }
    }

    private void OnDragStarted(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (GlobalSettings.IsMapDraggingEnabled)
        {
            if (CameraUtils.GetCameraCenterPointOnPlane(mainCamera, out _dragStartWorldPosition)) {
                _isDragging = true;
                _dragStartScreenSpace = args.vector2Arg.GetValueOrDefault();
            }
        }
    }

    private void OnDoubleClick(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        Vector2 mousePos = args.vector2Arg.GetValueOrDefault();
        if (!_isDragging && !_focusingPosition)
        {
            Vector3 worldPosition;
            if (CameraUtils.GetPointOnPlaneFromMousePosition(mousePos, mainCamera, out worldPosition))
            {
                Vector3 tilePosition = MapUtils.GetTileCenterFromPlanePosition(worldPosition);
                FocusPosition(tilePosition);
            }
        }
    }

    private void OnLeftMouseUp(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        StopDrag();
    }

    private void StopDrag()
    {
        _isDragging = false;
    }
    
    private void OnMouseMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        _mousePos = args.vector2Arg.GetValueOrDefault();
        
        bool wasInEdgeOfScreen = _isInEdgeOfScreen;

        bool inScreen = Screen.width > _mousePos.x && _mousePos.x > 0 && Screen.height > _mousePos.y && _mousePos.y > 0;
        bool inLeftEdge = inScreen && _mousePos.x < edgeOfScreenMoveThresholds.x * Screen.width;
        bool inRightEdge = inScreen && _mousePos.x > Screen.width - (edgeOfScreenMoveThresholds.y * Screen.width);
        bool inBottomEdge = inScreen && _mousePos.y < edgeOfScreenMoveThresholds.z * Screen.height;
        bool inTopEdge = inScreen && _mousePos.y > Screen.height - (edgeOfScreenMoveThresholds.w * Screen.height);
        _isInEdgeOfScreen = inLeftEdge || inRightEdge || inTopEdge || inBottomEdge;

        if (!inScreen)
        {
            StopDrag();
        }
        
        if (_isDragging)
        {
            Vector2 dragDiff = _dragStartScreenSpace - _mousePos;
            
            float aspectRatio = Screen.width / (float)Screen.height;

            float horizontal = (dragDiff.x / Screen.width * cameraMoveSensitivity * aspectRatio); 
            float vertical = (dragDiff.y / Screen.height * cameraMoveSensitivity); 
            
            Vector3 cumulativeDragDiffWorldSpace = horizontalWorldSpaceDirection.normalized * horizontal + verticalWorldSpaceDirection.normalized * vertical;
            
            Vector3 newPosition = _dragStartWorldPosition - cumulativeDragDiffWorldSpace;
            
            newPosition = new Vector3(newPosition.x, 0, newPosition.z);

            Vector3 diffFromLastFrame = (_cameraLastPosition - newPosition) / Time.deltaTime;
            _cameraVelocity = new Vector2(diffFromLastFrame.x, diffFromLastFrame.z);
            
            _cameraLastPosition = newPosition;
            
            SetCameraPosition(_dragStartWorldPosition + cumulativeDragDiffWorldSpace);
        }
        else
        {
            if (!wasInEdgeOfScreen && _isInEdgeOfScreen)
            {
                _isMovingOnScreenEdge = true;
            }
            else if (wasInEdgeOfScreen && !_isInEdgeOfScreen)
            {
                _isMovingOnScreenEdge = false;
            }
        }
    }

    private void SetCameraPosition(Vector3 newPosition)
    {
        cameraFollowRoot.position = newPosition;

        if (Vector3.Distance(cameraFollowRoot.position, _cameraLastCullingUpdatePosition) > cameraCullingUpdateDistance)
        {
            _cameraLastCullingUpdatePosition = cameraFollowRoot.position;
            TileFrustrumCulling.Instance.UpdateTileCulling();
        }
    }

    public void FocusPosition(Vector3 worldPosition)
    {
        Timing.RunCoroutineSingleton(_MoveAnimationCoroutine(worldPosition), _moveCoroutineHandle,
            SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> _MoveAnimationCoroutine(Vector3 targetPosition)
    {
        _focusingPosition = true;
        Vector3 initialPos = cameraFollowRoot.position;
        for (float t = 0f; t <= cameraMoveAnimTime; t += Time.deltaTime)
        {
            SetCameraPosition(Vector3.Lerp(initialPos, targetPosition, cameraMoveAnimCurve.Evaluate(t / cameraMoveAnimTime)));
            yield return 0;
        }
        
        _focusingPosition = false;
    }

    private void OnZoomAction(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        AdjustZoom(-(args.floatArg.GetValueOrDefault()) / Screen.height);
    }

    public void AdjustZoom(float delta)
    {
        _trackedDolly.m_PathPosition = Mathf.Clamp(_trackedDolly.m_PathPosition + delta * cameraZoomSensitivity, 0f, 1f);

        if (_trackedDolly.m_PathPosition < 1e-5f || 
            _trackedDolly.m_PathPosition > (1f-1e5f) || 
            Mathf.Abs(_cameraLastCullingZoomNormalizedValue - _trackedDolly.m_PathPosition) > cameraCullingUpdateNormalizedZoomDistance)
        {
            _cameraLastCullingZoomNormalizedValue = _trackedDolly.m_PathPosition;
            TileFrustrumCulling.Instance.UpdateTileCulling();
        }
    }
}
