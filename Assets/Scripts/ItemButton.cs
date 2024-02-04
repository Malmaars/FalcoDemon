using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum buttonType
{
    switchColors,
    fallPlatforms,
    movingDoor
}
public class ItemButton : MonoBehaviour
{
    public buttonType[] thisType;
    SpriteRenderer rend;

    public FallThroughManager fallManager;

    public Door[] thisDoor;

    Animator animator;

    public AudioClip clickSound;
    AudioSource src;
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        src = GetComponent<AudioSource>();
    }

    private void Update()
    {
        foreach (buttonType butType in thisType)
        {
            if(butType == buttonType.fallPlatforms)
            {
                animator.SetBool("Red", ColorStatic.red);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            //item hit the button. activate
            ActivateButton();
        }
    }

    void ActivateButton()
    {
        src.PlayOneShot(clickSound);
        foreach (buttonType butType in thisType)
        {
            switch (butType)
            {
                case buttonType.fallPlatforms:
                    fallManager.SwitchFalls();
                    break;

                case buttonType.movingDoor:
                    foreach (Door dr in thisDoor)
                    {
                        dr.ChangeDoorState();
                    }
                    break;
            }
        }

        animator.SetTrigger("Click");
    }
}
