using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isInvincible = false;
    public Color powerUpColor = Color.black; // Kararma rengi
    private Color originalColor;
    private SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        // Dinozorun modelindeki renderer'ı alıyoruz
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!isInvincible)
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
        // Eğer zaten aktifse coroutine'i durdurup yeniden başlatıyoruz
        StopAllCoroutines(); 
        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        
        // 1. Rengi değiştir
        if (meshRenderer != null) meshRenderer.material.color = powerUpColor;

        // 2. Müzik ve sesleri AudioManager üzerinden yönet
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartPowerUpAudio();
        }

        Debug.Log("Ölümsüzlük ve Frenzy Modu BAŞLADI!");

        // 5 saniye bekle
        yield return new WaitForSeconds(5f);

        // 3. Her şeyi normale döndür
        isInvincible = false;
        if (meshRenderer != null) meshRenderer.material.color = originalColor;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPowerUpAudio();
        }

        Debug.Log("Ölümsüzlük ve Frenzy Modu BİTTİ!");
    }
}