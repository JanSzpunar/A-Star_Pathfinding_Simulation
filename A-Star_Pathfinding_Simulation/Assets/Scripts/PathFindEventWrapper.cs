using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathFindEventWrapper : MonoBehaviour
{
    [SerializeField] private ValidTileClicked OnValidTileClicked;
    [SerializeField] private PathRequested OnPathRequested;
    [SerializeField] private PathFindReceived OnPathFound;
    [SerializeField] private PathValidated OnPathValidated;
    [SerializeField] private PathFinalized OnPathFinalized;

    [SerializeField] private SpawnUnitRequested OnSpawnUnitRequested;
    [SerializeField] private DestroyUnitRequested OnDestroyUnitRequested;
    [SerializeField] private UnitChangedPosition OnUnitChangedPosition;

    private void Awake()
    {
        TilesManager TilesManager = GetComponent<TilesManager>();
        TilesManager.OnValidTileClicked += ValidTileClicked;
        TilesManager.OnSpawnUnitRequested += SpawnUnitRequested;
        TilesManager.OnDestroyUnitRequested += DestoryUnitRequested;
        TilesManager.OnUnitChangedPosition += UnitChangedPosition;
        TilesManager.OnVisualizePathRequested += PathFinalized;
        UnitsManager UnitsManager = GetComponent<UnitsManager>();
        UnitsManager.OnPathRequested += PathRequested;
        UnitsManager.OnPathValidated += PathValidated;
        AStarPathfinder AStarPathfinder = GetComponent<AStarPathfinder>();
        AStarPathfinder.OnPathFound += PathFound;

    }
    private void ValidTileClicked(Vector2Int coordinates, Dictionary<Vector2Int, Tile> tiles)
    {
        OnValidTileClicked?.Invoke(coordinates, tiles);
    }

    private void SpawnUnitRequested(Vector3 spawnCoordinates, Vector2Int tileCoordinate)
    {
        OnSpawnUnitRequested?.Invoke(spawnCoordinates, tileCoordinate);
    }

    private void DestoryUnitRequested(Vector2Int tileCoordinate)
    {
        OnDestroyUnitRequested?.Invoke(tileCoordinate);
    }

    private void UnitChangedPosition(Vector2Int unitCoordinate, Unit unit)
    {
        OnUnitChangedPosition?.Invoke(unitCoordinate, unit);
    }

    private void PathFinalized(Dictionary<Vector3, int> Path)
    {
        OnPathFinalized?.Invoke(Path);
    }

    private void PathRequested(Vector2Int start, Vector2Int goal, Dictionary<Vector2Int, Tile> grid)
    {
        OnPathRequested?.Invoke(start, goal, grid);
    }

    private void PathValidated(Dictionary<Vector2Int, int> pathCost)
    {
        OnPathValidated?.Invoke(pathCost);
    }

    private void PathFound(List<Vector2Int> unitCoordinate)
    {
        OnPathFound?.Invoke(unitCoordinate);
    }
}

[Serializable]
public class ValidTileClicked : UnityEvent<Vector2Int, Dictionary<Vector2Int, Tile>> { }

[System.Serializable]
public class PathRequested : UnityEvent<Vector2Int, Vector2Int, Dictionary<Vector2Int, Tile>> { }

[System.Serializable]
public class PathFindReceived : UnityEvent<List<Vector2Int>> { }

[System.Serializable]
public class PathValidated : UnityEvent<Dictionary<Vector2Int, int>> { }

[Serializable]
public class PathFinalized : UnityEvent<Dictionary<Vector3, int>> { }

[Serializable]
public class SpawnUnitRequested : UnityEvent<Vector3, Vector2Int> { }

[Serializable]
public class DestroyUnitRequested : UnityEvent<Vector2Int> { }

[Serializable]
public class UnitChangedPosition : UnityEvent<Vector2Int, Unit> { }




