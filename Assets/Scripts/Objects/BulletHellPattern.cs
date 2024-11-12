using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BulletHellPattern : ScriptableObject
{
    [SerializeField] protected bool isStatic = false; // Toggle for static targeting

    public bool IsStatic => isStatic;

    protected List<GameObject> activeBullets = new List<GameObject>();

    // Modify the method signature to accept a function to get targetPosition
    public abstract IEnumerator ExecutePattern(Func<Vector2> getTargetPosition);

    public virtual void StopPattern()
    {
        // Destroy all active bullets
        foreach (var bullet in activeBullets)
        {
            if (bullet != null)
                Destroy(bullet);
        }
        activeBullets.Clear();
    }
}