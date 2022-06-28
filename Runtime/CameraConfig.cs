using System;
using Cinemachine;
using UnityEngine;

namespace LittleBit.Modules.CameraModule
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/Camera Config", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        public event Action OnConfigUpdate;

        [SerializeField] private LensSettings.OverrideModes _lensType = LensSettings.OverrideModes.Perspective;
        [SerializeField] private float _lensSize = 20f;
        
        [SerializeField] private Vector3 _cameraAngles = new Vector3(30f, 0, 0);

        [SerializeField, Range(0,1)] private float _followSmooth = .4f;
        
        [SerializeField, Range(0,1)] private float _zoomScale = 1f;

        [SerializeField] protected float _distance = 10f;

        [SerializeField] private Vector3 _centerBounds = new Vector3(0, 15f, 0);
        [SerializeField] private Vector3 _sizeBounds = new Vector3(60f, 30f, 60f);

        public LensSettings.OverrideModes LensType => _lensType;

        public float LensSize => _lensSize;
        
        public Vector3 CameraAngles => _cameraAngles;
        
        public float FollowSmooth => _followSmooth;
        public float ZoomScale => _zoomScale;
        public Vector3 CenterBounds => _centerBounds;
        public Vector3 SizeBounds => _sizeBounds;
        
        public float Distance => _distance;

        private void OnValidate()
        {
            OnConfigUpdate?.Invoke();
        }
    }
}