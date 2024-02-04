using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBallPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            //kill the ball
            MainItem item = collision.gameObject.GetComponent<MainItem>();

            item.DestroyBall();
        }
    }
}
