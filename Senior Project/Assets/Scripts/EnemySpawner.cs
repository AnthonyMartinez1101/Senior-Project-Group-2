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

    public WorldClock worldClock;

    public Transform EnemyCollection;

    public bool InTutorial = false;


    private void Update()
    {
        if(worldClock == null)
        {
            Debug.LogWarning("EnemySpawner: WorldTime reference is not set.");
            return;
        }

        if (worldClock.IsNight() && !InTutorial)
        {
            timer -= Time.deltaTime;
            spawnRate -= Time.deltaTime;


            if (timer <= 0)
            {
                for (int i = wave * 2; i > 0; i--)
                {
                    Spawn(enemyPrefab[0]);
                }
                wave++;
                timer = 10f + wave;
            }

            if (spawnRate <= 0)
            {
                Spawn(enemyPrefab[0]);
                spawnRate = 1f;
            }
        }
    }

    public void Spawn(GameObject enemyObject)
    {
        int randNum = Random.Range(0, 10);
        Transform currentSpawn = spawnPoints[randNum];

        randNum = Random.Range(0, enemyPrefab.Count);

        //var enemy = Instantiate(enemyPrefab[randNum]).GetComponent<EnemyFollow>();
        var enemy = Instantiate(enemyObject, currentSpawn.position, currentSpawn.rotation, EnemyCollection).GetComponent<EnemyFollow>();
        enemy.SetTarget(player);

        Enemy e = enemy.GetComponent<Enemy>();
        e.SetWorldTime(worldClock);
    }
}
