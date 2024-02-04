using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{
    public PlayerController playerRef;

    public Transform loadLocation;
    public float timeUntilShoot;
    public float cannonPower;

    public Transform cannonDirection;

    public AudioClip cannonShotSound;
    AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MainItem>() != null)
        {
            //put the item in the cannon
            LoadCannon(collision.GetComponent<MainItem>());
        }
    }

    void LoadCannon(MainItem toLoad)
    {
        playerRef.ReleaseItem();

        toLoad.AttachToObject(loadLocation, Vector2.zero);
        StartCoroutine(WaitToShoot(timeUntilShoot, toLoad));
    }

    IEnumerator WaitToShoot(float delay, MainItem toShoot)
    {
        float elapsed = 0.0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        toShoot.GetThrown(cannonDirection.up, cannonPower);
        src.PlayOneShot(cannonShotSound);
    }
}
