using UnityEngine;

public class SandWorm : MonoBehaviour
{
    private bool hasTriggered = false;
    private Transform player;
    private float fixedLaneX; 

    [Header("Mesafe Ayarları")]
    public float activationDistance = 20f; 
    public float stopDistance = 1.0f;  

    [Header("Hareket Ayarları")]
    public float moveSpeed = 12f;      

    void Start()
    {
        fixedLaneX = transform.position.x;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        transform.rotation = Quaternion.Euler(0, 180, 0); 
    }

    void Update()
    {
        if (player == null) return;

        float distanceZ = Mathf.Abs(transform.position.z - player.position.z);

        if (!hasTriggered && distanceZ < activationDistance)
        {
            hasTriggered = true;
        }

        if (hasTriggered)
        {
            if (transform.position.z > player.position.z + stopDistance)
            {
                MoveInLane();
            }
        }
    }

    void MoveInLane()
    {
        Vector3 currentPos = transform.position;


        float newZ = currentPos.z - (moveSpeed * Time.deltaTime);

        transform.position = new Vector3(fixedLaneX, currentPos.y, newZ);
    }
}