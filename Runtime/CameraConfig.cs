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
        [SerializeField] private float _sensetivity = 1f;
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

        public bool BlockWhileZooming => _blockWhileZooming;

        public event Action Updated;
        [SerializeField] private bool _realTimeUpdate;

        private void OnValidate()
        {
            if (_realTimeUpdate)
                Updated?.Invoke();
        }
    }
}