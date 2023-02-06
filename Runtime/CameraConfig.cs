using System;
using Cinemachine;
using UnityEngine;

namespace LittleBit.Modules.CameraModule
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/Camera Config", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        [SerializeField] private LensSettings.OverrideModes _lensType = LensSettings.OverrideModes.Perspective;
        [SerializeField] private float _lensSize = 20f;
        
        [SerializeField] private Vector3 _cameraAngles = new Vector3(30f, 0, 0);

        [SerializeField, Range(0,0.999f)] private float _followSmooth = .4f;
        [SerializeField, Range(0,20)] private float _damping = 1;
        
        [SerializeField, Range(0,1)] private float _zoomScale = 1f;

        [SerializeField] private float _perspectiveZoomDeviceSensitivity = 30f;
        [SerializeField] private float _orthographicZoomDeviceSensitivity = 20f;

        [SerializeField] protected float _MinDistance = 5f;
        [SerializeField] protected float _MaxDistance = 10f;

        [SerializeField] private Vector3 _centerBounds = new Vector3(0, 15f, 0);
        [SerializeField] private Vector3 _sizeBounds = new Vector3(60f, 30f, 60f);
        
        
        [Tooltip("Мультипликатор чувсвительности. Чем больше, большее расстояние будет проходить камера. При значении 1 камера будет проходить такое же расстояние как палец на экране")]
        [SerializeField, Range(0, 10)] private float _sensetivity = 1f;
        
        [Tooltip("Мультипликатор силы ускорения. Чем больше, тем большее расстояние пройдёт камера после отпускания пальца")]
        [SerializeField] private float _accelerationMultiply = 5;
        
        [Tooltip("Мультипликатор времени ускорения. Чем больше, дольше будет перемещаться камера после отпускания пальца")]
        [SerializeField] private float _timeAccelerationMultiply = 1;
        
        [Tooltip("Блокирует перемещение камеры при зуме")]
        [SerializeField] private bool _blockWhileZooming;

        public LensSettings.OverrideModes LensType => _lensType;

        public float LensSize => _lensSize;
        
        public Vector3 CameraAngles => _cameraAngles;
        
        public float FollowSmooth => _followSmooth;
        public float Damping => _damping;
        public float ZoomScale => _zoomScale;
        public float PerspectiveZoomDeviceSensitivity => _perspectiveZoomDeviceSensitivity;
        public float OrthographicZoomDeviceSensitivity => _orthographicZoomDeviceSensitivity;
        public Vector3 CenterBounds => _centerBounds;
        public Vector3 SizeBounds => _sizeBounds;
        
        public float MaxDistance => _MaxDistance;
        public float MinDistance => _MinDistance;
        public float Sensetivity => _sensetivity;

        public float AccelerationMultiply => _accelerationMultiply;

        public bool BlockWhileZooming => _blockWhileZooming;
        public float TimeAccelerationMultiply => _timeAccelerationMultiply;

        public event Action Updated;
        [SerializeField] private bool _realTimeUpdate;

        private void OnValidate()
        {
            if (_realTimeUpdate)
                Updated?.Invoke();
        }

        public void SetFollowSmooth(float value)
        {
            _followSmooth = value;
            if(_realTimeUpdate)
                Updated?.Invoke();
        }

        public void SetDamping(float value)
        {
            _damping = value;
            if(_realTimeUpdate)
                Updated?.Invoke();
        }

        public void SetAccelerationMultiply(float value)
        {
            _accelerationMultiply = value;
        }

        public void SetTimeAccelerationMultiply(float value)
        {
            _timeAccelerationMultiply = value;
        }
    }

   
}