using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//Definition of spawnable enemies
[System.Serializable]
public class SpawnableEnemy
{
    //Enemy prefab to spawn
    public GameObject enemyPrefab;

    //"Cost" of enemy
    public int spawnCost;

    //Probability of enemy spawning (1%-100%)
    [Range(1, 100)]
    public int spawnProbability;
}

[System.Serializable]
public class IntRange
{
    public int min;
    public int max;
    public int RandomRange()
    {
        return Random.Range(min, max + 1);
    }
}


public class EnemySpawner : MonoBehaviour
{
    [Header("Intro zombies (if intro night is enabled)")]
    [SerializeField] private List<GameObject> introZombies = new List<GameObject>();
    private bool introNightEnabled = false;

    [Header("Day Enemy Data")]
    [SerializeField] private List<GameObject> dayEnemies = new List<GameObject>();
    [SerializeField] private GameObject soils;
    public float daySpawnRate = 10f;
    private float daySpawnTimer = 0f;
    public IntRange daySpawnAmount;

    //All enemies spawnable during the respective season
    [Header("Set of Seasonal Enemies")]
    public List<SpawnableEnemy> springEnemies;
    public List<SpawnableEnemy> summerEnemies;
    public List<SpawnableEnemy> fallEnemies;
    public List<SpawnableEnemy> winterEnemies;
    public List<GameObject> bosses;
    private GameObject currentBoss;
    private bool bossSpawned = false;

    private List<SpawnableEnemy> currentEnemies = new List<SpawnableEnemy>();
    private List<GameObject> enemiesToSpawn = new List<GameObject>();


    [Header("Randomized Enemy Spawn Points")]
    public List<Transform> spawnPoints;
    public Transform bossSpawnPoint;


    [Header("Enemy References")]
    public Transform player; //For enemies to target
    public WorldClock worldClock; //For enemies to know when to burn
    public Transform EnemyCollection; //For organization of spawned enemies in the hierarchy
    

    [Header("Spawn System")]
    public float consistentSpawnRate = 5f; //Constant enemy spawn rate at all times during night

    public float waveIntervals = 15f;
    public float waveSpawnRate = 0.5f;


    private float consistenSpawnTimer = 0f;

    private float waveTimer = 0f;
    private float waveSpawnTimer = 0f;

    private int waveValue = 0;
    private int wave = 0;


    public bool InTutorial = false; //Tutorial mode, disabling spawning

    private void Update()
    {
        if (!CheckReference()) return;
        if(InTutorial) return;

        if (worldClock.IsIntroNight())
        {
            CheckStarterZombies();
            return;
        }

        if (worldClock.CurrentPhase == DayPhase.Night)
        {
            SpawnEnemies();

            if (currentBoss == null && worldClock.CanSpawnBoss() && !bossSpawned)
            {
                int bossInd = (int)worldClock.CurrentSeason;
                currentBoss = Instantiate(bosses[bossInd], bossSpawnPoint.position, bossSpawnPoint.rotation, EnemyCollection);
                var bossScript = currentBoss.GetComponent<BossScript>();
                if (bossScript != null) bossScript.player = player;
                var bossFollow = currentBoss.GetComponent<EnemyFollow>();
                if (bossFollow != null) bossFollow.SetTarget(player);   

                worldClock.BossSpawned();
                bossSpawned = true;

                MusicScript.Instance.PlayBossMusic();

                waveTimer = 0f;
                enemiesToSpawn.Clear();

                StartCoroutine(CheckBoss());
            }
        }
        else
        {
            SpawnDayEnemies();
        }
    }

    private void SpawnDayEnemies()
    {
        daySpawnTimer -= Time.deltaTime;
        if (daySpawnTimer <= 0f)
        {
            StartCoroutine(SpawnDays());
            daySpawnTimer = daySpawnRate;
        }
    }

