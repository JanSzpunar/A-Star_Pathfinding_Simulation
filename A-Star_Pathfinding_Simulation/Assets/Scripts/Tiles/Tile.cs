using UnityEngine;

public abstract class Tile : MonoBehaviour, IClickable
{
    protected bool IsOccupied;
    protected TileType Type;
    protected Renderer Renderer;
    protected Vector2Int GridPosition;

    public delegate void OnClickedRequested(Tile TileClicked, int Button);
    public event OnClickedRequested TileClickedRequested;

    public delegate void OnTileDestroyed(Tile TileDestroyed);
    public event OnTileDestroyed TileDestroyed;

    public delegate void OnUnitChangedPosition(Vector2Int NewCoordinates, Unit PassedUnit);
    public event OnUnitChangedPosition UnitChangedPosition;


    private void Awake()
    {
        if (GetComponentInChildren<Renderer>() != null)
        {
            Renderer = GetComponentInChildren<Renderer>();
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit)
        {
            SetOccupied(true);
            UnitChangedPosition?.Invoke(GridPosition, unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>())
        {
            SetOccupied(false);
        }
    }


    private void OnDestroy()
    {
        TileDestroyed?.Invoke(this);
    }

    public void InitializeTile(TileType InitialType,Vector2Int InitialGridPosition, Color InitialColor)
    {
        Type = InitialType;
        GridPosition = InitialGridPosition;
        if (Renderer != null)
        {
            Renderer.material.color = InitialColor;
        }
            
    }

    public void OnClickedLeft()
    {
        TileClickedRequested?.Invoke(this, 0);
    }

    public void OnClickedRight()
    {
        TileClickedRequested?.Invoke(this, 1);
    }


    public bool GetIsValidForPathFinding() => Type == TileType.Traversable;

    public Vector2Int GetGridPosition() => GridPosition;

    public TileType GetTileType() => Type;

    public void SetOccupied(bool NewIsOccupied) => IsOccupied = NewIsOccupied;

    public bool GetIsOccupied() => IsOccupied;


}

