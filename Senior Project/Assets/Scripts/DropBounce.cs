using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class DropBounce : MonoBehaviour
{
    [SerializeField] Transform mainSprite;
    [SerializeField] Transform shadowSprite;
    [SerializeField] SpriteRenderer shadowSR;

    [Header("Bounce tuning")]
    [SerializeField] float startHeight = 1.5f;
    [SerializeField] float gravity = 20f;
    [SerializeField] float restitution = 0.45f;
    [SerializeField] float stopVel = 0.05f;
    [SerializeField] float stopHeight = 0.02f;

    [Header("Shadow look")]
    [SerializeField] float minShadowScale = 0.9f; 
    [SerializeField] float maxShadowScale = 1.1f; 
    [SerializeField] float minShadowAlpha = 0.3f; 
    [SerializeField] float maxShadowAlpha = 0.8f; 

    Vector3 mainBaseLocalPos;
    Vector3 shadowBaseLocalPos;
    Vector3 shadowBaseScale;

    private ItemDropAudio dropAudio;


    void Awake()
    {
        if (!mainSprite) mainSprite = transform.GetChild(0);
        if (!shadowSprite) shadowSprite = transform.GetChild(1);

        mainBaseLocalPos = mainSprite.localPosition;
        shadowBaseLocalPos = shadowSprite.localPosition;
        shadowBaseScale = shadowSprite.localScale;

        dropAudio = GetComponent<ItemDropAudio>();
    }


    public void StartBounce() => StartCoroutine(Bounce(startHeight));

    IEnumerator Bounce(float height)
    {
        height = Random.Range(height * 0.7f, height * 1.3f);
        float y = Mathf.Max(0f, height);
        float v = 0f;

        mainSprite.localPosition = new Vector3(mainBaseLocalPos.x, y, mainBaseLocalPos.z);
        shadowSprite.localPosition = shadowBaseLocalPos;

        while (Mathf.Abs(v) > stopVel || y > stopHeight)
        {
            float dt = Time.deltaTime;

            v += -gravity * dt;
            y += v * dt;

            if (y <= 0f)
            {
                y = 0f;

                dropAudio.PlayDropSound();

                v = -v * restitution;
            }

            mainSprite.localPosition = new Vector3(mainBaseLocalPos.x, y, mainBaseLocalPos.z);

            if (shadowSR)
            {
                float t = Mathf.InverseLerp(height, 0f, y);
                float scaleFactor = Mathf.Lerp(minShadowScale, maxShadowScale, t);
                shadowSprite.localScale = new Vector3(
                    shadowBaseScale.x * scaleFactor,
                    shadowBaseScale.y * scaleFactor,
                    shadowBaseScale.z
                );

                var c = shadowSR.color;
                c.a = Mathf.Lerp(minShadowAlpha, maxShadowAlpha, t);
                shadowSR.color = c;
            }

            yield return null;
        }

        mainSprite.localPosition = mainBaseLocalPos;
        shadowSprite.localPosition = shadowBaseLocalPos;

        if (shadowSR)
        {
            float scaleFactor = maxShadowScale;
            shadowSprite.localScale = new Vector3(
                shadowBaseScale.x * scaleFactor,
                shadowBaseScale.y * scaleFactor,
                shadowBaseScale.z
            );

            var c = shadowSR.color; c.a = maxShadowAlpha; shadowSR.color = c;
        }
    }
}

