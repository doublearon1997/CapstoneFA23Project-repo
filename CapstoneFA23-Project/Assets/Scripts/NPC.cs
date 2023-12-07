using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class NPC : MonoBehaviour
{

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text npcNameText;
    public string npcName;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;
    public bool destroyAtEnd;
    public bool fightAtEnd;
    public Encounter encounter;

    public Coroutine currentTyper = null;

    public GameStateData.GameStateVariable removalVariable;
    public int removalStateValue; 


    public GameStateData.GameStateVariable dialougeEndState;
    public int dialougeEndStateAdd;

    public NPCEndAction endActionScript;
 
    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy) 
                NextLine();
            else
            {
                dialoguePanel.SetActive(true);
                npcNameText.text = npcName;
                currentTyper = StartCoroutine(Typing());
            }
        }
        if(dialogueText.text == dialogue[index]) 
        {
            contButton.SetActive(true);
        }
    }


    void Start()
    {
        if(removalVariable != GameStateData.GameStateVariable.None && GameStateData.GetGameStateValue(removalVariable) >= removalStateValue)
            gameObject.SetActive(false);
        else 
        {
            dialogueText.text = "";
            npcNameText.text = npcName;
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);

        if(currentTyper != null)
            StopCoroutine(currentTyper);
    }

    IEnumerator Typing()
    {
        float currentWaitTime = 0.12f;
        foreach(char letter in dialogue[index].ToCharArray())
        {
            if(currentWaitTime >= 0.12f)
            {
                SEManager.instance.PlaySE("type");
                currentWaitTime = 0.0f;
            }
                

            dialogueText.text += letter;
            currentWaitTime += wordSpeed;
            yield return new WaitForSeconds(wordSpeed);
        }
    }


    public void NextLine()
    {
        StopCoroutine(currentTyper);
        contButton.SetActive(false);

        if(index < dialogue.Length -1)
        {
            index++;
            dialogueText.text = "";
            currentTyper = StartCoroutine(Typing());
        }
        else
        {
            if(dialougeEndState != GameStateData.GameStateVariable.None)
                GameStateData.AddGameStateValue(dialougeEndState, dialougeEndStateAdd);
            if(endActionScript != null)
                endActionScript.DoAction();
            
            if (destroyAtEnd)
            {
                gameObject.SetActive(false);
            }
            if (fightAtEnd)
            {
                BattleSystem.currentEncounter = encounter;
                SceneManager.LoadScene("sceneBattle");
            }
            zeroText();
        }
            
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            playerIsClose = true;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }
}
