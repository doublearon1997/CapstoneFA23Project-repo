using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class UIBehavior : MonoBehaviour
{   //Island IDs: TutorialIsland - 4; CoveIsland - 5; GhostShipIsland - 6; TowerIsland - 7
    //game objects
    public GameObject LoadingScreen;
    public GameObject Crosshair;
    public GameObject TutorialIslandStatic;
    public GameObject TutorialIslandLocked;
    public GameObject TutorialIslandPanel;
    public GameObject CoveIslandStatic;
    public GameObject CoveIslandLocked;
    public GameObject CoveIslandPanel;
    public GameObject GhostShipIslandStatic;
    public GameObject GhostShipIslandLocked;
    public GameObject GhostShipIslandPanel;
    public GameObject FortressIslandStatic;
    public GameObject FortressIslandLocked;
    public GameObject FortressIslandPanel;
    public Image LoadingBarFill;
    //other vars
    public int selectedIsland;
    public string line;
    public string levelInfoString;
    public List<int> levelUnlockValues;


    // Start is called before the first frame update
    public void Start()
    {
        selectedIsland = 4;
        levelUnlockValues = new List<int>();
        loadLevelData();
    }

    // Update is called once per frame
    public void Update()
    {

    }

    //load level data
    public void loadLevelData()
    {
        string readFromFilePath = Application.dataPath + "/Data/" + "leveldata" + ".txt";
        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();
        foreach (string line in fileLines)
        {
            Console.WriteLine(line);
            try
            {
                int result = Convert.ToInt32(line);
                levelUnlockValues.Add(result);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{line}'");
            }
        }
        //setUnlockedLevels: 0=locked;1=unlocked
        if (levelUnlockValues[0] == 0)
        {
            TutorialIslandStatic.SetActive(false);
            TutorialIslandLocked.SetActive(true);
            TutorialIslandPanel.SetActive(false);
        }
        if (levelUnlockValues[1] == 0)
        {
            CoveIslandStatic.SetActive(false);
            CoveIslandLocked.SetActive(true);
            CoveIslandPanel.SetActive(false);
        }
        if (levelUnlockValues[2] == 0)
        {
            GhostShipIslandStatic.SetActive(false);
            GhostShipIslandLocked.SetActive(true);
            GhostShipIslandPanel.SetActive(false);
        }
        if (levelUnlockValues[3] == 0)
        {
            FortressIslandStatic.SetActive(false);
            FortressIslandLocked.SetActive(true);
            FortressIslandPanel.SetActive(false);
        }
    }

    //crosshair movement
    public void setCrosshairPos()
    {
        switch(selectedIsland)
        {
            case 4:
                Crosshair.transform.localPosition = new Vector3(-416, -282, 0);
                break;
            case 5:
                Crosshair.transform.localPosition = new Vector3(117, -142, 0);
                break;
            case 6:
                Crosshair.transform.localPosition = new Vector3(-375, 179, 0);
                break;
            case 7:
                Crosshair.transform.localPosition = new Vector3(499, 250, 0);
                break;
            default:
                Crosshair.transform.localPosition = new Vector3(-416, -282, 0);
                break;
        }
        
    }

    //island select
    public void setSelectedIsland(int inID)
    {
        selectedIsland = inID;
    }

    public void LoadScene()
    {
        if (selectedIsland != -1) {
            StartCoroutine(LoadSceneAsync(selectedIsland));
        }
    }

    public void LoadForceScene(int inID)
    {
        StartCoroutine(LoadSceneAsync(inID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;

            yield return null;
        }
    }
}
