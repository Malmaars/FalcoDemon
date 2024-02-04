using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoSequence : MonoBehaviour
{
    public GameObject logoWithText;
    public GameObject logoWithoutText;
    public ParticleSystem logoParticles;


    public float delay;
    bool once;

    public float wholeTimer;
    bool switchSceneBool;

    public float shakeDuration, shakeMagnitude;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(delay < 0 && !once)
        {
            logoWithoutText.SetActive(false);
            logoWithText.SetActive(true);
            StartCoroutine(CameraShaker.ShakeCamera(shakeDuration, shakeMagnitude));
            logoParticles.Play();
            once = true;
        }

        else
        {
            delay -= Time.deltaTime;
        }

        wholeTimer -= Time.deltaTime;

        if(wholeTimer <= 0 && !switchSceneBool)
        {
            switchSceneBool = true;
            StartCoroutine(EndLogoSequence(0.2f, 5));
        }
    }

    public IEnumerator EndLogoSequence(float duration, float fadeSpeed)
    {
        float elapsed = 0.0f;
        SpriteRenderer rend = logoWithText.GetComponent<SpriteRenderer>();
        while (rend.color.a > 0)
        {
            rend.color = new Color(rend.color.r, rend.color.b, rend.color.g, rend.color.a - Time.deltaTime * fadeSpeed);
            yield return null;
        }

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        LevelManager.ChangeScene(1);

        yield return null;
    }
}
