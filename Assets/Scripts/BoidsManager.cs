using UnityEngine;
using System.Collections.Generic;

public class BoidsManager : MonoBehaviour
{
    #region Variables
    [Header("Standard boid")]
    [SerializeField, Range(0.1f, 2f)]
    private float m_AccelerationFactor = 0.1f;
    [SerializeField, Range(0.1f, 2f)]
    private float m_DecelerationFactor = 0.1f;
    [SerializeField, Range(0.1f, 2f)]
    private float m_MaxVelocity = 0.1f;
    [SerializeField, Range(0.1f, 5f)]
    private float m_MaxSteeringForce = 0.1f;
    [SerializeField, Range(0f, 10f)]
    private float m_MinimumDistanceToTarget = 0.1f;
    [SerializeField, Range(0.1f, 10f)]
    private float m_AvoidanceFactor = 0.1f;
    [SerializeField, Range(0.1f, 10f)]
    private float m_ArriveFactor = 0.1f;
    [SerializeField, Range(0.1f, 10f)]
    private float m_MinimumDistanceToOtherBoid = 0.1f;

    [Header("Large boid")]
    [SerializeField, Range(-100, 800)]
    private float m_LargeAccelerationFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeDecelerationFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeMaxVelocity = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeMaxSteeringForce = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeMinimumDistanceToTarget = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeAvoidanceFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeArriveFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_LargeMinimumDistanceToOtherBoid = 0.1f;

    [Header("Close boid")]
    [SerializeField, Range(-100, 800)]
    private float m_CloseAccelerationFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseDecelerationFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseMaxVelocity = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseMaxSteeringForce = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseMinimumDistanceToTarget = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseAvoidanceFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseArriveFactor = 0.1f;
    [SerializeField, Range(-100, 800)]
    private float m_CloseMinimumDistanceToOtherBoid = 0.1f;

    private float m_CurrentAccelerationFactor;
    private float m_CurrentDecelerationFactor;
    private float m_CurrentMaxVelocity;
    private float m_CurrentMaxSteeringForce;
    private float m_CurrentMinimumDistanceToTarget;
    private float m_CurrentAvoidanceFactor;
    private float m_CurrentArriveFactor;
    private float m_CurrentMinimumDistanceToOtherBoid;

    [Header("NoNameYet")]
    [SerializeField]
    private Transform m_Target;
    [SerializeField, Range(0, 3)]
    private int m_BoidsBehaviour;

    [Header("Manager parameters")]
    [SerializeField]
    private Boid m_BoidPrefab;
    [SerializeField]
    private int m_NumberOfBoids = 1;
    [SerializeField]
    private bool m_CloseFormation;
    

    private List<Boid> m_Boids = new List<Boid>();
    #endregion Variables

    #region Singleton
    private static BoidsManager s_instance = null;

    public static BoidsManager Instance
    {
        get { return s_instance; }
    }

    protected void Awake()
    {
        if (s_instance != null)
        {
            Debug.LogError(name + " shouldn't have a BoidsManager referenced already.");
        }
        else
        {
            s_instance = this;
        }
    }
    #endregion Singleton

    #region PublicGetters
    public List<Boid> Boids
    {
        get { return m_Boids; }
    }
    #endregion PublicGetters

    #region Initialise
    protected void Start()
    {
        CreateBoids();
	}

    private void CreateBoids()
    {
        float radius = 0.5f;
        float xOffset = 0;
        float yOffset = 0;

        for (int i = 0; i < m_NumberOfBoids; i++)
        {
            radius += Mathf.PerlinNoise(xOffset, yOffset);
            Vector3 offset = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            Vector3 newPosition = transform.position + offset;

            GameObject boid = Instantiate(m_BoidPrefab.gameObject, newPosition, Quaternion.identity) as GameObject;
            //GameObject boid = Instantiate(m_BoidPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;

            Boid boidBehaviour = boid.GetComponent<Boid>();
            boidBehaviour.SetTarget(m_Target);
            boidBehaviour.SetMovementModifiers(m_AccelerationFactor, m_DecelerationFactor, m_MaxVelocity, m_MaxSteeringForce);
            boidBehaviour.SetBehaviorModifiers(m_MinimumDistanceToTarget, m_AvoidanceFactor, m_MinimumDistanceToOtherBoid, m_ArriveFactor);
            boidBehaviour.SetCurrentBehaviour(m_BoidsBehaviour);
            m_Boids.Add(boidBehaviour);
        }
    }
    #endregion Initialise

    protected void Update()
    {
        foreach (Boid boid in m_Boids)
        {
            boid.UpdateBehaviour(m_Boids);
        }

        if (!Input.GetButton("TL_1") || !Input.GetButton("LB_1"))
        {
            SetNewModifiers(0);
        }

        if (Input.GetButton("LB_1"))
        {
            SetNewModifiers(1);
        }

        if (Input.GetButton("TL_1"))
        {
            SetNewModifiers(2);
        }

        if (Input.GetButtonDown("A_1"))
        {
            if (m_BoidsBehaviour == 0)
            {
                SetBoidBehaviour(1);
            }
            else
            {
                SetBoidBehaviour(0);
            }
        }
    }

