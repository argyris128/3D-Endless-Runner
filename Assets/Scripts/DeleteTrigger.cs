using UnityEngine;

public class DeleteTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DeleteTrigger"))
        {
            GameManager.Instance.RemoveCurrObject(gameObject);
            Destroy(gameObject);
        }
    }
}
