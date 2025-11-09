using UnityEngine;

[RequireComponent(typeof(UnitMovement))]
public abstract class Unit : MonoBehaviour
{
    [SerializeField] int DefaultMovementPoints = 5;
    [SerializeField] int AttackRange = 5;
    [SerializeField] int MoveRange = 5;
    int CurrentMovementPoints => DefaultMovementPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetAttackRange() {  return AttackRange; }
    public int GetMoveRange() { return MoveRange; }
    public int GetCurrentMovementPoints() { return CurrentMovementPoints; }

    public void IncDecAttackRange(bool Increment)
    {
        AttackRange += Increment ? 1 : -1;
    }

    public void IncDecMoveRange(bool Increment)
    {
        MoveRange += Increment ? 1 : -1;
    }
}
