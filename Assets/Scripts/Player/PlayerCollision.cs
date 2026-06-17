using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isInvincible = false;
    public Color powerUpColor = Color.black;
    public int maxHealth = 3;
    public float hitInvincibilityDuration = 1.25f;
    public float powerUpDuration = 5f;
    private int currentHealth;
    private bool shieldActive;
    private Color originalColor;
    private SkinnedMeshRenderer meshRenderer;

    private CapsuleCollider playerCollider;
    private Rigidbody playerRigidbody;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        if (playerCollider == null) playerCollider = GetComponentInChildren<CapsuleCollider>();

        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null) playerRigidbody = GetComponentInChildren<Rigidbody>();

        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null) originalColor = meshRenderer.material.color;

        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null) playerMovement = GetComponentInChildren<PlayerMovement>();

        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TakeHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            TakeHit();
        }
    }

    public void TakeHit()
    {
        if (isInvincible) return;

        if (shieldActive)
        {
            shieldActive = false;
            StartCoroutine(HitGraceRoutine());
            NotifyHealthChanged();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetCombo();
                GameManager.Instance.ShowNotification("SHIELD BLOCKED HIT!", Color.cyan);
            }
            return;
        }

        currentHealth--;
        NotifyHealthChanged();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetCombo();
        }

        if (currentHealth <= 0)
        {
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
            return;
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayBonkSFX();
        if (GameManager.Instance != null) GameManager.Instance.ShowNotification("HIT! " + currentHealth + " HP LEFT", Color.red);
        StartCoroutine(HitGraceRoutine());
    }

    public void StartInvincibility()
    {
        StopAllCoroutines();
        StartCoroutine(InvincibilityRoutine());
    }

    public void ActivateShield()
    {
        shieldActive = true;
        NotifyHealthChanged();
        if (GameManager.Instance != null) GameManager.Instance.ShowNotification("SHIELD READY!", Color.cyan);
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        if (playerMovement != null)
        {
            playerMovement.EndSlide(); 
            playerMovement.canSlide = false; 
        }

        int playerLayer = gameObject.layer;
        int obstacleLayer = LayerMask.NameToLayer("Obstacle");

        if (obstacleLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, true);
        }
        else
        {
            Debug.LogWarning("Unity'de 'Obstacle' adında bir Layer bulunamadı! Lütfen katman atamasını kontrol edin.");
        }
        
        if (meshRenderer != null) meshRenderer.material.color = powerUpColor;

        if (AudioManager.Instance != null) AudioManager.Instance.StartPowerUpAudio();

        Debug.Log("Frenzy Modu BAŞLADI! Slide kilitlendi, engeller yok sayılıyor.");

        yield return new WaitForSeconds(powerUpDuration);

        isInvincible = false;

        if (playerMovement != null)
        {
            playerMovement.canSlide = true;
        }

        if (obstacleLayer != -1)
        {
            Physics.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
        }

        if (meshRenderer != null) meshRenderer.material.color = originalColor;

        if (AudioManager.Instance != null) AudioManager.Instance.StopPowerUpAudio();

        Debug.Log("Frenzy Modu BITTI! Slide acildi, engeller tekrar kati.");
        NotifyHealthChanged();
    }

    private IEnumerator HitGraceRoutine()
    {
        isInvincible = true;
        if (meshRenderer != null) meshRenderer.material.color = Color.red;

        yield return new WaitForSeconds(hitInvincibilityDuration);

        isInvincible = false;
        if (meshRenderer != null) meshRenderer.material.color = originalColor;
    }

    private void NotifyHealthChanged()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth, shieldActive);
        }
    }
}