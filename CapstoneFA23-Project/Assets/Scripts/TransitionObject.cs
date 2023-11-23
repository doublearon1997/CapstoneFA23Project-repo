using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionObject : MonoBehaviour
{
    public GameObject LoadingSceneManagerInstance, TravelDialogPanel;
    public int SpecifiedSceneToLoad;
    public bool IsOverWorldTransitionObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (IsOverWorldTransitionObject)
            {
                TravelDialogPanel.SetActive(true);
            }
            else
            {
                LoadSpecifiedScene();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(TravelDialogPanel!=null)
                TravelDialogPanel.SetActive(false);
        }
    }

    public void LoadOverworld()
    {
        LoadingSceneManager.sceneToLoad = 2;
        SceneManager.LoadScene(1);
    }

    public void LoadSpecifiedScene()
    {
        LoadingSceneManager.sceneToLoad = SpecifiedSceneToLoad;
        SceneManager.LoadScene(1);
    }
}