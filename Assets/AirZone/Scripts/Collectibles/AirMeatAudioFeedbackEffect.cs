using UnityEngine;

[DisallowMultipleComponent]
public class AirMeatAudioFeedbackEffect : AirMeatEffectBase
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    public override void Apply(GameObject collector)
    {
        if (pickupClip == null)
        {
            return;
        }

        Vector3 playPosition = transform.position;

        if (Camera.main != null)
        {
            playPosition = Camera.main.transform.position;
        }

        AudioSource.PlayClipAtPoint(pickupClip, playPosition, volume);
    }
}