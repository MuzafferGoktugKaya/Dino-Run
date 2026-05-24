using UnityEngine;

public class FlyingDinoCollisionHandler : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;
    private bool isGameOver = false;

    private void Start()
    {
        // Initialize player health at the start of the game.
        currentHealth = maxHealth;
        Debug.Log("Health: " + currentHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions after the game is over.
        if (isGameOver)
            return;
        // Check if the collided object is an air obstacle.
        if (other.CompareTag("AirObstacle"))
        {
            Debug.Log("hit");
            // Reduce health when the player hits an obstacle.
            currentHealth--;
            Debug.Log("Health: " + currentHealth);

            Destroy(other.gameObject);
            // End the game when health reaches zero.
            if (currentHealth <= 0)
            {
                Debug.Log("Game Over");
                Time.timeScale = 0f;
                isGameOver = true;
            }
        }
    }
}