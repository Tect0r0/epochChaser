using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager
{
    public static bool isVisible = true;
    public static bool throwDynamite = false;
    public static bool isDiscovered = false;
    public static void SetVisible()
    {
        isVisible = true;
    }

    public static void SetInvisible()
    {
        isVisible = false;
    }

    public static void ActivateDynamite()
    {
        throwDynamite = true;
    }

    public static void DeactivateDynamite()
    {
        throwDynamite = false;
    }

    public static void AlertOthers()
    {
        isDiscovered = true;
    }
    
}
