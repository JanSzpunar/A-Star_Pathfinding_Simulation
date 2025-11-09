using System;
using System.Collections;
using UnityEngine;

public class TileCamera : MonoBehaviour
{

    [SerializeField] float CameraHeight = 10f;
    [SerializeField] float MoveSpeed = 10f;

    private void OnCameraRepositionRequested(Vector2 NewPosition)
    {
        transform.position = new Vector3(NewPosition.x, CameraHeight, NewPosition.y);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalDelegates.Instance.CameraRepositionRequested += OnCameraRepositionRequested;
    }

    private void OnDestroy()
    {
        GlobalDelegates.Instance.CameraRepositionRequested -= OnCameraRepositionRequested;
    }

    // Update is called once per frame
    void Update()
    {
        float MoveX = Input.GetAxis("Horizontal");
        float MoveZ = Input.GetAxis("Vertical");

        Vector3 Movement = new Vector3(MoveX, 0, MoveZ);
        transform.Translate(Movement * Time.deltaTime * MoveSpeed, Space.World);
    }
}
