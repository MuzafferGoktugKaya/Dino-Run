using UnityEngine;

public class DestroyBehindPlayer : MonoBehaviour
{
    public float destroyDistance = 30f;
    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;

        if (transform.position.z < player.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}