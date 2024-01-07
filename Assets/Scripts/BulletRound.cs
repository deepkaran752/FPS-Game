using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRound : MonoBehaviour
{
    public ParticleSystem hitEffect;

    private void OnCollisionEnter(Collision collision)
    {
        hitEffect.gameObject.SetActive(true);
        hitEffect.transform.SetParent(null);
        Destroy(gameObject);
    }
}
