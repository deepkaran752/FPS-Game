using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.left);
    }

    private void OnTriggerEnter(Collider other)
    {
        FpsPlayerController fpsPlayer = other.GetComponent<FpsPlayerController>();
        if (fpsPlayer != null)
        {
            fpsPlayer.UpgradeHealth();
            Destroy(gameObject);
        }
    }
}
