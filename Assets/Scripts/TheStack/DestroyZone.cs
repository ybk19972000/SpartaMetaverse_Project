using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Rubble")) //Rubble �� ������Ʈ�� �浹
        {
            Destroy(collision.gameObject); //������Ʈ �ı�
        }
    }
}
