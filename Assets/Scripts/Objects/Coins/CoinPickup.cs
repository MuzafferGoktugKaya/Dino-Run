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
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUpSFX);
                    playerCol.StartInvincibility();
                }
            }
            else
            {

                GameManager.Instance.AddScore(scoreValue);
            }

            AudioManager.Instance.PlaySFX(AudioManager.Instance.coinSFX);
            Destroy(gameObject);
        }
    }
}