using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float ingameTimer = 0f;

    public bool timerRunning;

    public TextMeshProUGUI timerVisual;

    private void Start()
    {

    }

    private void Update()
    {
        if (timerRunning)
            UpdateTimer();
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void UpdateTimer()
    {
        if (Time.timeScale > 0)
        {
            ingameTimer += Time.deltaTime / Time.timeScale;
        }

        timerVisual.text = ingameTimer.ToString("N3");
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
}
