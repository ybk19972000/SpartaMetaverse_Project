using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTileTrigger : MonoBehaviour
{
    private Vector3 InitPosition = Vector3.zero;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트랩에 접촉했을 때
        if (other.CompareTag("Player"))
        {
            other.transform.position = InitPosition;
        }
    }
}