    IEnumerator SpawnDays()
    {
        GameObject randomDayEnemy = dayEnemies[Random.Range(0, dayEnemies.Count)];
        int spawnAmount = daySpawnAmount.RandomRange();
        for (int i = 0; i < spawnAmount; i++)
        {
            SpawnAndSetDayEnemy(randomDayEnemy);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnAndSetDayEnemy(GameObject dayEnemyToSpawn)
    {
        int randSpawn = Random.Range(0, spawnPoints.Count);
        Transform currentSpawn = spawnPoints[randSpawn];

        //Spawn enemy at spawn point into EnemyCollection and set soil target
        var enemy = Instantiate(dayEnemyToSpawn, currentSpawn.position, currentSpawn.rotation, EnemyCollection).GetComponent<DayEnemy>();
        if (enemy && soils)
        {
            enemy.GiveSoil(soils);
            enemy.GiveClock(worldClock);
        }
    }

    private void CheckStarterZombies()
    {
        if (introNightEnabled) return;

        //removes null zombies from list
        introZombies.RemoveAll(zombie => zombie == null);

        if (introZombies.Count == 0)
        {
            introNightEnabled = true;
            worldClock.CompleteIntroNight();
        }
    }

    IEnumerator CheckBoss()
    {
        yield return new WaitUntil(() => currentBoss == null);
        worldClock.ResumeTimer();
    }

    private void SpawnEnemies()
    {


        consistenSpawnTimer -= Time.deltaTime;
        waveTimer -= Time.deltaTime;

        //Constant spawn functionality
        if (consistenSpawnTimer <= 0f)
        {
            SpawnableEnemy spawnEnemy = GetRandomEnemy();
            Spawn(spawnEnemy.enemyPrefab);
            consistenSpawnTimer = consistentSpawnRate + spawnEnemy.spawnCost;
        }

        //Wave spawn functionality (only spawn waves if there is no boss)
        if (waveTimer <= 0f && currentBoss == null)
        {
            waveValue = (wave * 5);
            wave++;
            waveTimer = waveIntervals;
            GenerateWaveEnemies();
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        while (enemiesToSpawn.Count > 0)
        {
            Spawn(enemiesToSpawn[0]);
            enemiesToSpawn.RemoveAt(0);
            yield return new WaitForSeconds(waveSpawnRate);
        }
    }

    //Spawns enemy at random point
    public void Spawn(GameObject enemyToSpawn)
    {
        //Get a random spawn point
        int randSpawn = Random.Range(0, spawnPoints.Count);
        Transform currentSpawn = spawnPoints[randSpawn];

        //Spawn enemy at spawn point into EnemyCollection and set player as target
        var enemy = Instantiate(enemyToSpawn, currentSpawn.position, currentSpawn.rotation, EnemyCollection).GetComponent<EnemyFollow>();
        enemy.SetTarget(player);

        //Give enemy the worldClock
        Enemy e = enemy.GetComponent<Enemy>();
        e.SetWorldTime(worldClock);
    }

    private SpawnableEnemy GetRandomEnemy()
    {
        int safety = 1000;
        int safetyCount = 0;

        while (safetyCount++ < safety)
        {
            int randInd = Random.Range(0, currentEnemies.Count);
            SpawnableEnemy enemy = currentEnemies[randInd];

            int randProb = Random.Range(1, 101);
            if (randProb <= enemy.spawnProbability) return enemy;
        }
        return currentEnemies[0];
    }



    private void GenerateWaveEnemies()
    {
        //Clear list of enemies
        enemiesToSpawn.Clear();

        //Safe guards in case loop runs too many times
        int safety = 1000;
        int safetyCount = 0;

        //Generate enemies with given waveValue
        while (waveValue > 0 && safetyCount++ < safety)
        {
            //Get a random enemy from current season's enemies
            int randInd = Random.Range(0, currentEnemies.Count);
            SpawnableEnemy enemy = currentEnemies[randInd];

            //If enemy costs too much, skip and try again
            if(enemy.spawnCost > waveValue) continue;

            //If the random probability fails to choose enemy, skip and try again
            int randProb = Random.Range(1, 101);
            if(randProb > enemy.spawnProbability) continue;

            //If enemy is chosen, add to list and subtract cost
            enemiesToSpawn.Add(enemy.enemyPrefab);
            waveValue -= enemy.spawnCost;
        }
    }

    //Function to be called when season changes
    public void EnemySeasonChange()
    {
        ResetSpawner();

        switch (worldClock.CurrentSeason)
        {
            case SeasonPhase.Spring:
                currentEnemies = springEnemies;
                break;
            case SeasonPhase.Summer:
                currentEnemies = summerEnemies;
                break;
            case SeasonPhase.Fall:
                currentEnemies = fallEnemies;
                break;
            case SeasonPhase.Winter:
                currentEnemies = winterEnemies;
                break;
        }
    }

    private void ResetSpawner() 
    {
        wave = 0;
        consistenSpawnTimer = 0f;
        waveTimer = waveIntervals;
        waveSpawnTimer = waveSpawnRate;
        bossSpawned = false;
    }

    private bool CheckReference()
    {
        if (player == null || worldClock == null || EnemyCollection == null)
        {
            Debug.LogWarning("EnemySpawner: Reference is not set.");
            return false;
        }
        return true;
    }
}
