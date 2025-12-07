using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //list of enemy groups in the wave
        public int waveQuota;   //number of enemies in the wave
        public float spawnInterval; //interval of spawn
        public int spawnCount;  //number of enemies already spawned in te wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab; //enemy prefab
        public string enemyName; //enemy name
        public int enemyCount; //number of enemies in the group
        public int spawnCount; //number of enemies already spawned in the group
    }

    public List<Wave> waves; //list of waves
    public int currentWaveCount;   //index of the current wave

    [Header("Spawner Attributes")]
    float spawnTimer; //timer for the spawn interval
    public int enemiesAlive;    //number of enemies alive in the scene
    public int maxEnemiesAllowed;   //maximum number of enemies allowed in the scene
    public bool maxEnemiesReached = false;  //check if the maximum number of enemies is reached
    public float waveInterval; //interval between waves
    bool isWaveActive = false; //check if the wave is active

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; //list of spawn positions

    Transform player;
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform; //find the player in the scene
        CalculateWaveQuota(); 
    }


    void Update()
{
    spawnTimer += Time.deltaTime;

    if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount >= waves[currentWaveCount].waveQuota && !isWaveActive)
    {
        StartCoroutine(BeginNextWave());
    }

    if (spawnTimer >= waves[currentWaveCount].spawnInterval)
    {
        spawnTimer = 0f;
        SpawnEnemies();
    }
}

    IEnumerator BeginNextWave()
    {
        isWaveActive = true; //set the wave active flag to true

        yield return new WaitForSeconds(waveInterval); //wait for the wave interval
        if(currentWaveCount < waves.Count - 1) //check if there are more waves
        {
            isWaveActive = false; //set the wave active flag to false
            currentWaveCount++; //increment the wave count
            CalculateWaveQuota(); //calculate the wave quota for the next wave
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota; 
        Debug.LogWarning(currentWaveQuota);
    }

    void SpawnEnemies()
    {   //check if the minimum number of enemies in the wave is reached
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached) //check if the wave quota is not reached and if the maximum number of enemies is not reached
        {   //spawn each type of enemy in the wave
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {

                    //spawn the enemy outside the player view
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);


                    enemyGroup.spawnCount++; //increment the spawn count of the enemy group
                    waves[currentWaveCount].spawnCount++; //increment the spawn count of the wave
                    enemiesAlive++; //increment the number of enemies alive in the scene
                    
                    if(enemiesAlive >= maxEnemiesAllowed) //check if the maximum number of enemies is reached
                    {
                        maxEnemiesReached = true; //set the maximum enemies reached flag to true
                        return; //exit the function
                    }
                }
            }
        }
    }
    public void OnEnemyKilled()
    {
        enemiesAlive--; //decrement the number of enemies alive in the scene

        //reset the maxEnemiesReached flag if the number of enemies alive is less than the maximum number of enemies allowed
        if(enemiesAlive < maxEnemiesAllowed) //check if the maximum number of enemies is not reached
        {
            maxEnemiesReached = false; //set the maximum enemies reached flag to false
        }
    }

}
