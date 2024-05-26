using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearHeat : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EnemyManager.isDiscovered)
        {
            EnemyManager.isDiscovered = false;
            EnemyManager.heatCleared = true;
            Debug.Log("Heat Cleared");
            StartCoroutine(ResetHeat());
        }
    }

    IEnumerator ResetHeat()
    {
        yield return new WaitForSeconds(2f);
        EnemyManager.heatCleared = false;
    }
}
