using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float m_LerpVelocity;
    [SerializeField]
    private Transform m_Target;

    protected void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 newPosition = new Vector3(m_Target.transform.position.x, transform.position.y, m_Target.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, m_LerpVelocity * Time.deltaTime);
    }
}