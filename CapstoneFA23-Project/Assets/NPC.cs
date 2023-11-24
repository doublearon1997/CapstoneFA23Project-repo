using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Coroutine currentTyper = null;
 
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
        dialogueText.text = "";
        npcNameText.text = npcName;
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
        foreach(char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
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
            zeroText();
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
