using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AirStaminaHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AirStaminaController staminaController;
    [SerializeField] private Image staminaFillImage;

    private void Awake()
    {
        if (staminaController == null)
        {
            staminaController = FindFirstObjectByType<AirStaminaController>();
        }
    }

    private void OnEnable()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaChanged += HandleStaminaChanged;
        }
    }

    private void OnDisable()
    {
        if (staminaController != null)
        {
            staminaController.OnStaminaChanged -= HandleStaminaChanged;
        }
    }

    private void Start()
    {
        if (staminaController != null)
        {
            UpdateHud(staminaController.CurrentStamina, staminaController.MaxStamina);
        }
    }

    private void HandleStaminaChanged(float currentStamina, float maxStamina)
    {
        UpdateHud(currentStamina, maxStamina);
    }

    private void UpdateHud(float currentStamina, float maxStamina)
    {
        float normalized = maxStamina <= 0f ? 0f : currentStamina / maxStamina;

        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = normalized;
        }
    }
}