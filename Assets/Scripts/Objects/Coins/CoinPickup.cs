using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int scoreValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerCollision playerCol = other.GetComponent<PlayerCollision>();


            if (gameObject.CompareTag("BlackCoin"))
            {
                if (playerCol != null)
                {
                    if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUpSFX);
                    playerCol.StartInvincibility();
                }
            }
            else
            {

                if (GameManager.Instance != null) GameManager.Instance.RegisterCoinPickup(scoreValue);
            }

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.coinSFX);
            Destroy(gameObject);
        }
    }
}