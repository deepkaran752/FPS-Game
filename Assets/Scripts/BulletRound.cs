using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRound : MonoBehaviour
{
    public ParticleSystem hitEffect;
    public int damage;
    public int targetObjectLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == targetObjectLayer)
        {
            collision.gameObject.GetComponent<Health>().Damage(damage);
        }

        hitEffect.gameObject.SetActive(true);
        hitEffect.transform.SetParent(null);
        Destroy(gameObject);
    }
}
