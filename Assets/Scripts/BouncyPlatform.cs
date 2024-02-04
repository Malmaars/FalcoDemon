using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    public float ballBouncinessMultiplier;
    public float velocityClamp;

    public bool totheSide;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 && collision.transform.position.y > transform.position.y)
        {
            //check if the player is holding the item
            Debug.Log("Please make the player jump");
            collision.gameObject.GetComponent<PlayerController>().Jump(1.5f);
        }

        if (collision.gameObject.layer == 6)
        {
            if (totheSide)
            {
                Rigidbody2D rb1 = collision.gameObject.GetComponent<Rigidbody2D>();

                rb1.velocity *= ballBouncinessMultiplier;
                rb1.velocity = new Vector2(rb1.velocity.x, Mathf.Clamp(rb1.velocity.y, -10, 10));
                rb1.velocity = Vector2.ClampMagnitude(rb1.velocity, velocityClamp);
                return;
            }
            //the ball touches the bouncy platform
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            rb.velocity *= ballBouncinessMultiplier;
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, velocityClamp);
        }
    }
}
