using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target;
    [SerializeField, Range(0.5f, 5f)]
    private float m_CameraLerpSpeed;

    protected void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 newPosition = new Vector3(m_Target.transform.position.x, transform.position.y, m_Target.transform.position.z);
        transform.position = Vector3.LerpUnclamped(transform.position, newPosition, m_CameraLerpSpeed * Time.deltaTime);
    }
}