using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class DelayNode : BaseNode
{
    [SerializeField] private float m_delayTime = 0.0f;

    #region getters

    public float GetDelayTime() { return m_delayTime; }

    #endregion

#if UNITY_EDITOR

    public override void Init(Vector2 position)
    {
        base.Init(position);
            
        m_title = "Delay";
            
        m_rectangle.width = 128;
        m_rectangle.height = 58;
            
        AddOutputPoint(); // linear
    }

    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        DelayNode delayNode = node as DelayNode;

        m_delayTime = delayNode.m_delayTime;
    }

    protected override void DrawNodeWindow(int id)
	{
        float fieldWidth = m_rectangle.width - 10;
        float fieldHeight = 16;

        Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

        GUI.Label(fieldRect, "Delay Time (s)");
        fieldRect.y += fieldHeight;
        m_delayTime = EditorGUI.FloatField(fieldRect, m_delayTime);

		base.DrawNodeWindow(id);
	}

#endif
}

}
