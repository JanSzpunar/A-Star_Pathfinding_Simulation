using UnityEngine;

public class GlobalDelegates : MonoBehaviour
{
    public static GlobalDelegates Instance { get; private set; }

    public delegate void OnCameraRepositionRequested(Vector2 NewPosition);
    public OnCameraRepositionRequested CameraRepositionRequested;

    public delegate void ChangeTilesRowRequested(bool Add);
    public ChangeTilesRowRequested OnChangeTilesRowRequested;

    public delegate void ChangeTilesColumnsRequested(bool Add);
    public ChangeTilesColumnsRequested OnChangeTilesColumnsRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
