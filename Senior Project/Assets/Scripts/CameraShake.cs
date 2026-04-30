using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;

    private const string ShakeKey = "settings.shake"; // PlayerPrefs key for camera shake setting

    private readonly List<ShakeInstance> activeShakes = new List<ShakeInstance>();
    private readonly List<ConstantShakeInstance> constantShakes = new List<ConstantShakeInstance>();


    private class ShakeInstance
    {
        public float shakeIntensity;
        public float duration;
        public float elapsed;

        public ShakeInstance(float intensity, float dur)
        {
            shakeIntensity = intensity;
            duration = dur;
            elapsed = 0f;
        }
    }

    private class ConstantShakeInstance
    {
        public float intensity;
        public float duration;
        public float elapsed;

        public ConstantShakeInstance(float intensity, float duration)
        {
            this.intensity = intensity;
            this.duration = duration;
            elapsed = 0f;
        }
    }

    private void Awake()
    {
        enabled = PlayerPrefs.GetInt(ShakeKey, 1) == 1; // Enable or disable based on saved setting

        virtualCam = GetComponent<CinemachineCamera>();
        noise = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        ResetIntensity();
    }

    private void Update()
    {
        if (!enabled)
        {
            ResetIntensity();
            activeShakes.Clear();
            constantShakes.Clear();
            return;
        }

        float strongestShake = 0f;

        for (int i = activeShakes.Count - 1; i >= 0; i--)
        {
            ShakeInstance shake = activeShakes[i];

            shake.elapsed += Time.deltaTime;

            float t = shake.elapsed / shake.duration;
            float currentIntensity = Mathf.Lerp(shake.shakeIntensity, 0f, t);

            if (currentIntensity > strongestShake)
                strongestShake = currentIntensity;

            if (shake.elapsed >= shake.duration)
                activeShakes.RemoveAt(i);
        }

        for (int i = constantShakes.Count - 1; i >= 0; i--)
        {
            ConstantShakeInstance shake = constantShakes[i];

            shake.elapsed += Time.deltaTime;

            if (shake.intensity > strongestShake)
                strongestShake = shake.intensity;

            if (shake.elapsed >= shake.duration)
                constantShakes.RemoveAt(i);
        }

        noise.AmplitudeGain = strongestShake;
    }


    public void ShakeCamera(float intensity, float duration)
    {
        if(!enabled) return; //makes sure it doesnt shake if setting is off
        if (noise == null) return;
        if(intensity <= 0f || duration <= 0f) return;

        activeShakes.Add(new ShakeInstance(intensity, duration));
    }

    public void ConstantShakeCamera(float intensity, float duration)
    {
        if (!enabled) return;
        if (noise == null) return;
        if (intensity <= 0f || duration <= 0f) return;

        constantShakes.Add(new ConstantShakeInstance(intensity, duration));
    }

    //private IEnumerator ShakeCoroutine(float intensity, float shakeTime)
    //{
    //    float elapsed = 0f;

    //    while (elapsed < shakeTime)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / shakeTime;
    //        noise.AmplitudeGain = Mathf.Lerp(intensity, 0f, t);
    //        yield return null;
    //    }

    //    ResetIntensity();
    //}

    void ResetIntensity()
    {
        if(noise == null) return;
        noise.AmplitudeGain = 0f;
    }
}
