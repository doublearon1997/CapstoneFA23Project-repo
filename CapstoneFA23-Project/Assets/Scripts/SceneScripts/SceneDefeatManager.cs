using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneDefeatManager : MonoBehaviour
{
    public GameObject fadeToBlack;
    public GameObject fadeToBlackFinal;
    public GameObject menu;

    void Start()
    {
        StartCoroutine(SceneDefeat());
    }

    private IEnumerator SceneDefeat()
    {
        BGMManager.instance.PlayBGM("darkThrone");

        yield return new WaitForSeconds(5.0f);

        fadeToBlack.SetActive(true);

        yield return new WaitForSeconds(2.2f);

        menu.SetActive(true);

    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMain());
    }

    private IEnumerator ReturnToMain()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(1f));

        fadeToBlackFinal.SetActive(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("sceneMain");
    }
}
