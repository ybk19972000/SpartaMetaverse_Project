using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Rubble")) //Rubble 들어간 오브젝트의 충돌
        {
            Destroy(collision.gameObject); //오브젝트 파괴
        }
    }
}
