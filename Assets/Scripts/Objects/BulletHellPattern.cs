using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletHellPattern : ScriptableObject
{
    protected List<GameObject> activeBullets = new List<GameObject>();

    public abstract IEnumerator ExecutePattern(GameObject player);

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