using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;
using System;

public class WeaponManager : MonoBehaviour
{
    public List<GameObject> WeaponPrefabs;
    //
    private GameObject m_CurrentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        WeaponPrefabs = PopulateGuns();
        if (WeaponPrefabs.Count > 0)
        {
            m_CurrentWeapon = WeaponPrefabs[0];
        }
        else
        {
            Debug.Log("No Weapon Prefabs Available..!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SenseInput.GetButtonDown(ButtonName.L))
        {
            if (WeaponPrefabs.Count > 0)
            {
                ChangeWeapon(WeaponPrefabs);
            }
            else
            {
                Debug.Log("No Weapon Prefabs Available..!!");
            }
        }
    }

    private void ChangeWeapon(List<GameObject> WeaponPrefabs)
    {
        for (int i = 0; i < WeaponPrefabs.Count; i++)
        {
            if (WeaponPrefabs[i] == m_CurrentWeapon)
            {
                m_CurrentWeapon.SetActive(false);
                if (i + 1 < WeaponPrefabs.Count)
                {
                    m_CurrentWeapon = WeaponPrefabs[i + 1];
                    m_CurrentWeapon.SetActive(true);
                    break;
                }
                else
                {
                    m_CurrentWeapon = WeaponPrefabs[0];
                    m_CurrentWeapon.SetActive(true);
                    break;
                }
            }
        }
    }

    private List<GameObject> PopulateGuns()
    {
        List<GameObject> WeaponPrefabs = new List<GameObject>();

        GameObject gunContainer = GameObject.FindWithTag("GunContainer");

        if (!gunContainer)
        {
            Debug.Log("No GunContainer found..!");
        }
        else
        {
            foreach (Transform t in gunContainer.transform)
                WeaponPrefabs.Add(t.gameObject);
        }

        return WeaponPrefabs;
    }
}