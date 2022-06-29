using System;
using Cinemachine;
using LittleBit.Modules.TouchInput;
using UnityEngine;
using DG.Tweening;

namespace LittleBit.Modules.CameraModule
{
    public class CameraService : IDisposable
    {
        private readonly Camera _camera;
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly TouchInputService _touchInputService;
        private readonly BoxCollider _cameraBounds;
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
        private float _currZoom;
        private float _startPitchDistance;

        public CameraService(Camera camera, CinemachineVirtualCamera virtualCamera, TouchInputService touchInputService,
            Transform cameraTarget, BoxCollider cameraBounds, CameraConfig cameraConfig)
        {
            _camera = camera;
            _virtualCamera = virtualCamera;
            _touchInputService = touchInputService;
            _cameraBounds = cameraBounds;
            _cameraConfig = cameraConfig;
            _cameraTarget = cameraTarget;

            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _recomposer = _virtualCamera.GetComponentInChildren<CinemachineRecomposer>();
            
            SubscribeOnTouchInputEvents();
            UpdateCamParameters();
        }
        
        public void SetZoom(float value)
        {
            float zoomValue = 0;
            switch (_cameraConfig.LensType)
            {
                case LensSettings.OverrideModes.Perspective:
                    zoomValue = Mathf.Lerp(_cameraConfig.MinDistance, _cameraConfig.MaxDistance, value);
                    break;
                case LensSettings.OverrideModes.Orthographic:
                    zoomValue = Mathf.Lerp(_cameraConfig.MinDistance, _cameraConfig.LensSize, value);
                    break;
            }
            
            DOTween.To(() => _currZoom, x => _currZoom = x, zoomValue, 1)
                .OnUpdate(() =>
                {
                    Debug.Log(_currZoom);
                    switch (_cameraConfig.LensType)
                    {
                        case LensSettings.OverrideModes.Perspective:
                            _transposer.m_TrackedObjectOffset = CalculateDistance(_currZoom);
                            break;
                        case LensSettings.OverrideModes.Orthographic:
                            _virtualCamera.m_Lens.OrthographicSize = _currZoom;
                            break;
                    }
                });
        }

        private Vector3 CalculateDistance(float distance)
        {
            var direction = Quaternion.Euler(_cameraConfig.CameraAngles) * Vector3.forward;

            return direction * -distance;
        }

        private void UpdateCamParameters()
        {
            _virtualCamera.m_Lens.ModeOverride = _cameraConfig.LensType;
            _virtualCamera.m_Lens.OrthographicSize = _cameraConfig.LensSize;
            _recomposer.m_Tilt = _cameraConfig.CameraAngles.x;
            _recomposer.m_Pan = _cameraConfig.CameraAngles.y;
            _recomposer.m_Dutch = _cameraConfig.CameraAngles.z;

            _recomposer.m_FollowAttachment = _cameraConfig.FollowSmooth;
            _recomposer.m_ZoomScale = _cameraConfig.ZoomScale;
            switch (_cameraConfig.LensType)
            {
                case LensSettings.OverrideModes.Perspective:
                    _currZoom = _cameraConfig.MaxDistance;
                    break;
                case LensSettings.OverrideModes.Orthographic:
                    _currZoom = _cameraConfig.LensSize;
                    break;
            }
            _transposer.m_TrackedObjectOffset = CalculateDistance(_cameraConfig.MaxDistance);
            
            _cameraBounds.center = _cameraConfig.CenterBounds;
            _cameraBounds.size = _cameraConfig.SizeBounds;
        }

        private void SubscribeOnTouchInputEvents()
        {
            _touchInputService.OnInputClick += OnClick;
            _touchInputService.OnDragStart += OnDragStart;
            _touchInputService.OnDragUpdate += OnDragUpdate;
            _touchInputService.OnDragStop += OnDragStop;
#if UNITY_EDITOR || UNITY_STANDALONE
            _touchInputService.OnMouseZoom += OnVirtualZoom;
#endif
#if UNITY_IPHONE || UNITY_ANDROID
            _touchInputService.OnPinchStart += OnPinchStart;
            _touchInputService.OnPinchStop += OnPinchStop;
            _touchInputService.OnPinchUpdateExtended += OnPinchUpdate;
#endif
        }

        private void OnVirtualZoom(float mouseDelta)
        {
            float maxDistance = 0;
            float minDistance = _cameraConfig.MinDistance;
            
            switch (_cameraConfig.LensType)
            {
                case LensSettings.OverrideModes.Perspective:
                    maxDistance = _cameraConfig.MaxDistance;
                    _currZoom += -mouseDelta * 5f;
                    _currZoom = Mathf.Clamp(_currZoom, minDistance, maxDistance);
                    _transposer.m_TrackedObjectOffset = CalculateDistance(_currZoom);
                    break;
                case LensSettings.OverrideModes.Orthographic:
                    maxDistance = _cameraConfig.LensSize;
                    _currZoom += -mouseDelta * 5f;
                    _currZoom = Mathf.Clamp(_currZoom, minDistance, maxDistance);
                    _virtualCamera.m_Lens.OrthographicSize = _currZoom;
                    break;
            }
        }

        private void OnClick(Vector3 clickposition, bool isdoubleclick, bool islongtap)
        {
            
        }

        private void OnPinchUpdate(PinchUpdateData pinchupdatedata)
        {
            float maxDistance = 0;
            float minDistance = _cameraConfig.MinDistance;
            float difference = _startPitchDistance - pinchupdatedata.pinchDistance;
            _startPitchDistance = pinchupdatedata.pinchDistance;
            
            switch (_cameraConfig.LensType)
            {
                case LensSettings.OverrideModes.Perspective:
                    maxDistance = _cameraConfig.MaxDistance;
                    _currZoom += difference * _cameraConfig.PerspectiveZoomDeviceSensitivity;
                    _currZoom = Mathf.Clamp(_currZoom, minDistance, maxDistance);
                    _transposer.m_TrackedObjectOffset = CalculateDistance(_currZoom);
                    break;
                case LensSettings.OverrideModes.Orthographic:
                    maxDistance = _cameraConfig.LensSize;
                    _currZoom += difference * _cameraConfig.OrthographicZoomDeviceSensitivity;
                    _currZoom = Mathf.Clamp(_currZoom, minDistance, maxDistance);
                    _virtualCamera.m_Lens.OrthographicSize = _currZoom;
                    break;
            }
        }

        private void OnPinchStop()
        {
            _startPitchDistance = 0;
        }

        private void OnPinchStart(Vector3 pinchcenter, float pinchdistance)
        {
            _startPitchDistance = pinchdistance;
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

        public void Dispose()
        {
            _touchInputService.OnInputClick -= OnClick;
            _touchInputService.OnDragStart -= OnDragStart;
            _touchInputService.OnDragUpdate -= OnDragUpdate;
            _touchInputService.OnDragStop -= OnDragStop;
            _touchInputService.OnMouseZoom -= OnVirtualZoom;
            _touchInputService.OnPinchStart -= OnPinchStart;
            _touchInputService.OnPinchStop -= OnPinchStop;
            _touchInputService.OnPinchUpdateExtended -= OnPinchUpdate;
        }
    }
}