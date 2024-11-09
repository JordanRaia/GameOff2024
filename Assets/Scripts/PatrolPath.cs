using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            int k = GetNextIndex(i);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetWaypoint(i), 0.3f);
            Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(k));
        }
    }

    public int GetNextIndex(int i)
    {
        if (i + 1 == transform.childCount) return 0;
        return i + 1;
    }

    public Vector3 GetWaypoint(int i)
    {
        return transform.GetChild(i).position;
    }
}
