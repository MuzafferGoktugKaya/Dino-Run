using UnityEngine;

public class AirObstacle : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float destroyZ = -15f;

    private void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }
}