using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    [SerializeField] bool IsInConfigMode = true;

    [SerializeField] GameStateChanged OnGameStateChanged;

    public static GameState Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        OnGameStateChanged.Invoke(IsInConfigMode);
    }

    public void SetIsInConfigMode(bool NewInConfigMode)
    {
        if (IsInConfigMode != NewInConfigMode)
        {
            IsInConfigMode = NewInConfigMode;
            OnGameStateChanged.Invoke(IsInConfigMode);
            
        }
    }

    public bool GetIsInConfigMode() => IsInConfigMode;
}

[Serializable]
public class GameStateChanged : UnityEvent<bool> { }

