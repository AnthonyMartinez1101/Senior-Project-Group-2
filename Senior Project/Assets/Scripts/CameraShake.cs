using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        //noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

}
