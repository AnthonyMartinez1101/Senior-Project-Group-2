using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DayEnemy : MonoBehaviour, IDamageable, IPoisonable
{
    private FloatingHealth healthbar;   

    public float dealDamage = 1f;
    public float hitCooldown = 1.5f;
    private float hitTimer = 0f;

    private bool inPlantRange = false;
    public GameObject soil;
    private List<PlantScript> allPlants = new List<PlantScript>();

    private Transform currentTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if(soil != null && currentTarget == null)
        {
            PlantScript[] plants = soil.GetComponentsInChildren<PlantScript>();
            allPlants.Clear();
            foreach(PlantScript plant in plants)
            {
                if(plant != null)
                {
                    allPlants.Add(plant);
                }
            }
            if(allPlants.Count > 0)
            {
                int randomIndex = Random.Range(0, allPlants.Count);
                currentTarget = allPlants[randomIndex].transform;
            }
        }
        if(agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
        hitTimer -= Time.deltaTime;
        if(hitTimer < 0f && inPlantRange)
        {
            AttackPlant();
            hitTimer = hitCooldown;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Plant"))
        {
            inPlantRange = true;
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Plant"))
        {
            inPlantRange = false;
        }
    }
    void AttackPlant()
    {
        if(currentTarget == null) return;
        IDamageable damageable = currentTarget.GetComponent<IDamageable>();
        if (damageable == null) return; 
        damageable.TakeDamage(dealDamage, DamageType.Enemy);
    }

    public void ApplyPoison(int ticks)
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damageDealt, DamageType damageType)
    {
        Destroy(gameObject);

    }

    public void GiveSoil(GameObject s)
    {
        soil = s;
    }
}
