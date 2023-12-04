using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class PartyController : MonoBehaviour
{
    public int selectedPanel;
    public int targetPanel;
    public int activeDrag = 0;
    public int swappable = 0;
    public int[,] partyArray = new int[6, 2];
    bool partyFull = false;
    public GameObject[] panelArray = new GameObject[6];
    public List<Character> listCharacters = new List<Character>();
    public GameObject dragObject;
    string partyDataFilePath = System.IO.Directory.GetCurrentDirectory() + "/Data/" + "partyData" + ".txt";

    // Start is called before the first frame update
    void Start()
    {
        loadPartyData();

        foreach(Character c in listCharacters)
            c.InitializeCharacter();
    }

    // Update is called once per frame
    public void Update()
    {
        if (activeDrag == 1)
        {
            Vector3 mousePosition = Input.mousePosition;
            dragObject.transform.position = mousePosition;
        }
    }

    //file data and system
    public void loadPartyData()
    {
        //load data from datafile "partyDataFilePath" and put in array "partyArray"
        List<string> fileLines = File.ReadAllLines(partyDataFilePath).ToList();
        int count = 0;
        foreach (string inLine in fileLines)
        {
            try
            {
                string[] processValues = inLine.Split(',');
                for (int i = 0; i < 2; i++)
                {
                    partyArray[count, i] = Convert.ToInt32(processValues[i]);
                }
                count++;
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{inLine}'");
            }
        }
    }

    public void writePartyData()
    {
        string[] tempArray = new string[6];
        for (int i = 0; i < partyArray.GetLength(0); i++)
        {
            string tString = "";
            for (int j = 0; j < partyArray.GetLength(1); j++)
            {
                tString = tString + partyArray[i, j];
                if (j < 1)
                    tString = tString + ",";
            }
            tempArray[i] = tString;
        }
        File.WriteAllLines(partyDataFilePath, tempArray);
    }

    //external actions
    public void addCharacter(int inID)
    {
        for (int i = 0; i < partyArray.GetLength(0); i++)
        {
            if (partyArray[i, 1] == 0)
            {
                partyArray[i, 1] = inID;
                break;
            }
        }
        updatePartyUI();
    }

    public void removeCharacter(int inID)
    {
        for (int i = 0; i < partyArray.GetLength(0); i++)
        {
            if (partyArray[i, 1] == inID)
            {
                partyArray[i, 1] = 0;
                break;
            }
        }
        updatePartyUI();
    }

    public List<Character> GetPartyList()
    {
        List<Character> tempListCharacters = new List<Character>();
        for (int i = 0; i<partyArray.GetLength(0); i++)
        {
            if (partyArray[i, 1] != 0)
            {
                foreach(Character tCharacter in listCharacters)
                {
                    if(tCharacter.characterID==partyArray[i,1])
                    {
                        tempListCharacters.Add(tCharacter); 
                    }
                        
                }
            }
        }
        return tempListCharacters;
    }

    //party UI
    public void updatePartyUI()
    {
        //update party UI: slot number with corresponding image and quantity
        for (int panelID = 0; panelID < panelArray.GetLength(0); panelID++)
        {
            if (partyArray[panelID, 1] > 0)
            {
                foreach (Character character in this.listCharacters)
                {
                    if (partyArray[panelID, 1] == character.characterID)
                    {
                        //set sprite from characterID and local files
                        panelArray[panelID].GetComponent<Image>().sprite = character.characterImage;
                        //set alpha to 1
                        panelArray[panelID].GetComponent<Image>().color = new Color(panelArray[panelID].GetComponent<Image>().color.r,
                            panelArray[panelID].GetComponent<Image>().color.g, panelArray[panelID].GetComponent<Image>().color.b, 1f);
                        break;
                    }
                }
            }
            else
            {
                panelArray[panelID].GetComponent<Image>().sprite = null;
                panelArray[panelID].GetComponent<Image>().color = new Color(panelArray[panelID].GetComponent<Image>().color.r,
                    panelArray[panelID].GetComponent<Image>().color.g, panelArray[panelID].GetComponent<Image>().color.b, 0f);
            }
        }
    }

    public void swapPanelData()
    {
        if (targetPanel != selectedPanel)
        {
            int tempInt = 0;
            //store values in temp
            tempInt = partyArray[targetPanel, 1];
            Sprite tempSprite = panelArray[targetPanel].GetComponent<Image>().sprite;
            //swap
            //set target panel
            partyArray[targetPanel, 1] = partyArray[selectedPanel, 1];
            panelArray[targetPanel].GetComponent<Image>().sprite = panelArray[selectedPanel].GetComponent<Image>().sprite;
            //set selected panel
            partyArray[selectedPanel, 1] = tempInt;
            panelArray[selectedPanel].GetComponent<Image>().sprite = tempSprite;
            //set alpha and text
            if (panelArray[selectedPanel].GetComponent<Image>().sprite == null)
            {
                panelArray[selectedPanel].GetComponent<Image>().color = new Color(panelArray[selectedPanel].GetComponent<Image>().color.r,
                    panelArray[selectedPanel].GetComponent<Image>().color.g, panelArray[selectedPanel].GetComponent<Image>().color.b, 0f);
            }
            else
            {
                panelArray[selectedPanel].GetComponent<Image>().color = new Color(panelArray[selectedPanel].GetComponent<Image>().color.r,
                    panelArray[selectedPanel].GetComponent<Image>().color.g, panelArray[selectedPanel].GetComponent<Image>().color.b, 1f);
            }
            if (panelArray[targetPanel].GetComponent<Image>().sprite == null)
            {
                panelArray[targetPanel].GetComponent<Image>().color = new Color(panelArray[targetPanel].GetComponent<Image>().color.r,
                    panelArray[targetPanel].GetComponent<Image>().color.g, panelArray[targetPanel].GetComponent<Image>().color.b, 0f);
            }
            else
            {
                panelArray[targetPanel].GetComponent<Image>().color = new Color(panelArray[targetPanel].GetComponent<Image>().color.r,
                    panelArray[targetPanel].GetComponent<Image>().color.g, panelArray[targetPanel].GetComponent<Image>().color.b, 1f);
            }
        }
    }

    public void createDragObject(int inID)
    {
        int tCharacter = partyArray[inID, 1];
        Sprite tSprite = panelArray[inID].GetComponent<Image>().sprite;
        dragObject.SetActive(true);
        activeDrag = 1;
        dragObject.GetComponent<Image>().sprite = tSprite;
    }

    public void stopDrag()
    {
        activeDrag = 0;
        dragObject.SetActive(false);
        if (swappable == 1)
        {
            swapPanelData();
        }
    }

    public void setSelectedPanel(int inID)
    {
        selectedPanel = inID;
        targetPanel = inID;
        if (partyArray[inID, 1] > 0)
        {
            createDragObject(inID);
            swappable = 1;
        }
        else
        {
            swappable = 0;
        }
    }

    public void setTargetPanel(int inID)
    {
        targetPanel = inID;
    }
}
