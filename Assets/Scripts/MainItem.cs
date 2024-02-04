using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainItem : MonoBehaviour
{
    public Rigidbody2D rb;
    public CircleCollider2D thisCol;

    public Vector2 currentPosition;

    public float distanceFromPlayer;

    Animator myAnim;

    AudioSource oneShotSource;
    public AudioClip explosion;
    public AudioClip[] richocets;
    public AudioClip ouchSound;
    public float minimumMagnitude;

    //sometimes there's a chance the ball says ouch
    GameObject[] ouchVariants;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<CircleCollider2D>();
        oneShotSource = GetComponent<AudioSource>();

        ouchVariants = new GameObject[] { Resources.Load("Hey") as GameObject, Resources.Load("Ouch") as GameObject, Resources.Load("Oof") as GameObject, Resources.Load("Owie") as GameObject, Resources.Load("ThatHurt") as GameObject };
        ouchSound = Resources.Load("ouchupdate") as AudioClip;

        myAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        currentPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        currentPosition = transform.position;   
    }

    public void DestroyBall()
    {
        //game over screen?
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.isKinematic = true;
        thisCol.enabled = false;

        oneShotSource.PlayOneShot(explosion);

        myAnim.SetTrigger("DestroyBall");
    }

    public void AttachToObject(Transform attachTo, Vector2 offset)
    {
        transform.SetParent(attachTo);
        transform.localPosition = offset;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.isKinematic = true;
        thisCol.enabled = false;


        myAnim.SetTrigger("Pickup");
    }

    public void Detach()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        thisCol.enabled = true;
    }

    public void GetThrown(Vector2 direction, float force)
    {
        Detach();
        rb.velocity = Vector2.zero;

        rb.velocity = direction * force;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.y, 2)) > minimumMagnitude)
        {
            oneShotSource.PlayOneShot(richocets[Random.Range(0, richocets.Length)]);
            myAnim.SetTrigger("Hit");

            //25% chance
            if(Random.Range(0,5) == 0)
            {
                Instantiate(ouchVariants[Random.Range(0, ouchVariants.Length)], new Vector2(transform.position.x, transform.position.y) + Vector2.up, new Quaternion(0, 0, 0, 0));
            }
        }
    }
}
