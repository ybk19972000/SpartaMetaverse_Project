using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private Tilemap tileMap;

    private Vector2 minBounds; 
    private Vector2 maxBounds;

    private Camera mainCamera;

    private float halfHeight;
    private float halfWidth;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateCameraBound();
    }

    private void LateUpdate()
    {
        if (target == null) return; //예외처리

        Vector3 position = target.position + offset;

        float x = Mathf.Clamp(position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float y = Mathf.Clamp(position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(x,y,offset.z);
    }

    public void UpdateCameraBound()
    {
        if (target == null) return;

        halfHeight = mainCamera.orthographicSize;
        halfWidth = halfHeight * mainCamera.aspect;

        TilemapRenderer tilemapRenderer = tileMap.GetComponent<TilemapRenderer>();

        if (tilemapRenderer != null)
        {
            Bounds bounds = tilemapRenderer.bounds;

            minBounds = bounds.min;
            maxBounds = bounds.max;

        }
    }

    public void SetTargetAndTileMap(Transform newTarget, Tilemap newTileMap)
    {
        target = newTarget;
        tileMap = newTileMap;
        UpdateCameraBound();
    }
}
