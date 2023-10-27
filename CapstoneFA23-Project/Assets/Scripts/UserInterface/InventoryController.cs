using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class InventoryUI : MonoBehaviour
{
    public int selectedPanel;
    public int targetPanel;
    public int activeDrag = 0;
    public int swappable = 0;
    public int[,] inventoryArray  = new int[30, 3];
    public GameObject[] panelArray = new GameObject[30];
    public GameObject dragObject;
    string inventoryDataFilePath = Application.dataPath + "/Data/" + "inventorydata" + ".txt";
    string itemIDDataFilePath = Application.dataPath + "/Data/" + "itemID" + ".txt";

    // Start is called before the first frame update
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (activeDrag == 1)
        {
            dragObject.transform.position = mousePosition;
        }
    }

    public void loadInventoryData()
    {
        //load data from datafile and put in local array
        List<string> fileLines = File.ReadAllLines(inventoryDataFilePath).ToList();
        int count = 0;
        foreach (string inLine in fileLines)
        {
            try
            {
                string processLine = inLine;
                string[] processValues = processLine.Split(',');
                for(int i = 0; i < 3; i++)
                {
                    inventoryArray[count, i] = Convert.ToInt32(processValues[i]);
                }
                count++;
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{inLine}'");
            }
        }
        fileLines = null;
        //update inventory UI: slot number with corresponding image and quantity
        for(int panelID = 0; panelID < panelArray.GetLength(0); panelID++)
        {
            string tempFileName = "";
            if (inventoryArray[panelID, 1] > 0)
            {
                //find and return image using itemID.txt and local Assets
                fileLines = File.ReadAllLines(itemIDDataFilePath).ToList();
                foreach (string inLine in fileLines)
                {
                    string processLine = inLine;
                    string[] processValues = processLine.Split(',');
                    try
                    {
                        int tid = Convert.ToInt32(processValues[0]);
                        string tfn = processValues[1];
                        if (tid == inventoryArray[panelID, 1])
                        {
                            tempFileName = Application.dataPath + "/Artwork/UI/InGameUI/Items/" + tfn + ".png";
                            break;
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unable to parse '{inLine}'");
                    }
                }
                //set sprite from itemID and local files
                Sprite tempSprite = loadItemImage(tempFileName);
                if(panelArray[panelID].GetComponent<Image>() == null)
                {
                    panelArray[panelID].AddComponent<Image>();
                }
                panelArray[panelID].GetComponent<Image>().sprite = tempSprite;
                //set number of items in UI
                panelArray[panelID].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[panelID,2].ToString();
            }
        }
    }

    public void printLocalInventoryData()
    {
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            Debug.Log("Row " + i + ": ");
            for (int j = 0; j < inventoryArray.GetLength(1); j++)
            {
                Debug.Log(inventoryArray[i, j] + " ");
            }
        }
    }

    public void swapPanelData()
    {
        if (targetPanel != selectedPanel)
        {
            int[] tempArray = new int[2];
            if (panelArray[targetPanel].GetComponent<Image>() == null)
            {
                panelArray[targetPanel].AddComponent<Image>();
            }
            //store values in temp
            tempArray[0] = inventoryArray[targetPanel, 1];
            tempArray[1] = inventoryArray[targetPanel, 2];
            Sprite tempSprite = panelArray[targetPanel].GetComponent<Image>().sprite;
            //swap
                //set target panel
            inventoryArray[targetPanel, 1] = inventoryArray[selectedPanel, 1];
            inventoryArray[targetPanel, 2] = inventoryArray[selectedPanel, 2];
            
            panelArray[targetPanel].GetComponent<Image>().sprite = panelArray[selectedPanel].GetComponent<Image>().sprite;
            panelArray[targetPanel].GetComponent<Image>().color += new Color(0f, 0f, 0f, 1f);
            //set selected panel
            inventoryArray[selectedPanel, 1] = tempArray[0];
            inventoryArray[selectedPanel, 2] = tempArray[1];
            panelArray[selectedPanel].GetComponent<Image>().sprite = tempSprite;
            if(tempSprite == null)
            {
                
                panelArray[selectedPanel].GetComponent<Image>().color -= new Color(0f, 0f, 0f, 1f);
                panelArray[selectedPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
            else
            {
                panelArray[selectedPanel].GetComponent<Image>().color += new Color(0f, 0f, 0f, 1f);
                panelArray[selectedPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[selectedPanel, 2].ToString();
            }
            //update target text
            panelArray[targetPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[targetPanel, 2].ToString();
            //clear temp values
            tempArray[0] = 0;
            tempArray[1] = 0;
            tempSprite = null;
        }
    }

    public void createDragObject(int inID)
    {
        int tItem = inventoryArray[inID, 1];
        int tQuantity = inventoryArray[inID, 2];
        Sprite tSprite = panelArray[inID].GetComponent<Image>().sprite;
        dragObject.SetActive(true);
        activeDrag = 1;
        dragObject.GetComponent<Image>().sprite = tSprite;
    }

    public void stopDrag()
    {
        activeDrag = 0;
        dragObject.SetActive(false);
        if(swappable == 1)
        {
            swapPanelData();
        }
    }

    public void setSelectedPanel(int inID)
    {
        selectedPanel = inID;
        targetPanel = inID;
        if (inventoryArray[inID, 1] > 0)
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

    public Sprite loadItemImage(string inPath)
    {
        byte[] imageBytes = File.ReadAllBytes(inPath);
        Texture2D returningTex = new Texture2D(1, 1);
        returningTex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(returningTex, new Rect(0, 0, returningTex.width, returningTex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}
