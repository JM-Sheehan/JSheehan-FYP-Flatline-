using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("BaseGeneration");
    }

    public void Home()
    {
        SceneManager.LoadScene("Home");
    }

}
