using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isInvincible = false;
    public Color powerUpColor = Color.black;
    private Color originalColor;
    private SkinnedMeshRenderer meshRenderer;

    private CapsuleCollider playerCollider;
    private Rigidbody playerRigidbody;

    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider == null)
        {
            playerCollider = GetComponentInChildren<CapsuleCollider>();
        }

        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponentInChildren<Rigidbody>();
        }

        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isInvincible)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("Coin toplandı: " + other.gameObject.name);
        }

        if (other.CompareTag("Obstacle") && !isInvincible)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void StartInvincibility()
    {
        StopAllCoroutines(); 
        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        int playerLayer = gameObject.layer;
        int obstacleLayer = LayerMask.NameToLayer("Obstacle");

        if (obstacleLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, true);
        }
        else
        {
            Debug.LogWarning("Unity'de 'Obstacle' adında bir Layer bulunamadı! Karakterin pushlanmasını engellemek için lütfen katman atamasını yapın.");
        }
        
        if (meshRenderer != null) meshRenderer.material.color = powerUpColor;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartPowerUpAudio();
        }

        Debug.Log("Frenzy Modu BAŞLADI! Zıplama aktif, engeller yok sayılıyor.");

        yield return new WaitForSeconds(5f);

        isInvincible = false;

        if (obstacleLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
        }

        if (meshRenderer != null) meshRenderer.material.color = originalColor;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPowerUpAudio();
        }

        Debug.Log("Frenzy Modu BİTTİ! Engeller tekrar katı hale geldi.");
    }
}