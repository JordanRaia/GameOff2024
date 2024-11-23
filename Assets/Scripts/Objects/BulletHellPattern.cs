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

    // New coroutine to monitor and destroy out-of-bounds bullets
    public IEnumerator MonitorBullets()
    {
        while (true)
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                var bullet = activeBullets[i];
                if (bullet != null)
                {
                    Vector3 pos = bullet.transform.position;
                    if (!IsWithinScreenBounds(pos))
                    {
                        Destroy(bullet);
                        activeBullets.RemoveAt(i);
                    }
                }
            }
            yield return new WaitForSeconds(1f); // Check every second
        }
    }

    // Helper method to determine if a position is within the screen bounds
    private bool IsWithinScreenBounds(Vector3 position)
    {
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(position);
        return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;
    }

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