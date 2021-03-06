using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseUI;
    public AudioSource AS;
    public GameObject SkillSelect;
    // Start is called before the first frame update
    void Start()
    {
        GameIsPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SkillSelect.activeSelf == false)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        PauseUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
        AS.Pause();
    }

    void Resume()
    {
        PauseUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
        AS.UnPause();
    }
}
