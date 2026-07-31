using UnityEngine;

public class BounceAndSpin : MonoBehaviour
{
    public Vector3 direction;
    public float spinSpeed = 180f;
    public float bounceSpeed;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime * direction);

        Vector3 pos = transform.position;

        float offset = Mathf.PingPong(Time.time * bounceSpeed, 0.1f * 2) - 0.1f;

        pos.y = startY + offset;

        transform.position = pos;
    }
}
