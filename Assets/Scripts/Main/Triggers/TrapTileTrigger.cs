using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTileTrigger : MonoBehaviour
{
    private Vector3 InitPosition = Vector3.zero;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ Ʈ���� �������� ��
        if (other.CompareTag("Player"))
        {
            other.transform.position = InitPosition;
        }
    }
}
