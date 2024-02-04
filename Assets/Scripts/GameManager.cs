using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void RestartLevel()
    {
        LevelManager.RestartCurrentLevel();
    }

    public void NextLevel()
    {
        if (LevelManager.currentScene < 9)
            LevelManager.ChangeScene(LevelManager.currentScene + 1);
        else
        {
            BackToLevelSelect();
        }
    }

    public void BackToLevelSelect()
    {
        LevelManager.ChangeScene(2);
    }
}
