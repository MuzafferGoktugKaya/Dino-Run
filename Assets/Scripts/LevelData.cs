using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Runner/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Görsel Ayarlar")]
    public Material roadMaterial;
    public Material sideMaterial;

    [Header("Prefablar")]
    public GameObject coinPrefab;
    public GameObject ObstaclePrefab1;
    public GameObject ObstaclePrefab2;

    [Header("Spawn Yükseklikleri")]
    public float coinY = 1f;
    public float jumpObstacleY = 0.5f;
    public float slideObstacleY = 2.2f;

    [Header("Oynanış Ayarları")]
    public float forwardSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1.0f;
}