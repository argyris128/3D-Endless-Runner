using UnityEngine;

public class MovingPillar : MonoBehaviour
{
    public float speed;

    void Update() {
        Vector3 pos = transform.position;

        float offset = Mathf.PingPong(Time.time * speed, 2.7f * 2) - 2.7f;

        pos.x = offset;

        transform.position = pos;
    }
}
