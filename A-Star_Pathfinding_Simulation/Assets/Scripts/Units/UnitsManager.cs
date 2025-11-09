using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PathFindEventWrapper))]
public class UnitsManager : MonoBehaviour       // This class could have been done better by separating Player and Enemy units management
{
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] UnitsBackendFrontend PlayerBackendFrontend;
    [SerializeField] UnitsBackendFrontend EnemyBackendFrontend;

    Dictionary<Unit, Vector2Int> PlayerUnits = new();
    Dictionary<Unit, Vector2Int> EnemyUnits = new();

    public delegate void PathRequested(Vector2Int Start, Vector2Int Goal, Dictionary<Vector2Int, Tile> Grid);
    public event PathRequested OnPathRequested;

    public delegate void PathValidated(Dictionary<Vector2Int, int> Path);
    public event PathValidated OnPathValidated;

    private void Awake()
    {
        PlayerBackendFrontend.OnUnitParametersChanged += (AttackRange, MoveRange, Index) =>
        {
            if (PlayerUnits.Count > 0)
            {
                var Unit = PlayerUnits.Keys.ToList()[Index];
                if (AttackRange != 0)
                {
                    bool Increment = AttackRange > 0;
                    Unit.IncDecAttackRange(Increment);
                }
                if (MoveRange != 0)
                {
                    bool Increment = MoveRange > 0;
                    Unit.IncDecMoveRange(Increment);
                }
            }
            else
            {
                Debug.LogWarning($"No player unit available to change parameters.");
            }
        };
        EnemyBackendFrontend.OnUnitParametersChanged += (AttackRange, MoveRange, Index) =>
        {
            if (EnemyUnits.Count> 0)
            {
                var Unit = EnemyUnits.Keys.ToList()[Index];
                if (AttackRange != 0) 
                {
                    bool Increment = AttackRange > 0;
                    Unit.IncDecAttackRange(Increment);
                }
                if (MoveRange != 0)
                {
                    bool Increment = MoveRange > 0;
                    Unit.IncDecMoveRange(Increment);
                }

            }
            else
            {
                Debug.LogWarning($"No enemy unit available to change parameters.");
            }
        };
    }
    public void SpawnUnit(Vector3 SpawnPostion, Vector2Int Coordinates)
    {
        if (PlayerUnits.Count == 0)
        {
            if (PlayerPrefab == null)
                return;
            if (PlayerPrefab.GetComponent<Unit>() == null)
                return;
            Unit Player = Instantiate(PlayerPrefab, SpawnPostion, Quaternion.identity).GetComponent<Unit>();
            PlayerUnits[Player] = Coordinates;
            PlayerBackendFrontend.OnUnitFrontendReceived?.Invoke(Player.GetAttackRange(), Player.GetMoveRange(), 0);
        }
        else if (EnemyUnits.Count == 0)
        {
            if (EnemyPrefab == null)
                return;
            if (PlayerPrefab.GetComponent<Unit>() == null)
                return;
            Unit Enemy = Instantiate(EnemyPrefab, SpawnPostion, Quaternion.identity).GetComponent<Unit>();
            EnemyUnits[Enemy] = Coordinates;
            EnemyBackendFrontend.OnUnitFrontendReceived?.Invoke(Enemy.GetAttackRange(), Enemy.GetMoveRange(), 0);
        }
        else
        {
            Debug.LogWarning("Both player and enemy units are already spawned.");
        }
    }

    public void DestroyUnit(Vector2Int Coordinates)
    {
        if (PlayerUnits.ContainsValue(Coordinates))
        {
            int index = 0;
            foreach (var unit in PlayerUnits)
            {
                
                if (unit.Value == Coordinates)
                {
                    Destroy(unit.Key.gameObject);
                    PlayerBackendFrontend.OnUnitFrontendLost?.Invoke(index);
                    PlayerUnits.Remove(unit.Key);
                    return;
                }
                index++;
            }
            
        }

        if (EnemyUnits.ContainsValue(Coordinates))
        {
            int index = 0;
            foreach (var unit in EnemyUnits)
            {
                if (unit.Value == Coordinates)
                {
                    Destroy(unit.Key.gameObject);
                    EnemyBackendFrontend.OnUnitFrontendLost?.Invoke(index);
                    EnemyUnits.Remove(unit.Key);
                    return;
                }
                index++;
            }

        }
    }   

    public void ChangeUnitPosition(Vector2Int NewCoordinates, Unit PassedUnit)
    {
        if (PlayerUnits.ContainsKey(PassedUnit))
        {
            PlayerUnits[PassedUnit] = NewCoordinates;
            return;
        }
        if (EnemyUnits.ContainsKey(PassedUnit))
        {
            EnemyUnits[PassedUnit] = NewCoordinates;
            return;
        }
    }
    public void RequestPath(Vector2Int Goal, Dictionary<Vector2Int, Tile> Grid)
    {
        if (PlayerUnits.Count == 0)
        {
            Debug.LogWarning("No player unit available to request a path.");
            return;
        }

        var PlayerPosition = PlayerUnits.Keys.First().transform.position;
        Vector2Int Start = new((int)PlayerPosition.x, (int)PlayerPosition.z);
        OnPathRequested.Invoke(Start, Goal, Grid);
    }

    public void PathValidate(List<Vector2Int> Path)
    {
        if (Path.Count == 0)
        {
            Debug.Log("No need to move");
            return;
        }
        if (PlayerUnits.Count == 0)
        {
            Debug.LogWarning("No player unit available to validate a path.");
            return;
        }
        Dictionary<Vector2Int, int> PathTemp = new();
        bool AttackPath = false;
        if (EnemyUnits.Count > 0 && EnemyUnits.Values.First() == Path[^1])
        {
            AttackPath = true;
        }
        if (AttackPath)
        {

            if (Path.Count <= PlayerUnits.Keys.First().GetAttackRange())
            {
                foreach (var point in Path)
                {
                    PathTemp[point] = 1;
                }
            }
            else
            {
                for (int i = PlayerUnits.Keys.First().GetAttackRange() + 1; i < Path.Count; i++)
                {
                    PathTemp[Path[i]] = 2;
                }
                for (int i = 0; i <= PlayerUnits.Keys.First().GetAttackRange(); i++)
                {
                    PathTemp[Path[i]] = 1;
                }
            }
        }
        else
        {
            if (Path.Count <= PlayerUnits.Keys.First().GetMoveRange())
            {
                foreach (var point in Path)
                {
                    PathTemp[point] = 0;
                }

            }
            else
            {
                for (int i = PlayerUnits.Keys.First().GetMoveRange()+1; i < Path.Count; i++)
                {
                    PathTemp[Path[i]] = 2;
                }
                for (int i = 0; i <= PlayerUnits.Keys.First().GetMoveRange(); i++)
                {
                    PathTemp[Path[i]] = 0;
                }

            }
        }
        OnPathValidated.Invoke(PathTemp);
    }
}


