using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// utility delay node used for delaying node proceeding for an arbitrary
/// amount of time
/// </summary>
[System.Serializable]
public class DelayNode : BaseNode
{
    [SerializeField] private float m_delayTime = 0; // delay time in seconds

    #region getters

    public float GetDelayTime() { return m_delayTime; }

    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor
    /// </summary>
    /// <param name="position">position of creation</param>
    public override void Init(Vector2 position)
    {
        base.Init(position);
            
        m_title = "Delay";
            
        m_rectangle.width = 100;
        m_rectangle.height = 58;
            
        AddOutputPoint(); // linear
    }

    /// <summary>
    /// overridden copy constructor
    /// </summary>
    /// <param name="node">node to copy</param>
    /// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        DelayNode delayNode = node as DelayNode;

        // copy delay time
        m_delayTime = delayNode.m_delayTime;
    }

    /// <summary>
    /// overridden draw function, draws the delay time float field
    /// </summary>
    /// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
        // draw delay time label and float field
        EditorGUILayout.LabelField("Delay Time (s)");
        m_delayTime = EditorGUILayout.FloatField(m_delayTime);

		base.DrawNodeWindow(id);
	}

#endif
}

}