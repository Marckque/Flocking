using UnityEngine;

public class Controller : MonoBehaviour
{
    #region Variables
    [Header("Controller")]
    [SerializeField, Range(10f, 40f)]
    private float m_ControllerLerpSpeed;
    [SerializeField, Range(1f, 20f)]
    private float m_SlowVelocity;
    [SerializeField, Range(1f, 20f)]
    private float m_DefaultVelocity;
    [SerializeField, Range(1f, 20f)]
    private float m_FastVelocity;
    [SerializeField, Range(0.5f, 5f)]
    private float m_AttackCooldown;

    private float m_CurrentCooldown;

    [Header("Target")]
    [SerializeField]
    private Transform m_Target;
    [SerializeField, Range(10f, 40f)]
    private float m_TargetLerpSpeed;
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
        float inputX = Input.GetAxis("L_XAxis_1");
        float inputZ = Input.GetAxis("L_YAxis_1");

        Vector3 direction = new Vector3(inputX, 0, inputZ);

        if (direction != Vector3.zero)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, transform.position + (direction * m_CurrentVelocity * Time.deltaTime), m_ControllerLerpSpeed * Time.deltaTime);
        }
    }

    private void Formations()
    {
        if (!Input.GetButton("LB_1") || Input.GetAxisRaw("TL_1") == 0)
        {
            m_CurrentVelocity = m_DefaultVelocity;
        }

        if (Input.GetButton("LB_1"))
        {
            m_CurrentVelocity = m_SlowVelocity;
        }

        if (Input.GetAxisRaw("TL_1") > 0)
        {
            m_CurrentVelocity = m_FastVelocity;
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
            m_Target.transform.localPosition = Vector3.LerpUnclamped(m_Target.transform.localPosition, a_NewPosition, m_TargetLerpSpeed * Time.deltaTime);
        }
        else
        {
            if (m_Target.transform.position != Vector3.zero)
            {
                m_Target.transform.localPosition = Vector3.LerpUnclamped(m_Target.transform.localPosition, Vector3.zero, m_TargetLerpSpeed * Time.deltaTime);
            }
        }
    }

    private void Attack(Vector3 a_NewPosition)
    {
        if (m_CurrentCooldown > 0)
        {
            m_CurrentCooldown -= Time.deltaTime;
        }
        else 
        {
            if (Input.GetButtonDown("RB_1"))
            {
                m_CurrentCooldown = m_AttackCooldown;

                foreach (Boid boids in BoidsManager.Instance.Boids)
                {
                    boids.GetTrailRenderer.Clear();
                    boids.transform.position = (m_Target.transform.position + a_NewPosition * m_AttackRange);
                }
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