using UnityEngine;
using System.Collections;

public class SpawnTrigger : MonoBehaviour
{
    public GameObject Platform;
    public GameObject[] Obstacles;
    public GameObject[] Buffs;
    public GameObject SpeedupDebuff;
    public GameObject Coin;
    public GameObject Trophy;
    public Transform[] LaneSpawns;

    public float minSpawnObstacle, maxSpawnObstacle;
    public float minSpawnBuff, maxSpawnBuff;
    public float minSpawnCoin, maxSpawnCoin;

    private Coroutine increaseSpawnRate = null;
    private Coroutine spawnObstacleLoop = null;
    private Coroutine spawnCoinLoop = null;
    private Coroutine spawnTrophyLoop = null;
    private Coroutine spawnBuffsLoop = null;
    private Coroutine speedupDebuffLoop = null;

    void Start()
    {
        if(PlayerPrefs.HasKey("minSpawnObstacle"))
        {
            minSpawnObstacle = PlayerPrefs.GetFloat("minSpawnObstacle");
            maxSpawnObstacle = PlayerPrefs.GetFloat("maxSpawnObstacle");
            minSpawnBuff = PlayerPrefs.GetFloat("minSpawnBuff");
            maxSpawnBuff = PlayerPrefs.GetFloat("maxSpawnBuff");
            minSpawnCoin = PlayerPrefs.GetFloat("minSpawnCoin");
            maxSpawnCoin = PlayerPrefs.GetFloat("maxSpawnCoin");
        } else
        {
            minSpawnObstacle = 2f;
            maxSpawnObstacle = 3f;
            minSpawnBuff = 15f;
            maxSpawnBuff = 20f;
            minSpawnCoin = 1.5f;
            maxSpawnCoin = 3f;
        }

        increaseSpawnRate = StartCoroutine(IncreaseSpawnRate());
        spawnObstacleLoop = StartCoroutine(SpawnObstacleLoop());
        spawnCoinLoop = StartCoroutine(SpawnCoinLoop());
        spawnTrophyLoop = StartCoroutine(SpawnTrophyLoop());
        spawnBuffsLoop = StartCoroutine(SpawnBuffsLoop());
        speedupDebuffLoop = StartCoroutine(SpeedupDebuffLoop());
        
    }

    private IEnumerator IncreaseSpawnRate()
    {
        while(minSpawnObstacle > 0.5f)
        {
            yield return new WaitForSeconds(1f);
            minSpawnObstacle -= 0.006f;
            maxSpawnObstacle -= 0.015f;
            minSpawnBuff -= 0.01f;
            maxSpawnBuff -= 0.01f;
            minSpawnCoin -= 0.007f;
            maxSpawnCoin -= 0.007f;
        }
    }

    private IEnumerator SpawnObstacleLoop()
    {
        while(true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(Random.Range(minSpawnObstacle, maxSpawnObstacle));          
        }
    }

    private IEnumerator SpawnCoinLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnCoin, maxSpawnCoin));
            SpawnCoin();
        }
    }

    private IEnumerator SpawnTrophyLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 25f));
            SpawnTrophy();
        }   
    }

    private IEnumerator SpawnBuffsLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnBuff, maxSpawnBuff));
            SpawnBuff();
        }
    }

    private IEnumerator SpeedupDebuffLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnBuff, maxSpawnBuff));
            SpawnDebuff();
        }
    }

    private void SpawnObstacle()
    {
        int obstacleIndex = Random.Range(0, Obstacles.Length);
        GameObject obstacle = Obstacles[obstacleIndex];

        int laneIndex = Random.Range(0, 3);
        Vector3 spawnPoint = LaneSpawns[laneIndex].position;

        if (obstacle.CompareTag("Big Obstacle"))
        {
            spawnPoint = LaneSpawns[1].position;
        }

        GameObject obstalceInstance = Instantiate(obstacle, spawnPoint, Quaternion.identity);

        int object_id = GameManager.Instance.objectIDs.Find(x => x.obj == obstacle).id;

        GameManager.ObjectIDPair currPair = new()
        {
            id = object_id,
            obj = obstalceInstance
        };

        GameManager.Instance.CurrObjects.Add(currPair);
    }

    private void SpawnCoin()
    {
        int laneIndex = Random.Range(0, 6);
        Vector3 spawnPoint = LaneSpawns[laneIndex].position;

        GameObject coinInstance = Instantiate(Coin, spawnPoint, Quaternion.identity);

        int object_id = GameManager.Instance.objectIDs.Find(x => x.obj == Coin).id;

        GameManager.ObjectIDPair currPair = new()
        {
            id = object_id,
            obj = coinInstance
        };

        GameManager.Instance.CurrObjects.Add(currPair);
    }

    private void SpawnTrophy()
    {
        int laneIndex = Random.Range(0, 6);
        Vector3 spawnPoint = LaneSpawns[laneIndex].position;

        GameObject trophyInstance = Instantiate(Trophy, spawnPoint, Quaternion.identity);

        int object_id = GameManager.Instance.objectIDs.Find(x => x.obj == Trophy).id;

        GameManager.ObjectIDPair currPair = new()
        {
            id = object_id,
            obj = trophyInstance
        };

        GameManager.Instance.CurrObjects.Add(currPair);
    }

    private void SpawnBuff()
    {
        int buffIndex = Random.Range(0, Buffs.Length);
        GameObject buff = Buffs[buffIndex];

        int laneIndex = Random.Range(0, 6);
        Vector3 spawnPoint = LaneSpawns[laneIndex].position;

        GameObject buffInstance = Instantiate(buff, spawnPoint, Quaternion.identity);

        int object_id = GameManager.Instance.objectIDs.Find(x => x.obj == buff).id;

        GameManager.ObjectIDPair currPair = new()
        {
            id = object_id,
            obj = buffInstance
        };

        GameManager.Instance.CurrObjects.Add(currPair);
    }

    private void SpawnDebuff()
    {
        int laneIndex = Random.Range(0, 3);
        Vector3 spawnPoint = LaneSpawns[laneIndex].position;

        GameObject buffInstance = Instantiate(SpeedupDebuff, spawnPoint, Quaternion.identity);

        int object_id = GameManager.Instance.objectIDs.Find(x => x.obj == SpeedupDebuff).id;

        GameManager.ObjectIDPair currPair = new()
        {
            id = object_id,
            obj = buffInstance
        };

        GameManager.Instance.CurrObjects.Add(currPair);
    }
}
