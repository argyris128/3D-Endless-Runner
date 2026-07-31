using UnityEngine;
using System.Collections;
using TMPro;

public class Score : MonoBehaviour
{
    private static Coroutine scoreCoroutine = null;
    private static TMP_Text scoreText;

    private static int scorefield;
    public static int score {
        get => scorefield;
        set
        {
            scorefield = value;
            scoreText.text = scorefield.ToString();
        }
    }
    public static float interval = 1f;

    void Awake()
    {
        scoreCoroutine = null;
        scoreText = GetComponent<TMP_Text>();
    }

    void Start()
    {   
        if(PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
            interval = PlayerPrefs.GetFloat("ScoreInterval");
        } else
        {
            score = 0;
            interval = 1f;
        }
    }

    void Update()
    {
        if (GameManager.Instance.GameIsRunning)
            scoreCoroutine ??= StartCoroutine(IncreaseScore());
        else {
            if(scoreCoroutine != null) {
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = null;
            }
        }
    }

    static IEnumerator IncreaseScore()
    {
        while (interval > 0.1)
        {
            yield return new WaitForSeconds(interval);

            interval -= 0.002f;

            score++;
        }
    }
}
