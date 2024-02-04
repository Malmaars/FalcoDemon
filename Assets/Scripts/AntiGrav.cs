using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGrav : MonoBehaviour
{
    Dictionary<Rigidbody2D, float> rigidBodyGravityScales;

    AudioSource src;

    private void Start()
    {
        rigidBodyGravityScales = new Dictionary<Rigidbody2D, float>();

        src = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() && !src.isPlaying)
        {
            src.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rigidBodyGravityScales.TryAdd(rb, rb.gravityScale);
            rb.angularVelocity = 0;

            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            rb.gravityScale = rigidBodyGravityScales.GetValueOrDefault(rb);

            src.Stop();
        }
    }
}
