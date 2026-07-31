using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class SliderTimer : MonoBehaviour
{
    private Slider slider;
    public static float timer = 0f;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void StartTimer(float duration)
    {
        StartCoroutine(DecreaseSlider(duration));
    }

    IEnumerator DecreaseSlider(float duration)
    {
        if(PlayerPrefs.HasKey("BuffTimer")) {
            timer = PlayerPrefs.GetFloat("BuffTimer");
            PlayerPrefs.DeleteKey("BuffTimer");
        } else
            timer = 0f;

        slider.value = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            slider.value = Mathf.Lerp(1f, 0f, timer / duration);

            yield return null;
        }

        slider.value = 0f;
        timer = 0f;
    }
}
