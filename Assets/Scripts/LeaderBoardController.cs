using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LeaderBoardController : MonoBehaviour
{
    public TMP_InputField MemberID;
    int scoreInMilliseconds;

    public string boardKey;

    public int maxScores;
    public TextMeshProUGUI[] EntryNames;
    public TextMeshProUGUI[] Entries;

    private void Start()
    {
        ulong newSession = (ulong)Random.Range(0, ulong.MaxValue);
        LootLockerSDKManager.StartGuestSession(newSession.ToString(), (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succes");
            }
            else
            {
                Debug.Log("Failed");
            }
        });

        StartCoroutine(ShowScoresAfterTimer(3f));
        StartCoroutine(CheckForDuplicatePlayerID(1f, newSession));

    }

    public void SetScore(int _score)
    {
        Debug.Log("setting score to " + _score);
        scoreInMilliseconds = _score;
        Debug.Log("Score is " + scoreInMilliseconds);
    }

    public void SubmitScoreToLeaderBoard()
    {
        LootLockerSDKManager.SetPlayerName(MemberID.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name");
            }
            else
            {
                Debug.Log("Error setting player name");
            }
        });

        LootLockerSDKManager.SubmitScore("1111", scoreInMilliseconds, boardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succes");
            }
            else
            {
                Debug.Log("Failed");
            }

        });

        StartCoroutine(ShowScoresAfterTimer(1f));
    }

    public void ShowScores()
    {
        LootLockerSDKManager.GetScoreList(boardKey, maxScores, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;

                for(int i = 0; i < scores.Length; i++)
                {
                    EntryNames[i].text = (scores[i].player.name);

                    float scoreInSeconds = (float)scores[i].score / 1000;
                    Entries[i].text = (scores[i].rank + ".    " + scoreInSeconds + "s");
                }

                if(scores.Length < maxScores)
                {
                    for(int i = scores.Length; i < maxScores; i++)
                    {
                        EntryNames[i].text = "...";
                        Entries[i].text = (i + 1).ToString() + ".    ";
                    }
                }
            }

            else
            {
                Debug.Log("Failed");
            }
        });
    }

    public IEnumerator SubmitScore(float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        LootLockerSDKManager.SetPlayerName(MemberID.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name");
            }
            else
            {
                Debug.Log("Error setting player name");
            }
        });

        LootLockerSDKManager.SubmitScore("1111", scoreInMilliseconds, boardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succes");
            }
            else
            {
                Debug.Log("Failed");
            }

        });

        yield return null;
    }

    public IEnumerator ShowScoresAfterTimer(float duration)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        ShowScores();

        yield return null;
    }

    public IEnumerator CheckForDuplicatePlayerID(float duration, ulong _newSession)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        LootLockerSDKManager.LookupPlayerNamesByPlayerIds(new ulong[1] { _newSession }, (response) =>
        {
            if (response.success)
            {
                LootLockerSDKManager.EndSession((response) =>
                {
                    if (response.success)
                    {
                        Debug.Log("Succes");
                    }
                    else
                    {
                        Debug.Log("Failed");
                    }
                });

                _newSession = (ulong)Random.Range(0, 20000);
                LootLockerSDKManager.StartGuestSession(_newSession.ToString(), (response) =>
                {
                    if (response.success)
                    {
                        Debug.Log("Succes");
                    }
                    else
                    {
                        Debug.Log("Failed");
                    }
                });
            }
            else
            {
                Debug.Log("Failed");
            }
        });

        yield return null;
    }
}
