using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AirGrowthAudioFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AirGrowthController growthController;

    [Header("Audio")]
    [SerializeField] private AudioClip growthClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    private AudioSource audioSource;
    private int lastGrowthStage = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (growthController == null)
        {
            growthController = GetComponent<AirGrowthController>();
        }

        if (growthController == null)
        {
            growthController = GetComponentInParent<AirGrowthController>();
        }

        if (growthController == null)
        {
            growthController = FindFirstObjectByType<AirGrowthController>();
        }
    }

    private void OnEnable()
    {
        if (growthController != null)
        {
            growthController.OnGrowthStageChanged += HandleGrowthStageChanged;
        }
    }

    private void OnDisable()
    {
        if (growthController != null)
        {
            growthController.OnGrowthStageChanged -= HandleGrowthStageChanged;
        }
    }

    private void HandleGrowthStageChanged(int growthStage)
    {
        if (lastGrowthStage == -1)
        {
            lastGrowthStage = growthStage;
            return;
        }

        if (growthStage == lastGrowthStage)
        {
            return;
        }

        lastGrowthStage = growthStage;

        if (growthClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(growthClip, volume);
    }
}