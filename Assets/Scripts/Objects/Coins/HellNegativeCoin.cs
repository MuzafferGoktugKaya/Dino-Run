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
            GameManager.Instance.RemoveScore(scorePenalty);
        }

        Destroy(gameObject);
    }
}