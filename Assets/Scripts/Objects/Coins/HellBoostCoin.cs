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

        GameManager.Instance.AddScore(scoreValue);

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            if (boostType == BoostType.Speed)
            {
                playerMovement.ApplyTemporarySpeedBoost(boostMultiplier, boostDuration);
            }
            else if (boostType == BoostType.Jump)
            {
                playerMovement.ApplyTemporaryJumpBoost(boostMultiplier, boostDuration);
            }
        }

        Destroy(gameObject);
    }
}