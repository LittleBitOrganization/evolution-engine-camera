﻿using System;
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
        private Vector3 _start;
        private bool _pinch;

        public bool Enabled { get; private set; } = false;

        public CameraService(Camera camera, CinemachineVirtualCamera virtualCamera, TouchInputService touchInputService,
                             Transform cameraTarget, BoxCollider cameraBounds, CameraConfig cameraConfig)
        {
            _camera = camera;
            _virtualCamera = virtualCamera;
            _touchInputService = touchInputService;
            _cameraBounds = cameraBounds;
            _cameraConfig = cameraConfig;
            _cameraTarget = cameraTarget;
            _cameraConfig.Updated += UpdateCamParameters;

            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _recomposer = _virtualCamera.GetComponentInChildren<CinemachineRecomposer>();

            EnableCam();
            UpdateCamParameters();
            _startPos = _cameraTarget.position;
            SetBorders();
            MoveToPosition(_cameraTarget.position);
        }

        private void UnSubscribeOnTouchInputEvents()
        {
            _touchInputService.OnInputClickDown -= OnClick;
                _touchInputService.OnDragStart -= OnDragStart;
                _touchInputService.OnDragUpdate -= OnDragUpdate;
                _touchInputService.OnDragStop -= OnDragStop;
                _touchInputService.OnMouseZoom -= OnVirtualZoom;
                _touchInputService.OnPinchStart -= OnPinchStart;
                _touchInputService.OnPinchStop -= OnPinchStop;
                _touchInputService.OnPinchUpdateExtended -= OnPinchUpdate;
            
        }

        public void DisableCam()
        {
            if (Enabled)
            {
                Enabled = false;
                _moveTweener?.Kill();
                UnSubscribeOnTouchInputEvents();
            }
        }

        public void EnableCam()
        {
            if (Enabled == false)
            {
                Enabled = true;
                _moveTweener?.Kill();
                SubscribeOnTouchInputEvents();
            }
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

        public void MoveToPosition(Vector3 position)
        {
            _moveTweener?.Kill();
            _startPos = _cameraTarget.position;
            SetBorders();
            
            position.y = _cameraTarget.position.y;
            position.x = Mathf.Clamp(position.x, _xLower, _xUpper);
            position.z = Mathf.Clamp(position.z, _zLower, _zUpper);

            _cameraTarget.position = position;
        }
	
	public void SmoothMoveToPosition(Vector3 position, float duration)
        {
            //_cameraTarget.DOKill();
            _startPos = _cameraTarget.position;
            SetBorders();
            position.y = _cameraTarget.position.y;
            position.x = Mathf.Clamp(position.x, _xLower, _xUpper);
            position.z = Mathf.Clamp(position.z, _zLower, _zUpper);
            
            _cameraTarget.DOMove(position,duration);
        }

        public void SetSceenY(float value)
        {
            value = Mathf.Clamp(value, 0, 1);
            DOVirtual.Float(_transposer.m_ScreenY,value,1f, (x) =>
            {
                _transposer.m_ScreenY = x;
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
            
            _transposer.m_XDamping = _cameraConfig.Damping;
            _transposer.m_YDamping = _cameraConfig.Damping;
            _transposer.m_ZDamping = _cameraConfig.Damping;
            
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
            _touchInputService.OnInputClickDown += OnClick;
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
        
        
        private void OnClick(Vector3 pos)
        {
            _moveTweener?.Kill();
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
            _pinch = false;
            _startPitchDistance = 0;
        }

        private void OnPinchStart(Vector3 pinchcenter, float pinchdistance)
        {
            _startPitchDistance = pinchdistance;
            _pinch = true;
        }

        private void OnDragStart(Vector3 pos, bool islongtap)
        {
            _moveTweener?.Kill();
            if (_cameraConfig.BlockWhileZooming && _pinch) return;
          
            _startPos = _cameraTarget.position;
            _acceleration = null;
            SetBorders();
            
        }

        private void OnDragUpdate(Vector3 dragposstart, Vector3 dragposcurrent, Vector3 correctionoffset)
        {            
            _moveTweener?.Kill();
            if (_cameraConfig.BlockWhileZooming && _pinch) return;
           
            
            var start = GetPointOnPlane(dragposstart);
            var end = GetPointOnPlane(dragposcurrent + correctionoffset);

            var difference = end - start;
            difference *= _cameraConfig.Sensetivity;
            
            MoveCamera(difference);
        }
        
        
        private void OnDragStop(Vector3 dragstoppos, Vector3 dragfinalmomentum)
        {
            var currentPosition = _cameraTarget.transform.position;
            var lastPosition = _lastCameraPosition;
           
            
            _acceleration = new Acceleration(currentPosition, lastPosition, Time.deltaTime, _cameraConfig.AccelerationMultiply, _cameraConfig.TimeAccelerationMultiply);
            
            Vector3 target = _acceleration.TargetPosition;
            _moveTweener = DOVirtual.Vector3(_cameraTarget.position, target, _acceleration.TargetTime, (value) =>
            {
                MoveCamera(Vector3.zero, -value);
            });

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

        private Vector3 GetPositionWithCheckBorders(Vector3 startPos, Vector3 dragVector)
        {
            var pos = startPos - dragVector;
            
            pos.x = Mathf.Clamp(pos.x, _xLower, _xUpper);
            pos.z = Mathf.Clamp(pos.z, _zLower, _zUpper);
            return pos;
        }

        private Vector3 _lastCameraPosition;
        private Acceleration _acceleration;
            
        private void MoveCamera(Vector3 dragVector)
        {
           
            var position = GetPositionWithCheckBorders(_startPos, dragVector);
            _lastCameraPosition = _cameraTarget.position;
            _cameraTarget.position = position;
        }

        private void MoveCamera(Vector3 startPos, Vector3 dragVector)
        {
            
            var position = GetPositionWithCheckBorders(startPos, dragVector);
            _lastCameraPosition = _cameraTarget.position;
            _cameraTarget.position = position;
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
            DisableCam();
        }

        private Tweener _moveTweener;
        
        internal class Acceleration
        {
            private readonly float _accelerationMultiply;
            private readonly float _timeAccelerationMultiply;
            private readonly float _acceleration;
            private readonly Vector3 _direction;
            private readonly float _distance;
            

            public readonly float TargetDistance;
            private readonly Vector3 _targetPosition;
            private readonly float _speed;
            private readonly float _targetTime;

            public Vector3 TargetPosition => _targetPosition;

            public float TargetTime => _targetTime * _timeAccelerationMultiply;

            public Acceleration(Vector3 currentPosition, Vector3 lastPosition, float time,
                float accelerationMultiply,
                float timeAccelerationMultiply)
            {
                _accelerationMultiply = accelerationMultiply;
                _timeAccelerationMultiply = timeAccelerationMultiply;
                var difference = currentPosition - lastPosition;
                _direction = difference.normalized;
                _distance = difference.magnitude;
                
                if (time == 0)
                    time = 0.1f;
                        
                _speed = _distance / time;
                
                _acceleration = _distance / (time * time);

                TargetDistance = _distance * _accelerationMultiply;

                _targetPosition = currentPosition + TargetDistance * _direction;

                if (_speed != 0)
                    _targetTime = TargetDistance / _speed;
                else
                    _targetTime = 0.1f;
            }
        }
    }

}
