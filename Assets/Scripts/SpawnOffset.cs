using UnityEngine;

public class SpawnOffset : MonoBehaviour
{
    public float x, y, z;
    void Start()
    {
        transform.position += new Vector3(x, y, z);
    }
}
