using UnityEngine;
using AirZone.Weather;

[DisallowMultipleComponent]
public class AirMeatMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float returnZ = -10f;

    [Header("Weather Speed Effect")]
    [SerializeField] private float forwardWindSpeedBonus = 4f;
    [SerializeField] private float backwardWindSpeedPenalty = 4f;
    [SerializeField] private float minimumMoveSpeed = 1f;

    private AirWeatherManager weatherManager;

    private void Awake()
    {
        weatherManager = FindFirstObjectByType<AirWeatherManager>();
    }

    private void Update()
    {
        float finalMoveSpeed = GetWeatherAdjustedSpeed();

        transform.Translate(Vector3.back * finalMoveSpeed * Time.deltaTime, Space.World);

        if (transform.position.z <= returnZ)
        {
            ReturnToPoolOrDestroy();
        }
    }

    private float GetWeatherAdjustedSpeed()
    {
        float finalMoveSpeed = moveSpeed;

        if (weatherManager != null && weatherManager.CurrentEffect != null)
        {
            if (weatherManager.CurrentEffect.Type == AirWeatherType.ForwardWind)
            {
                finalMoveSpeed += forwardWindSpeedBonus;
            }
            else if (weatherManager.CurrentEffect.Type == AirWeatherType.BackwardWind)
            {
                finalMoveSpeed -= backwardWindSpeedPenalty;
            }
        }

        return Mathf.Max(minimumMoveSpeed, finalMoveSpeed);
    }

    private void ReturnToPoolOrDestroy()
    {
        if (TryGetComponent(out AirPooledObject pooledObject))
        {
            pooledObject.ReturnToPool();
            return;
        }

        Destroy(gameObject);
    }
}