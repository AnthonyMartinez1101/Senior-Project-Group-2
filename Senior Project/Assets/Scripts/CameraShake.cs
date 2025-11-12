using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;


    private void Awake()
    {
        virtualCam = GetComponent<CinemachineCamera>();
        noise = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        ResetIntensity();
    }


    public void ShakeCamera(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;
        StartCoroutine(ShakeCoroutine(duration));
    }

    private IEnumerator ShakeCoroutine(float shakeTime)
    {
        yield return new WaitForSeconds(shakeTime);
        ResetIntensity();
    }

    void ResetIntensity()
    {
        noise.AmplitudeGain = 0f;
    }
}
