using UnityEngine;
using System.Collections.Generic;
using TMPro;
using WorldTime;

public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> enemyPrefab; //Only works with 3 items in here
    public List<Transform> spawnPoints; //Only works with 10 items in here
    public Transform player;

    private float timer = 10f;
    private float spawnRate = 3f;
    private int wave = 1;

    [SerializeField] private WorldTime.WorldTime worldTime;

    //private float worldTimer = 0f; 
    //private bool isDay = true;

    private void Update()
    {
        if(worldTime == null)
        {
            Debug.LogWarning("EnemySpawner: WorldTime reference is not set.");
            return;
        }

        if (worldTime.CurrentPhase == Phase.Night)
        {
            timer -= Time.deltaTime;
            spawnRate -= Time.deltaTime;


            if (timer <= 0)
            {
                for (int i = wave * 2; i > 0; i--)
                {
                    Spawn();
                }
                wave++;
                timer = 10f + wave;
            }

            if (spawnRate <= 0)
            {
                Spawn();
                spawnRate = 1f;
            }
        }
    }

    private void Spawn()
    {
        int randNum = Random.Range(0, 10);
        Transform currentSpawn = spawnPoints[randNum];

        randNum = Random.Range(0, enemyPrefab.Count);

        //var enemy = Instantiate(enemyPrefab[randNum]).GetComponent<EnemyFollow>();
        var enemy = Instantiate(enemyPrefab[randNum], currentSpawn.position, currentSpawn.rotation).GetComponent<EnemyFollow>();
        enemy.SetTarget(player);

        Enemy e = enemy.GetComponent<Enemy>();
        e.SetWorldTime(worldTime);
    }
}
