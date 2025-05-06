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
        halfHeight = mainCamera.orthographicSize; //Y���� ��-������ �ݰ�
        halfWidth = halfHeight * mainCamera.aspect;//������ ���ϸ� x���� ���� ���̰� ���� 

        Bounds bounds = tileMap.localBounds; //Ÿ�ϸ� ����
        minBounds = bounds.min;
        maxBounds = bounds.max;
    }

    private void LateUpdate()
    {
        if (target == null) return; //����ó��

        Vector3 position = target.position + offset;

        float x = Mathf.Clamp(position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float y = Mathf.Clamp(position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(x,y,offset.z);
    }
}
