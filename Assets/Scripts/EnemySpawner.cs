using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static List<Transform> spawnPoints = new List<Transform>();
    public GameObject enemyPrefab, enemyContainer;
    public float enemyBurstCount = 4, spawnTime = 1;

    Transform oldLocation;

    Transform location;
    float updateTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
            spawnPoints.Add(child);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time > updateTime)
        {
            updateTime = Time.time + spawnTime;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if(enemyContainer.transform.childCount < enemyBurstCount)
        {
            location = spawnPoints[Random.Range(0, transform.childCount)];
            while(location == oldLocation)
            {
                location = spawnPoints[Random.Range(0, transform.childCount)];
            }
            oldLocation = location;

            var enemyInstance = Instantiate(enemyPrefab, location.position, location.rotation);
            enemyInstance.transform.SetParent(enemyContainer.transform);
        }
    }
}
