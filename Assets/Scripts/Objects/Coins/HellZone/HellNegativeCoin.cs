using UnityEngine;

public class HellNegativeCoin : MonoBehaviour
{
    [Header("Score Penalty")]
    public int scorePenalty = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.negativeCoinSFX);
            GameManager.Instance.RemoveScore(scorePenalty);
            
            GameManager.Instance.ShowNotification($"-{scorePenalty} SCORE!", Color.red);
        }

        Destroy(gameObject);
    }
}