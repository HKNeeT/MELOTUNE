using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonCheck : MonoBehaviour
{
    public List<NoteObject> NoteList;
    private NoteObject noteObj;

    public GameObject noteParticle;
    public GameObject missParticle;

    public GameObject GradeSpawn;
    public GameObject[] GradeDisplay;

    public GunShooting gun;

    public CameraShake camShake;
    // Skill script reference
    public SkillEasy EZ;
    public SecondSkill Ss;
    // For Setting KeyCode on Button
    public KeyCode Up = KeyCode.W;
    public KeyCode Down = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;

    //Tracking score and combo in game
    public static int Score = 0;
    public static int Combo = 0;
    //Tracking grade during the game
    public static int PerfectS = 0;
    public static int GreatS = 0;
    public static int GoodS = 0;
    public static int MissS = 0;
    public static float Skill = 0.0f;
    public static string SceneName;
    public static bool isSkillUsed;

    public bool NoteWrong;
    public bool NotePerfect;
    public bool NoteGreat;
    public bool NoteGood;
   
    public AudioSource AS;
    public AudioSource MAS;

    public Text comboText;
    public Text scoreText;
    //display what grade player press

    void Start()
    {
        // Initialize List
        NoteList = new List<NoteObject>();
        // For Debugging (!!Create circle for distance detection!!)
        /*DrawCircle(0.4f, 0.05f);
        DrawCircle(1.0f, 0.05f);
        /DrawCircle(2.0f, 0.05f);*/

        // Set SFX Volume via SoungController.cs
        GetComponent<AudioSource>().volume = SoundController.sfxVolume;

        //Setting Text Combo & Score to 0
        comboText.text = "Combo : " + Combo.ToString();
        scoreText.text = "Score : " + Score.ToString();
        Combo = 0;
        Score = 0;
        PerfectS = 0;
        GreatS = 0;
        GoodS = 0;
        MissS = 0;
        PlayerPrefs.SetInt("highcombo", Combo);
        SkillBar.AmountSkill = 0f;
        SceneName = SceneManager.GetActiveScene().name;
        Debug.Log(SceneName);
    }

    void Update()
    {
        CheckNoteOverFlow();

        noteObj = currentNote();
        // If currentNote is null then return
        if (currentNote() == null)
        {
            return;
        }

        if (Input.GetKeyDown(Up) || Input.GetKeyDown(Down) ||
            Input.GetKeyDown(Left) || Input.GetKeyDown(Right))
        {
            // If User Hit Note then Destroy Object
            if (Input.GetKeyDown(noteObj.keyToPress))
            {
                DistantCheck dc = noteObj.GetComponent<DistantCheck>();
                AS.Play();
                if (dc.distance <= 0.4)
                {
                    if (!isSkillUsed)
                    {
                        Skill += 0.1f;
                        SkillBar.AmountSkill += 5f;
                    }
                    Combo += 1;
                    PerfectS++;
                    Score += 1000;
                    NotePerfect = true;
                    GameObject prefab = Instantiate(GradeDisplay[0],GradeSpawn.transform.position, Quaternion.identity);
                }
                else if (dc.distance <= 1.0)
                {
                    if (!isSkillUsed)
                    {
                        Skill += 0.075f;
                        SkillBar.AmountSkill += 3f;
                    }
                    Combo += 1;
                    GreatS++;
                    Score += 500;
                    GameObject prefab = Instantiate(GradeDisplay[1], GradeSpawn.transform.position, Quaternion.identity);
                }
                else if (dc.distance <= 2.0)
                {
                    if (!isSkillUsed)
                    {
                        Skill += 0.05f;
                        SkillBar.AmountSkill += 1f;
                    }
                    Combo = 0;
                    GoodS++;
                    Score += 300;
                    GameObject prefab = Instantiate(GradeDisplay[2], GradeSpawn.transform.position, Quaternion.identity);
                }

                gun.Shoot(noteObj.transform.position);

                // Create Particle
                GameObject particle = Instantiate(noteParticle, noteObj.transform.position, Quaternion.identity);
                particle.GetComponent<ParticleSystem>().startColor = noteObj.particleColor;
                float duration = particle.GetComponent<ParticleSystem>().duration + particle.GetComponent<ParticleSystem>().startLifetime;
                // Destroy when particle play done
                Destroy(particle, duration);
            }
            else
            {
                // Create Particle
                GameObject particle = Instantiate(missParticle, noteObj.transform.position, Quaternion.identity);
                float duration = particle.GetComponent<ParticleSystem>().duration + particle.GetComponent<ParticleSystem>().startLifetime;
                // Destroy when particle play done
                Destroy(particle, duration);

                Miss();
            }
            deleteNote(noteObj);


            // Delete Note Object
            Destroy(noteObj.gameObject);
        }
        if(Combo > PlayerPrefs.GetInt("highcombo")) 
        {
            PlayerPrefs.SetInt("highcombo", Combo);
        }
        //Record highscore
        if (SceneName == "Tun_Scene")
        {
            if (Score > PlayerPrefs.GetInt("Highscore1"))
            {
                PlayerPrefs.SetInt("Highscore1", Score);
            }           
        }
        else if (SceneName == "SecondSong_Scene")
        {
            if (Score > PlayerPrefs.GetInt("Highscore2"))
            {
                PlayerPrefs.SetInt("Highscore2", Score);
            }
        }
        else if (SceneName == "ThirdSong_Scene")
        {
            if (Score > PlayerPrefs.GetInt("Highscore3"))
            {
                PlayerPrefs.SetInt("Highscore3", Score);
            }
        }
        Skill = (Skill >= 1.0f ? 1.0f : Skill);
        comboText.text = "Combo : " + Combo.ToString();
        scoreText.text = "Score : " + Score.ToString();
        
    }
    public void Miss() 
    {
        MAS.Play();
        MissS++;
        NoteWrong = true;
        if(EZ.isskill == false)
        {
            Combo = 0;
            camShake.ShakeCam(0.3f, 0.075f);
            GameObject prefab = Instantiate(GradeDisplay[3], GradeSpawn.transform.position, Quaternion.identity);
        }
        if (EZ.isskill) 
        {
            Combo += 1;
            Score += 1000;
            EZ.skillamount--;
            GameObject prefab = Instantiate(GradeDisplay[0], GradeSpawn.transform.position, Quaternion.identity);
        }
    }

    // Add note to NoteList
    public void addNote(NoteObject note)
    {
        NoteList.Add(note);
        NoteList.Sort(SortByPosX);
    }

    public void CheckNoteOverFlow()
    {
        if (NoteList.Count == 0)
        {
            return;
        }

        if (NoteList[0] == null)
        {
            NoteList.RemoveAt(0);
        }
    }

    // Delete note that are on the front
    public void deleteNote(NoteObject noteObj)
    {
        // If NoteList is empty -> return
        if (NoteList.Count == 0)
        {
            return;
        }

        // If Peek of NoteList is noteObj then delete from List
        if (NoteList[0] == noteObj)
        {
            NoteList.RemoveAt(0);
        }
    }

    // For checking what note is on the front
    public NoteObject currentNote()
    {
        // If NoteList is empty -> return null
        if (NoteList.Count == 0)
        {
            return null;
        }

        return NoteList[0];
    }

    int SortByPosX(NoteObject a, NoteObject b)
    {
        return a.transform.position.x.CompareTo(b.transform.position.x);
    }

    // Function for create circle (!!For distance detection!!)
    private void DrawCircle(float radius, float lineWidth)
    {
        GameObject obj = new GameObject("Circle with radius : " + radius);
        obj.GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y, -1.0f);

        int segments = 360;
        LineRenderer line = obj.AddComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        int pointCounts = segments + 1;
        Vector3[] points = new Vector3[pointCounts];

        for (int i = 0; i < pointCounts; i++)
        {
            float rad = Mathf.Deg2Rad * (i * 360.0f / segments);
            points[i] = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0.0f);
        }
        line.SetPositions(points);
    }
}