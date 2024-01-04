using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletRound;
    public float bulletSpeed;
    public float fireRate;

    bool shotFired;

    public void Shoot()
    {
        if (!shotFired)
        {
            var spawnedBullet = GameObject.Instantiate(bulletRound, transform.position, transform.rotation);
            var rb = spawnedBullet.GetComponent<Rigidbody>();

            rb.velocity = spawnedBullet.transform.forward * bulletSpeed;

            Destroy(spawnedBullet, 3f);

            shotFired = true;
            Invoke("ResetRound", fireRate);
        }
    }

    public void ResetRound()
    {
        shotFired = false;
    }
}
