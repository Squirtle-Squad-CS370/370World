using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Slider loadslide;
    public Text loadtext;
    public Text action;

    public void PlayGameEasy()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    public void PlayGameMedium()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    public void PlayGameHard()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {

            if (operation.progress < .9f)
                action.text = "Loading Assets...";
            else if (operation.progress == .9f)
                action.text = "Generating World...";
            else
                action.text = "";
            
            loadslide.value = operation.progress;
            loadtext.text = operation.progress * 100f + "%";
  
            yield return null;
        }
    }

    public void QuitGame()
    {
        Debug.Log("READ IF QUIT COMMAND REACHED");
        Application.Quit();
    }
}
