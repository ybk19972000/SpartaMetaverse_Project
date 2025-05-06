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
        halfHeight = mainCamera.orthographicSize; //Y축의 반-세로의 반값
        halfWidth = halfHeight * mainCamera.aspect;//비율을 곱하면 x축의 반쪽 길이가 나옴 

        Bounds bounds = tileMap.localBounds; //타일맵 범위
        minBounds = bounds.min;
        maxBounds = bounds.max;
    }

    private void LateUpdate()
    {
        if (target == null) return; //예외처리

        Vector3 position = target.position + offset;

        float x = Mathf.Clamp(position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float y = Mathf.Clamp(position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(x,y,offset.z);
    }
}
