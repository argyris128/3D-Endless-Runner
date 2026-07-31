using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    public float duration = 1f;
    public float moveDistance = 40f;

    public void Enable(int score)
    {
        GameObject obj = Instantiate(gameObject, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);

        TMP_Text text = obj.GetComponent<TMP_Text>();
        RectTransform rect = obj.GetComponent<RectTransform>();
        
        StartCoroutine(Animate(obj, score, text, rect));
    }

    IEnumerator Animate(GameObject obj, int score, TMP_Text text, RectTransform rect)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveDistance;

        text.text = "+" + score.ToString();

        Color startColor = text.color;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;

            yield return null;
        }

        Destroy(obj);
    }
}
