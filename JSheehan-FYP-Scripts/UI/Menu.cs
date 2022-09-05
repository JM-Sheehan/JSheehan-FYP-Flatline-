using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject homeMenu;
    // Start is called before the first frame update

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void Play()
    {
        SceneManager.LoadScene("BaseGeneration");
    }


    public void Quit()
    {
        Application.Quit();
    }
}
