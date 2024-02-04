using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Finish : MonoBehaviour
{
    //when the player stand on this while holding the ball, they finish the level
    public LevelTimer timer;

    public LeaderBoardController leaderBoardController;

    public GameObject leaderBoardParent;
    public TextMeshProUGUI largeTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 && collision.transform.position.y > transform.position.y)
        {
            //check if the player is holding the item
            if (collision.gameObject.GetComponent<PlayerController>().IsPlayerHoldingItem())
            {
                //finish the level
                FinishLevel();
            }
        }
    }

    void FinishLevel()
    {
        Debug.Log("Finshing level");
        timer.StopTimer();

        //show time
        ShowTime();

        int TimeinMilliseconds = (int)(Math.Round(timer.ingameTimer, 3) * 1000);
        leaderBoardController.SetScore(TimeinMilliseconds);
        LevelManager.EndLevel();
        timer.transform.parent.gameObject.SetActive(false);
    }

    void ShowTime()
    {
        leaderBoardParent.SetActive(true);
        largeTime.gameObject.SetActive(true);
        largeTime.text = timer.ingameTimer.ToString("N3");
    }
}
