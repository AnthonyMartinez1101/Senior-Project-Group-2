using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private const string ShakeKey = "settings.shake"; // PlayerPrefs key for camera shake setting

    Coroutine shakeCoroutine;

    private void Awake()
    {
        enabled = PlayerPrefs.GetInt(ShakeKey, 1) == 1; // Enable or disable based on saved setting

        virtualCam = GetComponent<CinemachineCamera>();
        noise = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        ResetIntensity();
    }


    public void ShakeCamera(float intensity, float duration)
    {
        if(!enabled) return; //makes sure it doesnt shake if setting is off

        noise.AmplitudeGain = intensity;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float shakeTime)
    {
        float elapsed = 0f;

        while (elapsed < shakeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shakeTime;
            noise.AmplitudeGain = Mathf.Lerp(intensity, 0f, t);
            yield return null;
        }

        ResetIntensity();
    }

    void ResetIntensity()
    {
        noise.AmplitudeGain = 0f;
    }
}
