using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class HopWhileMoving : MonoBehaviour
{
    public Transform spriteTransform;
    public Transform shadowTransform;
    public float hopHeight = 0.2f;
    public float hopDuration = 0.3f;

    private NavMeshAgent agent;
    public float moveStartThreshold = 0.05f; // start hopping if speed > this
    public float moveStopThreshold = 0.02f; // stop hopping if speed < this

    private bool isHopping = false;
    private bool moving = false;

    private float baseSpriteY;
    private Vector3 baseShadowScale;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        baseSpriteY = spriteTransform.localPosition.y;
        baseShadowScale = shadowTransform.localScale;
    }

    void Update()
    {
        // Check if moving to stop hopping
        float speed = (agent && agent.enabled && agent.isOnNavMesh) ? agent.velocity.magnitude : 0f;

        if (!moving && speed > moveStartThreshold) moving = true;
        else if (moving && speed < moveStopThreshold) moving = false;

        // Start hopping once when movement begins
        if (moving && !isHopping) StartHop();
    }

    void StartHop()
    {
        isHopping = true;

        // Kill any tweens on these transforms (prevents buildup)
        spriteTransform.DOKill();
        shadowTransform.DOKill();

        // Shrink shadow as the sprite moves up
        shadowTransform.DOScale(baseShadowScale * 0.7f, hopDuration / 2f).SetEase(Ease.OutQuad);

        // Animate sprite upward
        spriteTransform.DOLocalMoveY(baseSpriteY + hopHeight, hopDuration / 2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // Expand shadow as the sprite moves down
                shadowTransform.DOScale(baseShadowScale, hopDuration / 2f).SetEase(Ease.InQuad);

                // Animate sprite downward
                spriteTransform.DOLocalMoveY(baseSpriteY, hopDuration / 2f).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        isHopping = false;

                        // If we're still moving, immediately hop again (smooth loop)
                        if (moving)StartHop();
                    });
            });
    }
}
