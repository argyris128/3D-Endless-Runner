using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float GameSpeed {get; set;}
    private Coroutine increaseSpeedCoroutine = null;

    private bool gameisRunning;
    public bool GameIsRunning {
        get => gameisRunning;
        set
        {
            gameisRunning = value;
            if(gameisRunning)
            {
                Time.timeScale = 1f;
            } else
            {
                Time.timeScale = 0f;
            }
        }
    }


    [Serializable]
    public struct ObjectIDPair
    {
        public GameObject obj;
        public int id;
    }
    public List<ObjectIDPair> objectIDs;

    public List<ObjectIDPair> CurrObjects;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            Instance.GameIsRunning = true;
        }
        else if(scene.name == "MainMenu")
        {
            Instance.CurrObjects.Clear();
        }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Instance.CurrObjects = new();
            Instance.GameSpeed = 10f;
            Instance.increaseSpeedCoroutine = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("GameSpeed"))
            Instance.GameSpeed = PlayerPrefs.GetFloat("GameSpeed");

        if(SceneManager.GetActiveScene().name == "Game")
            increaseSpeedCoroutine = StartCoroutine(IncreaseSpeed());
    }

    IEnumerator IncreaseSpeed()
    {
        while (true)
        {
            //Debug.Log(Instance.GameSpeed);
            Instance.GameSpeed += 0.05f;
            yield return new WaitForSeconds(1f);
        }
    }

    public void RemoveCurrObject(GameObject obj)
    {
        ObjectIDPair pair = Instance.CurrObjects.Find(x => x.obj == obj);
        Instance.CurrObjects.Remove(pair);
    }

    public void NewGame()
    {
        Instance.CurrObjects.Clear();

        Instance.GameSpeed = 10f;

        PlayerPrefs.DeleteAll();

        PlayerPrefs.Save();

        SceneManager.LoadScene("Game");
    }

    public void ExitAndDeleteSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController playerController = player.GetComponent<PlayerController>();
        BuffsController buffsController = player.GetComponent<BuffsController>();
        SpawnTrigger spawnTrigger = player.GetComponent<SpawnTrigger>();

        PlayerPrefs.SetInt("Score", Score.score);
        PlayerPrefs.SetFloat("ScoreInterval", Score.interval);
        PlayerPrefs.SetInt("CurrentLane", playerController.CurrLane);

        PlayerPrefs.SetFloat("minSpawnObstacle", spawnTrigger.minSpawnObstacle);
        PlayerPrefs.SetFloat("maxSpawnObstacle", spawnTrigger.maxSpawnObstacle);
        PlayerPrefs.SetFloat("minSpawnBuff", spawnTrigger.minSpawnBuff);
        PlayerPrefs.SetFloat("maxSpawnBuff", spawnTrigger.maxSpawnBuff);
        PlayerPrefs.SetFloat("minSpawnCoin", spawnTrigger.minSpawnCoin);
        PlayerPrefs.SetFloat("maxSpawnCoin", spawnTrigger.maxSpawnCoin);

        if(buffsController.SpeedupActive)
            PlayerPrefs.SetFloat("GameSpeed", Instance.GameSpeed / 1.5f);
        else if(buffsController.SlowmoActive)
            PlayerPrefs.SetFloat("GameSpeed", Instance.GameSpeed * 2f);
        else
            PlayerPrefs.SetFloat("GameSpeed", Instance.GameSpeed);

        string saveObjects = "";
        foreach(ObjectIDPair pair in Instance.CurrObjects)
        {
            float posX = pair.obj.transform.position.x;
            float posY = pair.obj.transform.position.y;
            float posZ = pair.obj.transform.position.z;

            if(pair.obj.TryGetComponent<SpawnOffset>(out var spawnOffset))
            {
                posX -= spawnOffset.x;
                posY -= spawnOffset.y;
                posZ -= spawnOffset.z;
            }

            saveObjects += pair.id + "," + 
                posX.ToString(CultureInfo.InvariantCulture) + "," + 
                posY.ToString(CultureInfo.InvariantCulture) + "," + 
                posZ.ToString(CultureInfo.InvariantCulture) + ";";   // "id,x,y,z;"
        }
        if(saveObjects != "")
            PlayerPrefs.SetString("Objects", saveObjects);

        string saveBuffs = "";
        foreach(var buff in buffsController.Buffs)
        {
            saveBuffs += (int)buff + ",";
        }
        if(saveBuffs != "") {
            PlayerPrefs.SetString("Buffs", saveBuffs);
            PlayerPrefs.SetFloat("BuffTimer", SliderTimer.timer);
        }

        PlayerPrefs.SetInt("SaveExists", 1);
        
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        Instance.CurrObjects.Clear();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        BuffsController buffsController = player.GetComponent<BuffsController>();

        if(PlayerPrefs.HasKey("GameSpeed"))
            Instance.GameSpeed = PlayerPrefs.GetFloat("GameSpeed");

        if(PlayerPrefs.HasKey("Objects"))
        {
            string data = PlayerPrefs.GetString("Objects");
            
            string[] rows = data.TrimEnd(';').Split(';');

            foreach(string row in rows)
            {
                string[] columns = row.Split(',');

                int _id = int.Parse(columns[0]);
                float posX = float.Parse(columns[1], CultureInfo.InvariantCulture);
                float posY = float.Parse(columns[2], CultureInfo.InvariantCulture);
                float posZ = float.Parse(columns[3], CultureInfo.InvariantCulture);

                ObjectIDPair pair = objectIDs.Find(x => x.id == _id);
                Vector3 pos = new(posX, posY, posZ);

                GameObject instance = Instantiate(pair.obj, pos, Quaternion.identity);

                ObjectIDPair newPair = new()
                {
                    obj = instance,
                    id = _id
                };

                Instance.CurrObjects.Add(newPair);
            }
        }

        if(PlayerPrefs.HasKey("Buffs"))
        {
            string data = PlayerPrefs.GetString("Buffs");

            string[] buffs = data.TrimEnd(',').Split(',');

            foreach(string s in buffs)
            {
                int buff = int.Parse(s);
                
                buffsController.Buffs.Enqueue((BuffsController.Buff)buff);
            }

            buffsController.ProcessNextBuff(PlayerPrefs.GetFloat("BuffTimer"));
        }
    }

}
