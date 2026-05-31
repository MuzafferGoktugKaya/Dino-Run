using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    public bool isInvincible = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isInvincible == false)
            {
                GameManager.Instance.GameOver();
            }
            else
            {

                Debug.Log("Ölümsüzsün, engele çarptın ama yanmadın!");
            }
        }
    }

    public void StartInvincibility()
    {
        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        Debug.Log("Ölümsüzlük BAŞLADI!");

        yield return new WaitForSeconds(5f);

        isInvincible = false;
        Debug.Log("Ölümsüzlük BİTTİ!");
    }
}