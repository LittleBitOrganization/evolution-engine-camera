using System;
using UnityEngine;

namespace LittleBit.Modules.CameraModule
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/Camera Config", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        public event Action OnConfigUpdate;
        
        [SerializeField] private float _xAngle = 0;
        [SerializeField] private float _yAngle = 0;
        [SerializeField] private float _zAngle = 0;

        [SerializeField, Range(0,1)] private float _followSmooth = .4f;
        
        [SerializeField, Range(0,1)] private float _zoomScale = 1f;
        
        [SerializeField] private float _xOffset = 0;
        [SerializeField] private float _yOffset = 0;
        [SerializeField] private float _zOffset = 0;

        [SerializeField] protected float _distance = 0;


        public float XAngle => _xAngle;
        public float YAngle => _yAngle;
        public float ZAngle => _zAngle;
        
        public float FollowSmooth => _followSmooth;
        public float ZoomScale => _zoomScale;
        
        public float XOffset => _xOffset;
        public float YOffset => _yOffset;
        public float ZOffset => _zOffset;
        
        public float Distance => _distance;

        private void OnValidate()
        {
            OnConfigUpdate?.Invoke();
        }
    }
}