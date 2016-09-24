using UnityEngine;

public class Controller : MonoBehaviour
{
    #region Variables
    [Header("Character")]
    [SerializeField, Range(1f, 20f)]
    private float m_SlowVelocity;
    [SerializeField, Range(1f, 20f)]
    private float m_DefaultVelocity;
    [SerializeField, Range(1f, 20f)]
    private float m_FastVelocity;
    [Header("Target")]
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private float m_TargetSpeed;
    [SerializeField, Range(1f, 20f)]
    private float m_DistanceModifier;
    [SerializeField, Range(1f, 20f)]
    private float m_AttackRange;

    private float m_CurrentVelocity;
    #endregion Variables

    #region CheckControls
    protected void Update()
    {
        CheckControls();
	}

    private void CheckControls()
    {
        ControllerControls();
        TargetControls();
    }

    private void ControllerControls()
    {
        Movement();
        Formations();
    }

    private void TargetControls()
    {
        Aim();
    }
    #endregion CheckControls

    #region ControllerControls
    private void Movement()
    {
        float inputX = Input.GetAxisRaw("L_XAxis_1");
        float inputZ = Input.GetAxisRaw("L_YAxis_1");

        Vector3 direction = new Vector3(inputX, 0, inputZ);

        if (direction != Vector3.zero)
        {
            direction.Normalize();
            transform.Translate(direction * m_CurrentVelocity * Time.deltaTime);
        }
    }

    private void Formations()
    {
        if (!Input.GetButton("LB_1"))
        {
            m_CurrentVelocity = m_DefaultVelocity;
        }

        if (Input.GetButton("LB_1"))
        {
            m_CurrentVelocity = m_SlowVelocity;
        }
    }
    #endregion ControllerControls

    #region TargetControls
    private void Aim()
    {
        float inputX = Input.GetAxis("R_XAxis_1");
        float inputZ = -Input.GetAxis("R_YAxis_1");
        Vector3 newPosition = new Vector3(inputX, 0, inputZ);

        MoveTarget(newPosition);
        Attack(newPosition);
    }

    private void MoveTarget(Vector3 a_NewPosition)
    {
        if (a_NewPosition != Vector3.zero)
        {
            a_NewPosition *= m_DistanceModifier;
            m_Target.transform.localPosition = Vector3.LerpUnclamped(m_Target.transform.localPosition, a_NewPosition, m_TargetSpeed * Time.deltaTime);
        }
        else
        {
            if (m_Target.transform.position != Vector3.zero)
            {
                m_Target.transform.localPosition = Vector3.LerpUnclamped(m_Target.transform.localPosition, Vector3.zero, m_TargetSpeed * Time.deltaTime);
            }
        }
    }

    private void Attack(Vector3 a_NewPosition)
    {
        if (Input.GetButtonDown("RB_1"))
        {
            foreach (Boid boids in BoidsManager.Instance.Boids)
            {
                boids.GetTrailRenderer.Clear();
                boids.transform.position = (m_Target.transform.position + a_NewPosition * m_AttackRange);
            }
        }
    }
    #endregion TargetControls

    private void OnDrawGizmos()
    {
        // Draw attack sphere
        float inputX = Input.GetAxis("R_XAxis_1");
        float inputZ = -Input.GetAxis("R_YAxis_1");
        Vector3 newPosition = new Vector3(inputX, 0, inputZ);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(m_Target.transform.position + newPosition * m_AttackRange, 0.5f);
    }
}