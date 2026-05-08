using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    public Transform player;
    public Transform[] groundTiles; 
    public LevelData currentLevel;
    public float tileLength = 300f;

    void Start() => ApplyLevelMaterial();

    void Update()
    {
        if (player == null || groundTiles.Length == 0 || currentLevel == null) return;

        Transform nearestTile = groundTiles[0];
        Transform farthestTile = groundTiles[0];

        foreach (Transform t in groundTiles)
        {
            if (t.position.z < nearestTile.position.z) nearestTile = t;
            if (t.position.z > farthestTile.position.z) farthestTile = t;
        }

        if (player.position.z > nearestTile.position.z + tileLength / 2f)
        {
            nearestTile.position = new Vector3(nearestTile.position.x, nearestTile.position.y, farthestTile.position.z + tileLength);
            UpdateTileMaterial(nearestTile);
        }
    }

    public void ApplyLevelMaterial()
    {
        if (currentLevel == null) return;
        foreach (Transform tile in groundTiles) UpdateTileMaterial(tile);
    }

private void UpdateTileMaterial(Transform tile)
{
    if (tile == null || currentLevel == null) return;

    // 1. Ana objenin (yanların) materyalini değiştir
    Renderer mainRend = tile.GetComponent<Renderer>();
    if (mainRend != null) mainRend.material = currentLevel.sideMaterial;

    // 2. Altındaki "RoadStrip" objesini bul ve onun materyalini de değiştir
    // Transform.Find kullanırken ismin hiyerarşidekiyle (RoadStrip) tam eşleştiğinden emin ol
    Transform roadStrip = tile.Find("RoadStrip"); 
    if (roadStrip != null)
    {
        Renderer roadRend = roadStrip.GetComponent<Renderer>();
        if (roadRend != null) roadRend.material = currentLevel.roadMaterial;
    }
}
}