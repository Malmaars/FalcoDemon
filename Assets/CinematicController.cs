using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    public Texture2D[] framesInOrder;
    int currentFrameIndex = -1;

    public AudioClip[] soundsPerFrame;
    AudioSource oneShotSource;

    public RawImage rend;
    // Start is called before the first frame update
    void Start()
    {
        oneShotSource = GetComponent<AudioSource>();
        AdvanceCinematic();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AdvanceCinematic();
        }
    }

    void AdvanceCinematic()
    {
        if(currentFrameIndex >= framesInOrder.Length - 1)
        {
            //play game
            LevelManager.ChangeScene(2);
        }
        currentFrameIndex++;
        rend.texture = framesInOrder[currentFrameIndex];

        if(soundsPerFrame[currentFrameIndex] != null)
        {
            oneShotSource.PlayOneShot(soundsPerFrame[currentFrameIndex]);
        }
    }
}
