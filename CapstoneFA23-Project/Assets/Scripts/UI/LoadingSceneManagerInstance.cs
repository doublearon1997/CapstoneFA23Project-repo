using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManagerInstance : MonoBehaviour
{
    public void loadScene(int sceneID)
    {
        LoadingSceneManager.sceneToLoad = sceneID;
        SceneManager.LoadScene(1);
    }
}