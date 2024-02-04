using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    SpriteRenderer rend;
    public float fadeSpeed;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        rend.color = new Color(rend.color.r, rend.color.b, rend.color.g, rend.color.a - fadeSpeed * Time.deltaTime);
    }
}
