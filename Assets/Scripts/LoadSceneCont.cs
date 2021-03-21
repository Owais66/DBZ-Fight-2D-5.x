using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneCont : MonoBehaviour
{
    [SerializeField] Button InvisTouch;
    [SerializeField] Image LoadingBar;
    [SerializeField] Text LoadingText;

    bool InvisClicked = false;

    public void Load(string SceneName)
    {   
        InvisTouch.enabled = false;
        InvisTouch.onClick.AddListener(()=>{InvisClicked = true; });
        StartCoroutine(AsynchronousLoad(SceneName));
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        LoadingBar.fillAmount = 0;

        LoadingText.text = "Loading...";
        Color FadeCol = LoadingText.color;
        bool FadeTrig = false;

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            LoadingBar.fillAmount = progress;

            // Loading completed
            if (ao.progress == 0.9f)
            {   
                InvisTouch.enabled = true;
                if (!FadeTrig)
                {
                    FadeCol.a -= 0.01f;
                    if(FadeCol.a <= 0.2f) FadeTrig = true;
                }else if(FadeTrig){
                    FadeCol.a += 0.01f;
                    if(FadeCol.a == 1) FadeTrig = false;
                }
                
                LoadingText.color = FadeCol;

                LoadingText.text = "Click to Continue";
                if (InvisClicked)
                    ao.allowSceneActivation = true;
            }

            yield return null;
        }
        yield return null;
        Destroy(gameObject);
    }
}