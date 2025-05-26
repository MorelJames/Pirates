using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    private int numberOfTreasure, numberOfEnemy;
    private static SpawnerManager instance = null;
    public static SpawnerManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }
    private List<Spawner> treasureSpawner;
    private List<Spawner> enemySpawner;

    void Start()
    {
        treasureSpawner = new List<Spawner>();
        foreach (var item in GameObject.FindGameObjectsWithTag("treasureSpawner"))
        {
            treasureSpawner.Add(item.GetComponent<Spawner>());
        }
        enemySpawner = new List<Spawner>();
        foreach (var item in GameObject.FindGameObjectsWithTag("enemySpawner"))
        {
            enemySpawner.Add(item.GetComponent<Spawner>());
        }

        for (int i = 0; i < numberOfTreasure; i++)
            SpawnTreasure();

        for (int i = 0; i < numberOfEnemy; i++)
            SpawnEnemy();

    }

    public void SpawnTreasureWithDelay(int delay)
    {
        Invoke(nameof(SpawnTreasure), delay);
    }

    public void SpawnTreasure()
    {
        treasureSpawner[Random.Range(0, treasureSpawner.Count)].Spawn();
    }

    public void SpawnEnemy()
    {
        enemySpawner[Random.Range(0, enemySpawner.Count)].Spawn();
    }
}
