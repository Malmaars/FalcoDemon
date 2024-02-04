using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraShaker
{
    public static IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;

        float elapsed = 0.0f;

        Debug.Log("Entering Shaking");

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            Debug.Log("shaking camera");

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

}
