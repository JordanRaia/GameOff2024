using System.Collections;
using UnityEngine;

public abstract class BulletHellPattern : ScriptableObject
{
    public abstract IEnumerator ExecutePattern(GameObject player);
}