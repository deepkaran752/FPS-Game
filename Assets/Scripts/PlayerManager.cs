using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TechXR.Core.Sense;

public class PlayerManager : MonoBehaviour
{
    public Shooting shoot;
    public GameObject laserPointer, MainMenu;
    public SenseController senseController;
    private Health playerHealth;
    public GameObject GameStartAssets;

    bool inGame;
    
    public Transform startPosition;
    // Start is called before the first frame update
    void Start()
    {
        GameStartAssets.SetActive(false);
        playerHealth = GetComponent<Health>();
        startPosition.position = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (SenseInput.GetButtonDown(ButtonName.L) && inGame)
        {
            shoot.Shoot();
        }

        if (playerHealth.health <= 0)
            GameOver();
    }

    public void GameStart()
    {
        GameStartAssets.SetActive(true);
        inGame = true;
        MainMenu.SetActive(false);
        senseController.ToggleJoystickMovement(true);
        shoot.gameObject.SetActive(true);

        laserPointer.SetActive(false);
    }

    public void GameOver()
    {
        GameStartAssets.SetActive(false);
        inGame = false;
        MainMenu.SetActive(true);
        senseController.ToggleJoystickMovement(false);
        shoot.gameObject.SetActive(false);

        laserPointer.SetActive(true);
        laserPointer.GetComponent<LaserPointer>().ButtonState = false;
        
        playerHealth.ResetHealth();

        transform.position = startPosition.position;
    }
}
