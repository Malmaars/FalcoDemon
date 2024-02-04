using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DampenImpact : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            //the ball touches the bouncy platform
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity *= 0.1f;
        }
    }
}
