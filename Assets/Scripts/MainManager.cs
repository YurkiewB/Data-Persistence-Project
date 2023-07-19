using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text BestScoreText;

    public TextMeshProUGUI MenuBestScoreText;

    public Text ScoreText;
    public GameObject GameOverText;

    public TextMeshProUGUI NameInputField;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    public static string PlayerName = "";

    public static int HighScore;

    private static string HighScoreName ="";

    private int SpawnCount = 1;


    
    // Start is called before the first frame update
    void Start()
    {
        LoadName();
        MenuBestScoreText.text = $"Best score: {HighScoreName}; Score:{HighScore}";
    
        
    }

    void StartNew()
    {
        if (PlayerName != ""){
            BestScoreText.text = $"Best score: {HighScoreName}; Score:{HighScore}";
        }
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {   
            if (SpawnCount != 0)
            {
                SpawnCount -= 1;
                StartNew();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartNew();
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        if (m_Points > HighScore)
        {
            HighScore = m_Points;
            if (PlayerName != HighScoreName)
            {
                HighScoreName = PlayerName;
            }
            SaveName();
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    [System.Serializable]
    class SaveData
    {
        public string highScoreName;
        public int highScore;
    }

    public void SaveName() 
    {
        SaveData data = new SaveData();
        data.highScoreName = HighScoreName;
        data.highScore = HighScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadName()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            HighScoreName = data.highScoreName;
            HighScore = data.highScore;
        }
    }

    public void ToGame()
    {
        PlayerName = NameInputField.text;
        SaveName();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        SaveName();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
