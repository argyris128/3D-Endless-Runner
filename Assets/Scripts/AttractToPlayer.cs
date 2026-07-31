using UnityEngine;

public class AttractToPlayer : MonoBehaviour
{
    public float force = 5f;
    private bool attracted;
    private Transform player;

    public void Attract(Transform _player)
    {
        player = _player;
        GetComponent<BounceAndSpin>().enabled = false;
        GetComponent<ObjectMove>().enabled = false;
        attracted = true;
    }

    void Awake()
    {
        attracted = false;
    }

    void Update()
    {
        if(attracted)
            transform.position = Vector3.MoveTowards(transform.position, player.position, force * Time.deltaTime);
    }
}
