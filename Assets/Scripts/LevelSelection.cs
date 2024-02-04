using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    public void LoadLevel(int levelOrScene)
    {
        LevelManager.ChangeScene(levelOrScene);
    }
}
