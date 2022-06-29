using Cinemachine;
using UnityEngine;
using Zenject;

namespace LittleBit.Modules.CameraModule
{
    public class CameraInstaller  : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private BoxCollider _cameraBounds;
        [SerializeField] private CameraConfig _cameraConfig;
    
        public override void InstallBindings()
        {
            BindCamera();
        }

        private void BindCamera()
        {
            Container
                .Bind<Camera>()
                .FromInstance(_camera)
                .AsSingle()
                .NonLazy();
        
            Container
                .Bind<CinemachineVirtualCamera>()
                .FromInstance(_virtualCamera)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<Transform>()
                .FromInstance(_cameraTarget)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<BoxCollider>()
                .FromInstance(_cameraBounds)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<CameraConfig>()
                .FromInstance(_cameraConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<CameraService>()
                .AsSingle()
                .NonLazy();
        }
    }
}