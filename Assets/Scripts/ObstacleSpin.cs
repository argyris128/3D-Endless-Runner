using UnityEngine;

public class ObstacleSpin : MonoBehaviour
{
    public Vector3 direction;
    public float spinSpeed = 180f;
    void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime * direction);
    }
}
