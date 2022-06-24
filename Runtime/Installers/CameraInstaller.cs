using Cinemachine;
using LittleBit.Modules.TouchInput;
using UnityEngine;
using Zenject;

namespace LittleBit.Modules.CameraModule
{
    public class CameraInstaller  : MonoInstaller
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private Collider _cameraBounds;
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
                .Bind<Collider>()
                .FromInstance(_cameraBounds)
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<CameraConfig>()
                .FromInstance(_cameraConfig)
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<CameraService>()
                .AsSingle()
                .NonLazy();
        }
    }
}