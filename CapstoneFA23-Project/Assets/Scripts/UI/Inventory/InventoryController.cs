using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class InventoryController : MonoBehaviour
{
    public int selectedPanel;
    public int targetPanel;
    public int activeDrag = 0;
    public int swappable = 0;
    public int[,] inventoryArray  = new int[30, 3];
    bool inventoryFull = false;
    public GameObject[] panelArray = new GameObject[30];
    public List<Item> listItems = new List<Item>();
    public GameObject dragObject;
    string inventoryDataFilePath = Application.dataPath + "/Data/" + "inventorydata" + ".txt";

    // Start is called before the first frame update
    public void Start()
    {
        loadInventoryData();
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
    public void loadInventoryData()
    {
        //load data from datafile "inventoryDataFilePath" and put in array "inventoryArray"
        List<string> fileLines = File.ReadAllLines(inventoryDataFilePath).ToList();
        int count = 0;
        foreach (string inLine in fileLines)
        {
            try
            {
                string[] processValues = inLine.Split(',');
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
    }

    public void writeInventoryData()
    {
        string[] tempArray = new string[30];
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            string tString = "";
            for (int j = 0; j < inventoryArray.GetLength(1); j++)
            {
                tString = tString + inventoryArray[i, j];
                if (j < 2)
                    tString = tString + ",";
            }
            tempArray[i] = tString;
        }
        File.WriteAllLines(inventoryDataFilePath, tempArray);
    }

    //external actions
    public void addItem(int inID, int inQuantity)
    {
        bool idFound = false;
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            if (inventoryArray[i, 1] == inID)
            {
                inventoryArray[i, 2] += inQuantity;
                idFound = true;
                break;
            }
        }
        if (idFound==false)
        {
            for (int i = 0; i < inventoryArray.GetLength(0); i++)
            {
                if (inventoryArray[i, 1] == 0)
                {
                    inventoryArray[i, 1] = inID;
                    inventoryArray[i, 2] = inQuantity;
                    break;
                }
            }
        }
        updateInventory();
    }

    public void removeItem(int inID, int inQuantity)
    {
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            if (inventoryArray[i, 1] == inID)
            {
                inventoryArray[i, 2] -= inQuantity;
                if(inventoryArray[i, 2] <= 0)
                {
                    inventoryArray[i, 1] = 0;
                    inventoryArray[i, 2] = 0;
                }
                break;
            }
        }
        updateInventory();
    }

    public bool searchInventory(int inID, int inQuantity)
    {
        bool foundItem = false;
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            if (inventoryArray[i, 1] == inID)
            {
                if(inventoryArray[i, 2] >= inQuantity)
                {
                    foundItem = true;
                    break;
                }
            }
        }
        return foundItem;
    }
    
    public Dictionary<Item,int> GetItemList()
    {
        Dictionary<Item, int> tempListItems = new Dictionary<Item, int>();
        for (int i = 0; i < inventoryArray.GetLength(0); i++)
        {
            if (inventoryArray[i, 1] != 0)
            {
                foreach (Item tItem in listItems)
                {
                    if (tItem.itemID == inventoryArray[i, 1])
                        tempListItems.Add(tItem, inventoryArray[i,2]);
                }
            }
        }
        return tempListItems;
    }
    //inventory UI
    public void updateInventory()
    {
        //update inventory UI: slot number with corresponding image and quantity
        for (int panelID = 0; panelID < panelArray.GetLength(0); panelID++)
        {
            if (inventoryArray[panelID, 1] > 0)
            {
                foreach (Item item in this.listItems)
                {
                    if (inventoryArray[panelID, 1] == item.itemID)
                    {
                        //set sprite from itemID and local files
                        panelArray[panelID].GetComponent<Image>().sprite = item.itemImage;
                        //set number of items in UI
                        panelArray[panelID].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[panelID, 2].ToString();
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
                panelArray[panelID].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        writeInventoryData();
    }
    
    public void swapPanelData()
    {
        if (targetPanel != selectedPanel)
        {
            int[] tempArray = new int[2];
            //store values in temp
            tempArray[0] = inventoryArray[targetPanel, 1];
            tempArray[1] = inventoryArray[targetPanel, 2];
            Sprite tempSprite = panelArray[targetPanel].GetComponent<Image>().sprite;
            //swap
            //set target panel
            inventoryArray[targetPanel, 1] = inventoryArray[selectedPanel, 1];
            inventoryArray[targetPanel, 2] = inventoryArray[selectedPanel, 2];
            panelArray[targetPanel].GetComponent<Image>().sprite = panelArray[selectedPanel].GetComponent<Image>().sprite;
            //set selected panel
            inventoryArray[selectedPanel, 1] = tempArray[0];
            inventoryArray[selectedPanel, 2] = tempArray[1];
            panelArray[selectedPanel].GetComponent<Image>().sprite = tempSprite;
            //update text
            panelArray[selectedPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[selectedPanel, 2].ToString();
            panelArray[targetPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = inventoryArray[targetPanel, 2].ToString();
            //set alpha and text
            if(panelArray[selectedPanel].GetComponent<Image>().sprite == null)
            {
                panelArray[selectedPanel].GetComponent<Image>().color = new Color(panelArray[selectedPanel].GetComponent<Image>().color.r, 
                    panelArray[selectedPanel].GetComponent<Image>().color.g, panelArray[selectedPanel].GetComponent<Image>().color.b, 0f);
                panelArray[selectedPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
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
                panelArray[targetPanel].transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "";
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
}
