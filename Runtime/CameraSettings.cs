using Cinemachine;
using LittleBit.Modules.CameraModule;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraConfig _cameraConfig;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private BoxCollider _cameraBounds;
    private CinemachineFramingTransposer _transposer;
    private CinemachineRecomposer _recomposer;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (!_transposer)
                _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (!_recomposer)
                _recomposer = _virtualCamera.GetComponentInChildren<CinemachineRecomposer>();
            UpdateCamParameters();
        }
    }

    private Vector3 CalculateDistance(float distance)
    {
        var direction = Quaternion.Euler(_cameraConfig.CameraAngles) * Vector3.forward;

        return direction * distance;
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

        _transposer.m_TrackedObjectOffset = CalculateDistance(-_cameraConfig.MaxDistance);

        _cameraBounds.center = _cameraConfig.CenterBounds;
        _cameraBounds.size = _cameraConfig.SizeBounds;
    }
}