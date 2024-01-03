using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public FpsPlayerController FpsPlayerController;

    // Start is called before the first frame update
    void Start()
    {
        FpsPlayerController = FindObjectOfType<FpsPlayerController>();
    }

    public void RestartGame()
    {
        if (FpsPlayerController != null)
        {
            FpsPlayerController.RestartGame();
        }
    }
    //
    public void QuitGame()
    {
        Application.Quit();
    }
}
