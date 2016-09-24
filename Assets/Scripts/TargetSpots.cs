using UnityEngine;

public class TargetSpots : MonoBehaviour
{
    [SerializeField]
    private float m_Radius;
    [SerializeField]
    private int m_NumberOfTargets;

    private Vector3[] m_TargetLocations;

    protected void Start()
    {
        m_TargetLocations = new Vector3[m_NumberOfTargets];
        Vector3 position = transform.position;

        for (int i = 0; i < m_TargetLocations.Length; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-m_Radius, m_Radius), 0, Random.Range(-m_Radius, m_Radius));
            m_TargetLocations[i] = new Vector3(position.x + offset.x, 0, position.z + offset.z);
        }
    }

    protected void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < m_TargetLocations.Length; i++)
            {
                Gizmos.DrawWireSphere(m_TargetLocations[i], 0.25f);
            }
        }
    }
}