using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fallColor
{
    blue,
    red
}
public class FallThrough : MonoBehaviour
{
    public fallColor thisColor;

    BoxCollider2D thisCol;
    
    public GameObject solidPlatform, FallPlatform;

    private void Start()
    {
        thisCol = GetComponent<BoxCollider2D>();
    }
    public void MakeSolid()
    {
        solidPlatform.SetActive(true);
        FallPlatform.SetActive(false);
        thisCol.enabled = true;
    }

    public void MakeFallThrough()
    {
        solidPlatform.SetActive(false);
        FallPlatform.SetActive(true);
        thisCol.enabled = false;
    }
}
