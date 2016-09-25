using UnityEngine;

public class Moving : MonoBehaviour
{
    [SerializeField]
    private Transform m_StartPosition;
    [SerializeField]
    private Transform m_EndPosition;
    [SerializeField, Range(1f, 10f)]
    private float m_MovingLerpSpeed;

    private Transform m_CurrentDestination;

    protected void Start()
    {
        transform.position = m_StartPosition.position;

        m_CurrentDestination = m_EndPosition;
    }

    protected void Update()
    {
        Vector3 direction = (m_CurrentDestination.position - transform.position);
        float distance = direction.magnitude;

        transform.Translate(direction.normalized * m_MovingLerpSpeed * Time.deltaTime);

        if (distance <= m_MovingLerpSpeed * Time.deltaTime)
        {
            SetCurrentDestination();
        }
    }

    private void SetCurrentDestination()
    {
        bool newDestination = m_CurrentDestination == m_StartPosition ?
            m_CurrentDestination = m_EndPosition : m_CurrentDestination = m_StartPosition;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_StartPosition.position, 0.5f);
        Gizmos.DrawWireSphere(m_EndPosition.position, 0.5f);
    }
}