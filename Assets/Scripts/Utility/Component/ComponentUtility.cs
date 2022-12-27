using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public static class ComponentUtility
{
    public static GameScene gameScene
    {
        get
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().SingleOrDefault(o => o.name == "GameScene")
                .GetComponent<GameScene>();
        }
        private set { }
    }

    public static SceneTransition sceneTransition
    {
        get
        {
            return SceneManager.GetActiveScene().GetRootGameObjects().SingleOrDefault(o => o.name == "SceneTransition")
                .GetComponent<SceneTransition>();
        }
        private set { }
    }

    public static UI.PersistentTopBar topBar
    {
        get
        {
            // find the first PersistentTopBar component
            return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<UI.PersistentTopBar>())
                .FirstOrDefault();
        }
        private set { }
    }

    public static AudioManager audioManager
    {
        get
        {
            // find the first AudioManager component
            return SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<AudioManager>())
                .FirstOrDefault();
        }
        private set { }
    }

    public static void LoadScene(string sceneName)
    {
        SceneTransition sceneTransition = ComponentUtility.sceneTransition;
        if (sceneTransition)
        {
            sceneTransition.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

    }

    public static void RemoveChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in transform)
        {
            child.SetParent(null);
        }
    }

    public static void RemoveChildren(Component component)
    {
        foreach (Transform child in component.transform.Cast<Transform>().ToArray())
        {
            GameObject.Destroy(child.gameObject);
            child.SetParent(null);
        }
    }

    public static List<Transform> GetChildren(Transform component)
    {
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < component.transform.childCount; i++)
        {
            transforms.Add(component.transform.GetChild(i));
        }
        return transforms;
    }
}
