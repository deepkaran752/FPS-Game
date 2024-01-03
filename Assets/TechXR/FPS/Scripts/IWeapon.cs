using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    // Fields
    float Damage { get;}

    // Methods
    void Shoot();
    IEnumerator Reload();
}
