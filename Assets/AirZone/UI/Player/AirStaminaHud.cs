using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AirStaminaHud : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AirStaminaController staminaController;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TMP_Text staminaText;

    [Header("Text")]
    [SerializeField] private string label = "STAMINA";
    [SerializeField] private bool showLabel = true;

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

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f;
            staminaSlider.value = normalized;
        }

        if (staminaText != null)
        {
            int displayedStamina = Mathf.CeilToInt(currentStamina);
            staminaText.text = showLabel ? $"{label} {displayedStamina}" : displayedStamina.ToString();
        }
    }
}