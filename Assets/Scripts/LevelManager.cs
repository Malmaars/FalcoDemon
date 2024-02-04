using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    public static int currentScene;

    public static bool levelEnd;

    public static void StartLevel()
    {
        levelEnd = false;
    }
    public static void ChangeScene(int _scene)
    {
        Cursor.visible = true;
        currentScene = _scene;
        levelEnd = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(_scene);
    }

    public static void RestartCurrentLevel()
    {
        levelEnd = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(currentScene);
    }

    public static void EndLevel()
    {
        Cursor.visible = true;
        levelEnd = true;
    }
}
