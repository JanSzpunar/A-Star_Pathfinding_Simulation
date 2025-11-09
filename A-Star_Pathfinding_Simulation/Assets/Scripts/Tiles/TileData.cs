using UnityEngine;

public enum TileType { Traversable, Obstacle, Cover }

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public GameObject Prefab;

    public TileType Type;

    public Color Color = Color.white;

    public Tile SpawnTile(Vector3 SpawnPosition, Vector2Int GridPosition)
    {
        if (!Prefab)
        {
            Debug.LogError("No prefab assigned to TileData.");
            return null;
        }
        if (!Prefab.GetComponent<Tile>())
        {
            Debug.LogError("The prefab assigned to TileData does not have a Tile component.");
            return null;
        }

        GameObject TileGO = Instantiate(Prefab, SpawnPosition, Quaternion.identity);
        Tile Tile = TileGO.GetComponent<Tile>();
        Tile.InitializeTile(Type, GridPosition, Color);
        return Tile;
    }

}
