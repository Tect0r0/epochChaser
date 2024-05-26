using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTarget : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnemyManager.SetInvisible();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnemyManager.SetVisible();
        }
    }
}
