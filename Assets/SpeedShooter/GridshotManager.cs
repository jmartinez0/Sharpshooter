using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridshotManager : MonoBehaviour
{
    public GameObject targetPrefab;
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 1f;
    public float maxY = 3f;
    public float minZ = 5f;
    public float maxZ = 10f;
    public int numberOfTargets = 10;
    public float targetLifetime = 5f; // Targets last 5 seconds
    public float spawnDelay = 5f; // Time between target respawns

    void Start()
    {
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (true) // Infinite loop to keep spawning targets
        {
            for (int i = 0; i < numberOfTargets; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
                GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                Destroy(newTarget, targetLifetime);
            }
            yield return new WaitForSeconds(spawnDelay); // Wait before respawning targets
        }
    }
}
