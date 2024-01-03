using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Public fields
    public float LifeDuration = 5f;

    // Private fields
    private IWeapon currentGun;
    private float lifeTimer;
    [Tooltip("If enabled the bullet destroys on impact")]
    private bool destroyOnImpact = true;
    [Tooltip("Minimum time after impact that the bullet is destroyed")]
    private float minDestroyTime = 5f;
    [Tooltip("Maximum time after impact that the bullet is destroyed")]
    private float maxDestroyTime = 10f;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Set the life timer of the bullet
        lifeTimer = LifeDuration;
        // Call the function to destroy the bullet
        StartCoroutine(DestroyAfter(lifeTimer));

        currentGun = FindObjectOfType<Gun>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If destroy on impact is false, start 
        //coroutine with random destroy timer
        if (!destroyOnImpact)
        {
            StartCoroutine(DestroyTimer());
        }
        //Otherwise, destroy bullet on impact
        else
        {
            this.gameObject.SetActive(false);
        }

        //if (collision.transform.gameObject.GetComponentInParent<EnemyMutant>() != null)
        //{
        //    EnemyMutant enemyMutant = collision.transform.gameObject.GetComponentInParent<EnemyMutant>();
        //    GameObject go = collision.transform.gameObject;
        //    enemyMutant.GotHit(go.GetComponent<Collider>(), go, currentGun.Damage);
        //}

        IDamageable enemy = collision.transform.gameObject.GetComponentInParent<IDamageable>();
        if (enemy != null)
        {
            enemy.TakeDamage(currentGun.Damage);
        }
    }

    private IEnumerator DestroyAfter(float timer)
    {
        // Wait for set amount of time
        yield return new WaitForSeconds(timer);
        // Deactivate bullet object
        this.gameObject.SetActive(false);
    }

    private IEnumerator DestroyTimer()
    {
        //Wait random time based on min and max values
        yield return new WaitForSeconds
            (Random.Range(minDestroyTime, maxDestroyTime));
        //Destroy bullet object
        this.gameObject.SetActive(false);
    }
}
