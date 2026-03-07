using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    public Slider loadingSlider;
    public TextMeshProUGUI loadingProgressText;

    private AsyncOperation m_AsyncOperation;

    [SerializeField] private float minLoadingTime = 0.2f;
    [SerializeField] private float realLoadingLimit = 0.9f;
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
        
    }

    private IEnumerator LoadSceneProcess()
    {
        m_AsyncOperation = SceneLoader.Instance.LoadSceneAsync(SceneType.Lobby);
        if(m_AsyncOperation == null)
        {
            Logger.Log("Lobby async loading error", this);
            yield break;
        }

        m_AsyncOperation.allowSceneActivation = false;

        float loadingTimer = 0f;

        loadingSlider.value = 0f;

        while(!m_AsyncOperation.isDone)
        {
            yield return null;
            loadingTimer += Time.deltaTime;

            float targetValue = m_AsyncOperation.progress;

            if(m_AsyncOperation.progress >= realLoadingLimit)
            {
                targetValue = 1.0f;

            }

            float fakeProgress = Mathf.Clamp01(loadingTimer / minLoadingTime);

            float currentVisualValue = (m_AsyncOperation.progress < realLoadingLimit) ? Mathf.Min(m_AsyncOperation.progress, fakeProgress) : fakeProgress;
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, currentVisualValue, Time.deltaTime * 5f);
            
            loadingProgressText.text = $"{(int)(loadingSlider.value * 100)}%";

            if(loadingSlider.value >= 0.99f && m_AsyncOperation.progress >= realLoadingLimit)
            {
                loadingSlider.value = 1f;
                loadingProgressText.text = "100%";

                yield return new WaitForSeconds(minLoadingTime);

                m_AsyncOperation.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
