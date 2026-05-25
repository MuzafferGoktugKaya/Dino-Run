using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Runner/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Görsel Ayarlar")]
    public Material roadMaterial;
    public Material sideMaterial;
    public Material skyboxMaterial;

    [Header("Prefablar")]
    public GameObject coinPrefab;
    public GameObject ObstaclePrefab1;
    public GameObject ObstaclePrefab2;

    [Header("Hell Zone Special Coins")]
    public GameObject hellSpeedCoinPrefab;
    public GameObject hellJumpCoinPrefab;
    [Range(0f, 1f)] public float hellSpecialCoinChance = 0f;

    [Header("Spawn Yükseklikleri")]
    public float coinY = 1f;
    public float obstacle1Y = 0.5f;
    public float obstacle2Y = 2.2f;

    [Header("Oynanış Ayarları")]
    public float forwardSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1.0f;

    [Header("Sand Worm Ayarları")]
    public GameObject wormPrefab;
    public float wormSpawnChance = 0.15f;
}