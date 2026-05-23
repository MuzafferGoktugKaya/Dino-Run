using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float destroyZ = -10f;

    private void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }
}