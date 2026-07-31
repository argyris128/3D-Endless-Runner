using UnityEngine;
using System.Collections;

public class ObjectMove : MonoBehaviour
{
    void Update()
    {
        transform.position += new Vector3(0, 0, -GameManager.Instance.GameSpeed) * Time.deltaTime;
    }
}
