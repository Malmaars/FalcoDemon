using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoTextAnimation : MonoBehaviour
{
    public SpriteRenderer thisRen;
    public float delay;
    bool once;

    public float fallSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (delay < 0 && !once)
        {
            thisRen.enabled = true;

            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2, 2, 2), fallSpeed * Time.deltaTime);

            if(transform.localScale.x < 2.1f)
            {
                thisRen.enabled = false;
                once = true;
            }
        }

        else
        {
            delay -= Time.deltaTime;
        }
    }
}
