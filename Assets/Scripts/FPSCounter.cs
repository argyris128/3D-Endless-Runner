using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    float fps;

    void Update()
    {
        fps = 1f / Time.deltaTime;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(30, 90, 600, 150), "FPS: " + fps.ToString("F0"));
    }
}