    #region BoidsBehavior
    public void SetBoidBehaviour(int a_Behavior)
    {
        m_BoidsBehaviour = a_Behavior;
        UpdateCurrentBehavior();
    }

    public void SetNewModifiers(int a_State)
    {
        if (a_State == 0)
        {
            m_CurrentAccelerationFactor = m_AccelerationFactor;
            m_CurrentDecelerationFactor = m_DecelerationFactor;
            m_CurrentMaxSteeringForce = m_MaxSteeringForce;
            m_CurrentMaxVelocity = m_MaxVelocity;
            m_CurrentArriveFactor = m_ArriveFactor;
            m_CurrentAvoidanceFactor = m_AvoidanceFactor;
            m_CurrentMinimumDistanceToTarget = m_MinimumDistanceToTarget;
            m_CurrentMinimumDistanceToOtherBoid = m_MinimumDistanceToOtherBoid;
        }
        else if (a_State == 1)
        {
            m_CurrentAccelerationFactor = m_AccelerationFactor * (1 + m_LargeAccelerationFactor * 0.01f);
            m_CurrentDecelerationFactor = m_DecelerationFactor * (1 +m_LargeDecelerationFactor * 0.01f);
            m_CurrentMaxSteeringForce = m_MaxSteeringForce * (1 + m_LargeMaxSteeringForce * 0.01f);
            m_CurrentMaxVelocity = m_MaxVelocity * (1 + m_LargeMaxVelocity * 0.01f);
            m_CurrentArriveFactor = m_ArriveFactor * (1 + m_LargeArriveFactor * 0.01f);
            m_CurrentAvoidanceFactor = m_AvoidanceFactor * (1 + m_LargeAvoidanceFactor * 0.01f);
            m_CurrentMinimumDistanceToTarget = m_MinimumDistanceToTarget * (1 + m_LargeMinimumDistanceToTarget * 0.01f);
            m_CurrentMinimumDistanceToOtherBoid = m_MinimumDistanceToOtherBoid * (1 + m_LargeMinimumDistanceToOtherBoid * 0.01f);
        }
        else if (a_State == 2)
        {
            m_CurrentAccelerationFactor = m_AccelerationFactor * (1 - m_CloseAccelerationFactor * 0.01f);
            m_CurrentDecelerationFactor = m_DecelerationFactor * (1- m_CloseDecelerationFactor * 0.01f);
            m_CurrentMaxSteeringForce = m_MaxSteeringForce * (1 - m_CloseMaxSteeringForce * 0.01f);
            m_CurrentMaxVelocity = m_MaxVelocity * (1 - m_CloseMaxVelocity * 0.01f);
            m_CurrentArriveFactor = m_ArriveFactor * (1 - m_CloseArriveFactor * 0.01f);
            m_CurrentAvoidanceFactor = m_AvoidanceFactor * (1 - m_CloseAvoidanceFactor * 0.01f);
            m_CurrentMinimumDistanceToTarget = m_MinimumDistanceToTarget * (1 - m_CloseMinimumDistanceToTarget * 0.01f);
            m_CurrentMinimumDistanceToOtherBoid = m_MinimumDistanceToOtherBoid * (1 - m_CloseMinimumDistanceToOtherBoid * 0.01f);
        }

        UpdateMovementModifiers(m_CurrentAccelerationFactor, m_CurrentDecelerationFactor, m_CurrentMaxVelocity, m_CurrentMaxSteeringForce);
        UpdateBehaviourModifiers(m_CurrentMinimumDistanceToTarget, m_CurrentAvoidanceFactor, m_CurrentMinimumDistanceToOtherBoid, m_CurrentArriveFactor);
    }

    private void UpdateMovementModifiers(float a_Acceleration, float a_Deceleration, float a_MaxVelocity, float a_MaxSteering)
    {
        foreach(Boid boid in m_Boids)
        {
            boid.SetMovementModifiers(a_Acceleration, a_Deceleration, a_MaxVelocity, a_MaxSteering);
        }
    }

    private void UpdateBehaviourModifiers(float a_MinimumDistanceToTarget, float a_AvoidanceFactor, float a_MinimumDistanceToOtherBoid, float a_ArriveFactor)
    {
        foreach (Boid boid in m_Boids)
        {
            boid.SetBehaviorModifiers(a_MinimumDistanceToTarget, a_AvoidanceFactor, a_MinimumDistanceToOtherBoid, a_ArriveFactor);
        }
    }

    private void UpdateCurrentBehavior()
    {
        foreach (Boid boid in m_Boids)
        {
            boid.SetCurrentBehaviour(m_BoidsBehaviour);
        }
    }
    #endregion BoidsBehavior
}