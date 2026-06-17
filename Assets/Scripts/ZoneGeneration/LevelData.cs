using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Runner/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Gorsel Ayarlar")]
    public Material roadMaterial;
    public Material sideMaterial;
    public Material skyboxMaterial;
    public Color themeColor = Color.white;

    [Header("Ses Ayarlari")]
    public AudioClip zoneBGM;

    [Header("Zone Giris Ayarlari")]
    public string displayName = "New Zone";
    [TextArea(2, 5)] public string firstTimeDescription;
    [TextArea(1, 3)] public string transitionMessage;

    [Header("Prefablar")]
    public GameObject coinPrefab;
    public GameObject ObstaclePrefab1;
    public GameObject ObstaclePrefab2;

    [Header("Hell Zone Special Coins")]
    public GameObject hellSpeedCoinPrefab;
    public GameObject hellJumpCoinPrefab;
    public GameObject hellNegativeCoinPrefab;
    [Range(0f, 1f)] public float hellSpecialCoinChance = 0f;
    [Range(0f, 1f)] public float hellNegativeCoinChance = 0f;

    [Header("Spawn Yukseklikleri")]
    public float coinY = 1f;
    public float obstacle1Y = 0.5f;
    public float obstacle2Y = 2.2f;

    [Header("Oynanis Ayarlari")]
    public float forwardSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1.0f;

    [Header("Sand Worm Ayarlari")]
    public GameObject wormPrefab;
    public float wormSpawnChance = 0.15f;
}
