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
    private GameObject soil;
    private List<PlantScript> allPlants = new List<PlantScript>();

    private Transform currentTarget;
    private NavMeshAgent agent;

    private float waitThreshold = 5f;
    private float waitTimer = 0f;

    private WorldClock clock;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        waitTimer = waitThreshold;
    }
    void Update()
    {
        //If enemy has no target, find one
        if(soil != null && currentTarget == null)
        {
            PlantScript[] plants = soil.GetComponentsInChildren<PlantScript>();
            allPlants.Clear();
            foreach(PlantScript plant in plants)
            {
                if(plant != null && plant.BugsCanTarget)
                {
                    allPlants.Add(plant);
                }
            }
            if(allPlants.Count > 0)
            {
                int randomIndex = Random.Range(0, allPlants.Count);
                currentTarget = allPlants[randomIndex].transform;
                waitTimer = waitThreshold;
            }
            else
            {
                //If no plants found within waitThreshold, destroy self (to prevent large enemy buildup)
                waitTimer -= Time.deltaTime;
                if(waitTimer < 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        //If target is found, move towards it
        if(agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }

        //Hit behavior 
        hitTimer -= Time.deltaTime;
        if(hitTimer < 0f && inPlantRange)
        {
            AttackPlant();
            hitTimer = hitCooldown;
        }

        if(clock && clock.IsNight())
        {
            Destroy(gameObject);
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

    public void GiveClock(WorldClock c)
    {
        clock = c;
    }
}
