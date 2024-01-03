using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a Singleton Class that helps to optimize the game performance
public class ObjectPoolingManager : MonoBehaviour
{
    //Static fields
    public static ObjectPoolingManager Instance => instance;
    
    private static ObjectPoolingManager instance;

    // Public fields
    public GameObject BulletPrefab;
    public int BulletAmount = 20;

    // Private fields
    private List<GameObject> _bullets;

    private void Awake()
    {
        instance = this;

        // Preload Bullets
        _bullets = new List<GameObject>();

        // Instantiate and Store the bullets
        for (int i=0; i<BulletAmount; i++)
        {
            GameObject prefabInstance = Instantiate(BulletPrefab);
            prefabInstance.transform.SetParent(transform);
            prefabInstance.SetActive(false);

            _bullets.Add(prefabInstance);
        }
    }

    public GameObject GetBullet()
    {
        // Check if any bullet is available or active in the list return that bullet
        foreach(GameObject bullet in _bullets)
        {
            if(!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        // If any bullet is not available in the list instantiate a new bullet and return it
        GameObject prefabInstance = Instantiate(BulletPrefab);
        prefabInstance.transform.SetParent(transform);
        // Add it to the list for further "re-use"
        _bullets.Add(prefabInstance);

        return prefabInstance;
    }
}
