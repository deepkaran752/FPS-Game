using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;

public class PlayerManager : MonoBehaviour
{
    public Shooting shoot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SenseInput.GetButtonDown(ButtonName.L))
        {
            shoot.Shoot();
        }
    }
}
