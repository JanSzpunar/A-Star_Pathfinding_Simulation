using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct GridContentsData
{
    public Vector2Int Position;
    public TileData Data;
}
[RequireComponent(typeof(PathFindEventWrapper))]
public class TilesManager : MonoBehaviour
{

    public delegate void ValidTileClicked(Vector2Int Coordinates, Dictionary<Vector2Int, Tile> Tiles);
    public event ValidTileClicked OnValidTileClicked;

    public delegate void SpawnUnitRequested(Vector3 SpawnCoordinates, Vector2Int TileCoordinate);
    public event SpawnUnitRequested OnSpawnUnitRequested;

    public delegate void DestroyUnitRequested(Vector2Int TileCoordinate);
    public event DestroyUnitRequested OnDestroyUnitRequested;

    public delegate void UnitChangedPosition(Vector2Int UnitCoordinate, Unit Unit);
    public event UnitChangedPosition OnUnitChangedPosition;

    public delegate void VisualizePathRequested(Dictionary<Vector3, int> Path);
    public event VisualizePathRequested OnVisualizePathRequested;

    [SerializeField] private int Width = 5;
    [SerializeField] private int Height = 5;
    [SerializeField] private List<TileData> TylesVariantsInRespawnOrder = new();
    [SerializeField] private List<GridContentsData> GridContents = new();

    Dictionary<Vector2Int, Tile> TilesMap = new();

    public int GridWidth => Width;
    public int GridHeight => Height;

    private IEnumerator Start()
    {
        yield return null;
        GenerateGrid();
        GlobalDelegates.Instance.OnChangeTilesRowRequested += ChangeRow;
        GlobalDelegates.Instance.OnChangeTilesColumnsRequested += ChangeColumn;
    }

    void GenerateGrid()
    {
        Vector3 Center = Vector3.zero;
        foreach (GridContentsData Data in GridContents)
        {
            if (Data.Data)
            {
                Tile Tile = SpawnTile(Data);
                Center += Tile.transform.position;
            }
        }
        if (TilesMap.Count > 0)
        {
            Center /= TilesMap.Count;
        }

        GlobalDelegates.Instance.CameraRepositionRequested?.Invoke(new Vector2(Center.x, Center.z));
        GridContents.Clear();
    }
    void OnTileDestroyed(Tile DestroyedTile)
    {
        RemoveTile(DestroyedTile);
        DestroyedTile.TileClickedRequested -= OnTileClicked;
        DestroyedTile.UnitChangedPosition -= OnTileRequestedUnitChangePosition;
        DestroyedTile.TileDestroyed -= OnTileDestroyed;

    }
    private void OnTileClicked(Tile ClickedTile, int Button)
    {
        if (GameState.Instance.GetIsInConfigMode())
        {
            switch (Button)
            {
                case 0:
                    if(!ClickedTile.GetIsOccupied())
                    {
                        Vector2Int PreviousPosition = ClickedTile.GetGridPosition();
                        TileType PreviousType = ClickedTile.GetTileType();
                        foreach (var variant in TylesVariantsInRespawnOrder)
                        {
                            if (variant.Type == PreviousType)
                            {
                                int CurrentIndex = TylesVariantsInRespawnOrder.IndexOf(variant);
                                int NextIndex = CurrentIndex + 1 < TylesVariantsInRespawnOrder.Count ? CurrentIndex + 1 : 0;
                                RemoveTile(ClickedTile);
                                Destroy(ClickedTile.gameObject);
                                SpawnTile(new GridContentsData() { Position = PreviousPosition, Data = TylesVariantsInRespawnOrder[NextIndex] });
                                break;
                            }
                        }
                    }
                    break;

                case 1:
                    if (ClickedTile.GetIsOccupied())
                    {
                        OnDestroyUnitRequested.Invoke(ClickedTile.GetGridPosition());
                        ClickedTile.SetOccupied(false);
                    }
                    else if (ClickedTile.GetTileType() == TileType.Traversable)
                    {
                        OnSpawnUnitRequested.Invoke(ClickedTile.transform.position, ClickedTile.GetGridPosition());
                    }

                    break;

                default:
                    break;
            }

        }
        else
        {
            switch(Button)
            {
                case 0:
                    if (ClickedTile.GetTileType() != TileType.Obstacle)
                    {
                        OnValidTileClicked.Invoke(ClickedTile.GetGridPosition(), TilesMap);
                    }
                    break;

                default:
                    break;
            }
            

        }
    }

