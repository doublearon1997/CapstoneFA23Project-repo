using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionObject : MonoBehaviour
{
    public GameObject LoadingSceneManagerInstance, TravelDialogPanel, DungeonDialogPanel;
    public LevelManager levelManager;
    public int SpecifiedSceneToLoad;
    public bool IsOverWorldTransitionObject, IsDungeonTransitionObject, SavePosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (IsOverWorldTransitionObject)
                TravelDialogPanel.SetActive(true);
            else if (IsDungeonTransitionObject)
                DungeonDialogPanel.SetActive(true);
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
            if (DungeonDialogPanel != null)
                DungeonDialogPanel.SetActive(false);
        }
    }

    public void LoadOverworld()
    {
        LoadingSceneManager.sceneToLoad = 2;
        SceneManager.LoadScene(1);
    }

    public void LoadSpecifiedScene()
    {
        if (SavePosition)
        {
            levelManager.UpdatePlayerPosition();
            LevelManager.InDungeon = 1;
        }
        LoadingSceneManager.sceneToLoad = SpecifiedSceneToLoad;
        SceneManager.LoadScene(1);
    }
}