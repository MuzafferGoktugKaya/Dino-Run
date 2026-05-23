using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatCollectible : MonoBehaviour
{
    [Header("Collectible")]
    [SerializeField] private int meatAmount = 1;

    private bool isCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;

        IAirMeatConsumer meatConsumer = other.GetComponentInParent<IAirMeatConsumer>();

        if (meatConsumer == null)
            return;

        isCollected = true;
        meatConsumer.AddMeat(meatAmount);

        Destroy(gameObject);
    }
}