using Cinemachine;
using LittleBit.Modules.TouchInput;
using UnityEngine;
using Zenject;

namespace LittleBit.Modules.CameraModule
{
    public class CameraService : IInitializable
    {
        private readonly Camera _camera;
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly TouchInputService _touchInputService;
        private readonly Collider _cameraBounds;
        private readonly CameraConfig _cameraConfig;
        private readonly Transform _cameraTarget;
        private readonly CinemachineFramingTransposer _transposer;
        private readonly CinemachineRecomposer _recomposer;

        private readonly Plane _refPlaneXZ = new Plane(new Vector3(0, 1, 0), 0);
        
        private Vector3 _startPos;
        
        private float _pinchStartValue;
        private float _xUpper;
        private float _xLower;
        private float _zUpper;
        private float _zLower;

        public CameraService(Camera camera, CinemachineVirtualCamera virtualCamera, TouchInputService touchInputService,
            Transform cameraTarget, Collider cameraBounds, CameraConfig cameraConfig)
        {
            _camera = camera;
            _virtualCamera = virtualCamera;
            _touchInputService = touchInputService;
            _cameraBounds = cameraBounds;
            _cameraConfig = cameraConfig;
            _cameraTarget = cameraTarget;

            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _recomposer = _virtualCamera.GetComponentInChildren<CinemachineRecomposer>();
        }

        public void Initialize()
        {
            SubscribeOnTouchInputEvents();
            _cameraConfig.OnConfigUpdate += UpdateCamParameters;
        }

        private void UpdateCamParameters()
        {
            _recomposer.m_Tilt = _cameraConfig.XAngle;
            _recomposer.m_Pan = _cameraConfig.YAngle;
            _recomposer.m_Dutch = _cameraConfig.ZAngle;

            _recomposer.m_FollowAttachment = _cameraConfig.FollowSmooth;
            _recomposer.m_ZoomScale = _cameraConfig.ZoomScale;

            _transposer.m_TrackedObjectOffset =
                new Vector3(_cameraConfig.XOffset, _cameraConfig.YOffset, _cameraConfig.ZOffset);

            _transposer.m_CameraDistance = _cameraConfig.Distance;
        }

        private void SubscribeOnTouchInputEvents()
        {
            //_touchInputService.OnInputClick += OnClick;
            _touchInputService.OnDragStart += OnDragStart;
            _touchInputService.OnDragUpdate += OnDragUpdate;
            _touchInputService.OnDragStop += OnDragStop;
            // _touchInputService.OnPinchStart += OnPinchStart;
            // _touchInputService.OnPinchStop += OnPinchStop;
            // _touchInputService.OnPinchUpdateExtended += OnPinchUpdate;
        }

        private void OnClick(Vector3 clickposition, bool isdoubleclick, bool islongtap)
        {
            
        }

        private void OnPinchUpdate(PinchUpdateData pinchupdatedata)
        {
            
        }

        private void OnPinchStop()
        {
            
        }

        private void OnPinchStart(Vector3 pinchcenter, float pinchdistance)
        {
            
        }

        private void OnDragStart(Vector3 pos, bool islongtap)
        {
            _startPos = _cameraTarget.position;
            SetBorders();
        }

        private void OnDragUpdate(Vector3 dragposstart, Vector3 dragposcurrent, Vector3 correctionoffset)
        {
            var start = GetPointOnPlane(dragposstart);
            var end = GetPointOnPlane(dragposcurrent + correctionoffset);

            var difference = end - start;
            CheckBorders(difference);
        }
        
        private void OnDragStop(Vector3 dragstoppos, Vector3 dragfinalmomentum)
        {
            
        }

        private Vector3 GetPointOnPlane(Vector3 dragposstart)
        {
            Ray ray = _camera.ScreenPointToRay(dragposstart);
            float distance = 0;
            Plane plane = _refPlaneXZ;
            bool success = plane.Raycast(ray, out distance);
            if (success == false) {
                Debug.LogWarning("Failed to compute intersection between camera ray and reference plane. Make sure the camera Axes are set up correctly.");
            }
            Vector3 point = ray.GetPoint(distance);
            return point;
        }

        private void CheckBorders(Vector3 dragVector)
        {
            var pos = _startPos - dragVector;

            pos.x = Mathf.Clamp(pos.x, _xLower, _xUpper);
            pos.z = Mathf.Clamp(pos.z, _zLower, _zUpper);
            
            _cameraTarget.position = pos;
        }

        private void SetBorders()
        {
            var bounds = _cameraBounds.bounds;
            
            // var yHalfExtents = collider.bounds.extents.y;
            // var yCenter = collider.bounds.center.y;
            // float yUpper = yCenter + yHalfExtents;
            // float yLower = yCenter - yHalfExtents;
            
            var xHalfExtents = bounds.extents.x;
            var xCenter = bounds.center.x;
            _xUpper = xCenter + xHalfExtents;
            _xLower = xCenter - xHalfExtents;
            
            var zHalfExtents = bounds.extents.z;
            var zCenter = bounds.center.z;
            _zUpper = zCenter + zHalfExtents;
            _zLower = zCenter - zHalfExtents;
        }
    }
}