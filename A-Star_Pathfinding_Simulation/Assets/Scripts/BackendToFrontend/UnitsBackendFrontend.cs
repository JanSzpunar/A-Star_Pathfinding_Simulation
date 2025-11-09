using UnityEngine;

[CreateAssetMenu(fileName = "UnitsBackendFrontend", menuName = "Scriptable Objects/UnitsBackendFrontend")]
public class UnitsBackendFrontend : ScriptableObject
{
    public delegate void UnitFrontendReceived(int AttackRange, int MoveRange, int Index);
    public UnitFrontendReceived OnUnitFrontendReceived;

    public delegate void UnitFrontendLost(int Index);
    public UnitFrontendLost OnUnitFrontendLost;

    public delegate void UnitParamretersChanged(int AttackRange, int MoveRange, int Index);
    public UnitParamretersChanged OnUnitParametersChanged;

    public void IncDecUnitAttackRange(int AttackRange)
    {
        OnUnitParametersChanged.Invoke(AttackRange, 0, 0);
    }

    public void IncDecUnitMoveRange(int MoveRange)
    {
        OnUnitParametersChanged.Invoke(0, MoveRange, 0);
    }
}
