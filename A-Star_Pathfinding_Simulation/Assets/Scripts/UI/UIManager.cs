using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIGameplay;
    [SerializeField] private GameObject UIConfig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void SwapUI(bool IsInConfigMode)
    {
        UIGameplay.SetActive(!IsInConfigMode);
        UIConfig.SetActive(IsInConfigMode);
    }
}
