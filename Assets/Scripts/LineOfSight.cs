using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    //called when something enters the trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponentInParent<EnemyController>().player = collision.transform;
            Debug.Log("SEE PLAYER => RUN AT PLAYER");
        }
    }
}
