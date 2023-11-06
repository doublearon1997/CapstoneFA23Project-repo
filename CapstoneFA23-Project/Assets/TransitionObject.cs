using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionObject : MonoBehaviour
{
    public GameObject LoadingSceneManagerInstance;
    public bool playerIsClose;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            LoadingSceneManager.sceneToLoad = 2;
            SceneManager.LoadScene(1);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
}