    private void OnTileRequestedUnitChangePosition(Vector2Int NewCoordinates, Unit PassedUnit)
    {
        OnUnitChangedPosition.Invoke(NewCoordinates, PassedUnit);
    }
    private void RemoveTile(Tile TileToRemove)
    {
        foreach (Tile Value in TilesMap.Values)
        {
            if(Value == TileToRemove)
            {
                TilesMap.Remove(TileToRemove.GetGridPosition());
                break;
            }
        }

    }
    private Tile SpawnTile(GridContentsData Data)
    {
        Vector3 SpawnPosition = new(Data.Position.x + transform.position.x, 0, Data.Position.y + transform.position.z);
        Vector2Int GridPosition = new(Data.Position.x, Data.Position.y);
        Tile Tile = Data.Data.SpawnTile(SpawnPosition, GridPosition);
        Tile.transform.SetParent(transform);
        TilesMap[GridPosition] = Tile;

        Tile.TileClickedRequested += OnTileClicked;
        Tile.TileDestroyed += OnTileDestroyed;
        Tile.UnitChangedPosition += OnTileRequestedUnitChangePosition;
        return Tile;
    }

    public void ConvertCoordinatesToWorld(Dictionary<Vector2Int, int> Path)
    {
        Dictionary<Vector3, int> WorldPath = new();
        foreach (KeyValuePair<Vector2Int, int> kvp in Path)
        {
            if (TilesMap.ContainsKey(kvp.Key))
            {
                WorldPath.Add(TilesMap[kvp.Key].transform.position, kvp.Value);
            }
        }
        OnVisualizePathRequested?.Invoke(WorldPath);
    }
    private void ChangeColumn(bool Add)
    {
        if(Add)
        {
            int newX = Width;

            for (int y = 0; y < Height; y++)
            {
                Vector2Int gridPos = new(Width, y);
                TileData tileData = TylesVariantsInRespawnOrder.FirstOrDefault(); 
                GridContentsData newData = new() { Position = gridPos, Data = tileData };
                SpawnTile(newData);
            }

            Width += 1;
        }
        else
        {
            if (Width <= 1) return;

            int targetX = Width - 1;

            List<Vector2Int> toRemove = new();
            foreach (var kvp in TilesMap)
            {
                if (kvp.Key.x == targetX)
                {
                    Destroy(kvp.Value.gameObject);
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var pos in toRemove)
            {
                TilesMap.Remove(pos);
            }

            Width -= 1;
        }

    }

    private void ChangeRow(bool Add)
    {
        if (Add)
        {
            int newY = Height;

            for (int x = 0; x < Width; x++)
            {
                Vector2Int gridPos = new(x, newY);
                TileData tileData = TylesVariantsInRespawnOrder.FirstOrDefault(); 
                GridContentsData newData = new() { Position = gridPos, Data = tileData };
                SpawnTile(newData);
            }

            Height += 1;
        }
        else
        {
            if (Height <= 1) return;

            int targetY = Height - 1;

            List<Vector2Int> toRemove = new();
            foreach (var kvp in TilesMap)
            {
                if (kvp.Key.y == targetY)
                {
                    Destroy(kvp.Value.gameObject);
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var pos in toRemove)
            {
                TilesMap.Remove(pos);
            }

            Height -= 1;
        }
    }


}








