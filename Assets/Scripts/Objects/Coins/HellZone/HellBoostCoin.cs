using UnityEngine;

public class HellBoostCoin : MonoBehaviour
{
    public enum BoostType
    {
        Speed,
        Jump
    }

    [Header("Score")]
    public int scoreValue = 1;

    [Header("Boost Settings")]
    public BoostType boostType = BoostType.Speed;
    public float boostMultiplier = 1.5f;
    public float boostDuration = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null) GameManager.Instance.RegisterCoinPickup(scoreValue);

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            if (boostType == BoostType.Speed)
            {
                playerMovement.ApplyTemporarySpeedBoost(boostMultiplier, boostDuration);
                
                GameManager.Instance.ShowNotification("SPEED BOOST!", Color.green);
            }
            else if (boostType == BoostType.Jump)
            {
                playerMovement.ApplyTemporaryJumpBoost(boostMultiplier, boostDuration);
                
                GameManager.Instance.ShowNotification("JUMP BOOST!", Color.green);
            }
        }
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.specialCoinSFX);
        Destroy(gameObject);
    }
}