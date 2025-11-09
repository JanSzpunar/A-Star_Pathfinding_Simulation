using UnityEngine;

public class UIBase : MonoBehaviour
{
    public void SetConfigGameState()
    {
        GameState.Instance.SetIsInConfigMode(true);
    }

    public void SetGameplayGameState()
    {
        GameState.Instance.SetIsInConfigMode(false);
    }

    public void RequestAddRow()
    {
        GlobalDelegates.Instance.OnChangeTilesRowRequested?.Invoke(true);
    }

    public void RequestRemoveRow()
    {
        GlobalDelegates.Instance.OnChangeTilesRowRequested?.Invoke(false);
    }

    public void RequestAddColumn()
    {
        GlobalDelegates.Instance.OnChangeTilesColumnsRequested?.Invoke(true);
    }

    public void RequestRemoveColumn()
    {
        GlobalDelegates.Instance.OnChangeTilesColumnsRequested?.Invoke(false);
    }
}
