using UnityEngine;

public class CameraShake : SingletonBehaviour<CameraShake>
{
    private Vector3 originalPos;

    public System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        originalPos = transform.localPosition;
        float elapsed = 0.0f;

        float frequency = 60f;

        while (elapsed < duration)
        {
            // 무작위 좌표 생성
            float x = originalPos.x + Mathf.Sin(elapsed * frequency) * magnitude;
            float y = originalPos.y + Mathf.Cos(elapsed * frequency) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들림이 끝나면 정확히 원래 위치로 복구
        transform.localPosition = originalPos;
    }
}