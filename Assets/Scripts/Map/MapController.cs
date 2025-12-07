using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    PlayerMovement pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist;
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;
    Vector3 playerLastPosition;

    void Start()
    {
        playerLastPosition = player.transform.position;
        currentChunk = terrainChunks[0];
        spawnedChunks.Add(currentChunk);
    }

    void Update()
    {
        UpdateCurrentChunk();
        CheckAllDirections(); 
        ChunkOptimzer();
    }

    void UpdateCurrentChunk()
    {
        foreach (GameObject chunk in spawnedChunks)
        {
            if (Vector3.Distance(player.transform.position, chunk.transform.position) < checkerRadius)
            {
                if (currentChunk != chunk)
                {
                    currentChunk = chunk;
                    Debug.Log("Player entered new chunk: " + currentChunk.name);
                }
                return;
            }
        }
    }

    void CheckAllDirections()
    {
        if (!currentChunk) return;

        string[] directions = new string[] {
            "Up", "Down", "Left", "Right",
            "Left Up", "Right Up", "Left Down", "Right Down"
        };

        foreach (string dir in directions)
        {
            ChunkCheckerDirection(dir);
        }
    }

    void ChunkCheckerDirection(string directionName)
    {
        Transform directionTransform = currentChunk.transform.Find(directionName);
        if (directionTransform == null) return;

        if (!Physics2D.OverlapCircle(directionTransform.position, checkerRadius, terrainMask))
        {
            SpawnChunk(directionTransform.position);
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimzer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0f) return;
        optimizerCooldown = optimizerCooldownDur;

        foreach (GameObject chunk in spawnedChunks)
        {
            chunk.SetActive(Vector3.Distance(player.transform.position, chunk.transform.position) <= maxOpDist);
        }
    }
}
