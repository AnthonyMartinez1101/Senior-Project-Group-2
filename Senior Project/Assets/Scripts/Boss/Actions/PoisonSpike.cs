using UnityEngine;

public class PoisonSpike : MonoBehaviour
{
    public int poisonTick = 3;

    private void OnTriggerEnter2D(Collider2D player)
    {
        if(player.CompareTag("Player"))
        { 
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if(playerHealth != null)
            {
                playerHealth.ApplyPoison(poisonTick);
            }
        }
    }
}