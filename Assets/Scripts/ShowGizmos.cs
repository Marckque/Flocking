using UnityEngine;

public class ShowGizmos : MonoBehaviour
{
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
