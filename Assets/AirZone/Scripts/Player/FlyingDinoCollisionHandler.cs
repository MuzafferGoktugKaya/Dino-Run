using UnityEngine;
using UnityEngine.UI;

public class FlyingDinoCollisionHandler : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Health UI")]
    [SerializeField] private Image healthFillImage;

    private int currentHealth;
    private bool isGameOver = false;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Health: " + currentHealth);

        UpdateHealthBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
            return;

        if (other.CompareTag("AirObstacle"))
        {
            Debug.Log("hit");

            currentHealth--;
            Debug.Log("Health: " + currentHealth);

            UpdateHealthBar();

            AirPooledObject pooledObject = other.GetComponentInParent<AirPooledObject>();

            if (pooledObject != null)
            {
                pooledObject.ReturnToPool();
            }
            else
            {
                Destroy(other.gameObject);
            }

            if (currentHealth <= 0)
            {
                Debug.Log("Game Over");
                Time.timeScale = 0f;
                isGameOver = true;
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage == null)
            return;

        float normalizedHealth = maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = normalizedHealth;
    }
}