using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    #region Variables    
    // Movement
    private float m_AccelerationFactor;
    private float m_DecelerationFactor;
    private float m_MaxSteeringForce;
    private float m_MaxVelocity;
    private float m_CurrentMaxSpeed;

    // Behavior modifiers
    private float m_ArriveFactor;
    private float m_AvoidanceFactor;
    private float m_MinimumDistanceToTarget;
    private float m_MinimumDistanceToOtherBoid;
    
    // Other
    public enum CurrentBehaviour {None, Arrive, Flow, Path};
    private CurrentBehaviour m_CurrentBehaviour;
    private Transform m_Target;
    private Vector3 m_Acceleration;
    private Vector3 m_CurrentVelocity;

    private Vector3 m_DesiredVelocity;
    #endregion Variables

    #region PublicGetters
    public TrailRenderer GetTrailRenderer { get; set; }
    #endregion PublicGetters

    #region DefineVariables
    public void SetMovementModifiers(float a_AccelerationFactor, float a_DecelerationFactor, float a_MaxVelocity, float a_MaxSteeringForce)
    {
        m_AccelerationFactor = a_AccelerationFactor;
        m_DecelerationFactor = a_DecelerationFactor;
        m_MaxVelocity = a_MaxVelocity;
        m_MaxSteeringForce = a_MaxSteeringForce;
    }

    public void SetBehaviorModifiers(float a_MinimumDistanceToTarget, float a_AvoidanceFactor, float a_MinimumDistanceToOtherBoid, float a_ArriveFactor)
    {
        m_MinimumDistanceToTarget = a_MinimumDistanceToTarget;
        m_AvoidanceFactor = a_AvoidanceFactor;
        m_MinimumDistanceToOtherBoid = a_MinimumDistanceToOtherBoid;
        m_ArriveFactor = a_ArriveFactor;
    }

    public void SetCurrentBehaviour(int a_CurrentBehaviour)
    {
        switch(a_CurrentBehaviour)
        {
            case 0:
                m_CurrentBehaviour = CurrentBehaviour.None;
                break;

            case 1:
                m_CurrentBehaviour = CurrentBehaviour.Arrive;
                break;

            case 2:
                m_CurrentBehaviour = CurrentBehaviour.Flow;
                break;

            case 3:
                m_CurrentBehaviour = CurrentBehaviour.Path;
                break;

            default:
                m_CurrentBehaviour = CurrentBehaviour.Arrive;
                break;
        }
    }

    public void SetTarget(Transform a_Target)
    {
        m_Target = a_Target;
    }
    #endregion DefineVariables

    protected void Start()
    {
        GetTrailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    protected void Update()
    {
        UpdatePosition();
	}

    private void OnTriggerEnter(Collider a_Collider)
    {
        BoidKiller boidKiller = a_Collider.GetComponent<BoidKiller>();

        if (boidKiller != null)
        {
            BoidsManager.Instance.Boids.Remove(this);
            Destroy(gameObject);
        }
    }

    /*
    private void UpdateCurrentBehaviour()
    {
        switch(m_CurrentBehaviour)
        {
            case CurrentBehaviour.Arrive:
                break;

            case CurrentBehaviour.Flow:
                break;

            case CurrentBehaviour.Path:
                break;

            case CurrentBehaviour.None:
            default:
                break;
        }
    }
    */

    #region VelocityCalculation
    private void UpdatePosition()
    {
        m_CurrentVelocity += m_Acceleration;
        Vector3.ClampMagnitude(m_CurrentVelocity, m_MaxVelocity);
        transform.position = transform.position + m_CurrentVelocity;

        m_Acceleration = Vector3.zero;
    }

    private void UpdateAcceleration(Vector3 a_Force)
    {
        m_Acceleration += a_Force * m_AccelerationFactor;
    }
    #endregion VelocityCalculation

    #region Behaviors
    public void UpdateBehaviour(List<Boid> a_Boids)
    {
        if (m_CurrentBehaviour == CurrentBehaviour.None)
        {
            if (m_CurrentVelocity != Vector3.zero)
            {
                m_CurrentVelocity = Vector3.Lerp(m_CurrentVelocity, Vector3.zero, m_DecelerationFactor * Time.deltaTime);
            }
        }   

        if (m_CurrentBehaviour == CurrentBehaviour.Arrive)
        {
            UpdateAcceleration(AvoidOtherBoids(a_Boids));
            UpdateAcceleration(Arrive());
        }   
    }

    private Vector3 Arrive()
    {
        Vector3 targetDirection = m_Target.transform.position - transform.position;
        float distanceToTarget = targetDirection.sqrMagnitude;
        m_CurrentMaxSpeed = 0;
        
        if (distanceToTarget < m_MinimumDistanceToTarget)
        {
            currentMaxSpeed = ExtensionMethods.Remap(m_MaxVelocity, 0, m_MaxVelocity, m_MaxVelocity, 0);
        }
        else
        {
            m_CurrentMaxSpeed = m_MaxVelocity;
        }

        //targetDirection.Normalize();
        targetDirection *= m_CurrentMaxSpeed;

        targetDirection -= m_CurrentVelocity;
        targetDirection *= m_ArriveFactor;
        Vector3.ClampMagnitude(targetDirection, m_MaxSteeringForce);

        return targetDirection;
    }

    private void Flow()
    {

    }

    private void Path()
    {

    }

    private Vector3 AvoidOtherBoids(List<Boid> a_Boids)
    {
        int numberOfCloseBoids = 0;
        m_DesiredVelocity = Vector3.zero;

        foreach (Boid otherBoid in a_Boids)
        {
            Vector3 oppositeDirection = transform.position - otherBoid.transform.position;
            float distanceToOtherBoids = oppositeDirection.sqrMagnitude;

            if (distanceToOtherBoids > 0 && distanceToOtherBoids < m_MinimumDistanceToOtherBoid)
            {
                numberOfCloseBoids++;

                //oppositeDirection.Normalize();
                oppositeDirection /= distanceToOtherBoids;
                m_DesiredVelocity += oppositeDirection;
            }
        }

        if (numberOfCloseBoids > 0)
        {
            m_DesiredVelocity /= numberOfCloseBoids;

            m_DesiredVelocity -= m_CurrentVelocity;
            m_DesiredVelocity *= m_AvoidanceFactor;
            Vector3.ClampMagnitude(m_DesiredVelocity, m_MaxSteeringForce);
            return m_DesiredVelocity;
        }

        return Vector3.zero;
    }
    #endregion Behaviors   
}