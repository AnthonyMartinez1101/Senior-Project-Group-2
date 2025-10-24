using UnityEngine;
using UnityEngine.AI;

public class EnemyFaceFlip : MonoBehaviour
{
    private NavMeshAgent agent;  
    private SpriteRenderer sr;          // your sprite
    private float deadZone = 0.05f;     // ignore jitters

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;    
    }

    void LateUpdate()
    {
        if (!agent || !sr) return;

        Vector3 v = agent.velocity;           // actual current velocity

        float vx = v.x;
        if (Mathf.Abs(vx) > deadZone) sr.flipX = vx < 0f;
    }
}
