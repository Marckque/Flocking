using UnityEngine;

public class FlowField : MonoBehaviour
{
    [SerializeField]
    private int m_Resolution;
    [SerializeField]
    private int m_Width;
    [SerializeField]
    private int m_Height;

    private Vector3[,] m_FlowField;
    private int m_Rows;
    private int m_Columns;

	private void Start ()
    {
        InitialiseFlowField();
	}
	
	private void InitialiseFlowField()
    {
        m_Rows = m_Width / m_Resolution;
        m_Columns = m_Height / m_Resolution;

        m_FlowField = new Vector3[m_Rows, m_Columns];

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                m_FlowField[x, y] = new Vector3(x * m_Rows, 0, y * m_Columns);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Debug.DrawLine(m_FlowField[x, y], m_FlowField[x, y] * 5);
                }
            }
        }
    }
}