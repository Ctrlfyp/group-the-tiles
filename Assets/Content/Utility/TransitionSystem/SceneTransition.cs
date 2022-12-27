using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SceneTransition : MonoBehaviour
{

    public Animator transition;
    public GameObject transitionImage;
    public GameObject loadingText;
    public float transitionTime = 1.5f;
    public string fileName = "transitionScreen";
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    void Start()
    {
        StartCoroutine(HandleFirstLoad());
    }

    IEnumerator HandleFirstLoad()
    {
        bool isDoTransition = PlayerPrefs.GetInt("isSceneTransition") == 1 ? true : false;
        if (isDoTransition)
        {
            PlayerPrefs.SetInt("isSceneTransition", 0);
            try
            {
                Image imageComponent = transitionImage.GetComponent<Image>();
                Texture2D transitionTexture = null;
                byte[] fileData;

                fileData = File.ReadAllBytes(Application.persistentDataPath + "/" + fileName + ".png");
                transitionTexture = new Texture2D(2, 2);
                transitionTexture.LoadImage(fileData);
                imageComponent.sprite = Sprite.Create(transitionTexture, new Rect(0, 0, transitionTexture.width, transitionTexture.height), new Vector2(0.5f, 0.5f));
                transitionImage.SetActive(true);
                transition.SetTrigger("Start");
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            yield return new WaitForSeconds(transitionTime);
        }
        else
        {
            transitionImage.SetActive(false);
            yield return new WaitForSeconds(0f);
        }
    }

    IEnumerator CaptureAndTransition(string sceneName)
    {
        yield return frameEnd;

        try
        {
            Texture2D fakeScreen = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false, false);
            fakeScreen.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            fakeScreen.Apply();

            // Save the screenshot somewhere
            byte[] bytes = fakeScreen.EncodeToPNG();
            Destroy(fakeScreen);

            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + ".png", bytes);
            PlayerPrefs.SetInt("isSceneTransition", 1);
        }
        catch (System.Exception e)
        {
            PlayerPrefs.SetInt("isSceneTransition", 0);
            Debug.Log(e);
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LoadScene(string sceneName)
    {
        loadingText.SetActive(true);
        StartCoroutine(CaptureAndTransition(sceneName));
    }

}
