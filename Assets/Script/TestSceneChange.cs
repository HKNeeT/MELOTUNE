using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TestSceneChange : MonoBehaviour
{
    public void DeleteData() 
    {
        PlayerPrefs.DeleteAll();
    }
    public void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
