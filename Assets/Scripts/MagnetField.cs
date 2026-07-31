using UnityEngine;

public class MagnetField : MonoBehaviour
{
    public Transform player;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin") || other.gameObject.CompareTag("Trophy"))
        {
            other.GetComponent<AttractToPlayer>().Attract(player);
        }
    }
}
