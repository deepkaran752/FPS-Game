using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    public float health = 100;
    public Image healthImage;
    public TMP_Text healthText;

    public void Damage(int damage)
    {
        health -= damage;
        if(healthImage)
            healthImage.fillAmount = health / 100;

        if(healthText)
            healthText.text = health.ToString("F0");

    }
}
