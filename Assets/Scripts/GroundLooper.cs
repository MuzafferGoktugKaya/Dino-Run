using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    public Transform player;
    public Transform[] groundTiles;
    public float tileLength = 300f;

    void Update()
    {
        Transform nearestTile = groundTiles[0];
        Transform farthestTile = groundTiles[0];

        for (int i = 1; i < groundTiles.Length; i++)
        {
            if (groundTiles[i].position.z < nearestTile.position.z)
                nearestTile = groundTiles[i];

            if (groundTiles[i].position.z > farthestTile.position.z)
                farthestTile = groundTiles[i];
        }

        if (player.position.z > nearestTile.position.z + tileLength / 2f)
        {
            nearestTile.position = new Vector3(
                nearestTile.position.x,
                nearestTile.position.y,
                farthestTile.position.z + tileLength
            );
        }
    }
}