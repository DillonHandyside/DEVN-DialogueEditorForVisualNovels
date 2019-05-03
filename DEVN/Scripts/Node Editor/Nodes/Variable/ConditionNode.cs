using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

[System.Serializable]
public class ConditionNode : BaseNode
{
    // blackboard selection variables
    [SerializeField] private Blackboard m_currentBlackboardA;
    [SerializeField] private Blackboard m_currentBlackboardB;
    [SerializeField] private int m_blackboardSelectionA = 0;
    [SerializeField] private int m_blackboardSelectionB = 0;

    // variable selection
    [SerializeField] private string m_currentKeyA;
    [SerializeField] private string m_currentKeyB;
    [SerializeField] private int m_keySelectionA = 0;
    [SerializeField] private int m_keySelectionB = 0;

    [SerializeField] private int m_sourceSelection = 0;
	[SerializeField] private int m_booleanSelection = 0;
    [SerializeField] private int m_floatSelection = 1;

    [SerializeField] private float m_nodeHeight = 0;

	#region getters

		public Blackboard GetBlackboardA() { return m_currentBlackboardA; }
		public Blackboard GetBlackboardB() { return m_currentBlackboardB; }
		public int GetBlackboardSelectionA() { return m_blackboardSelectionA; }
		public int GetBlackboardSelectionB() { return m_blackboardSelectionB; }
		public string GetKeyA() { return m_currentKeyA; }
		public string GetKeyB() { return m_currentKeyB; }
		public int GetKeySelectionA() { return m_keySelectionA; }
		public int GetKeySelectionB() { return m_keySelectionB; }
		public int GetSourceSelection() { return m_sourceSelection; }
		public int GetBooleanSelection() { return m_booleanSelection; }
		public int GetFloatSelection() { return m_floatSelection; }

		#endregion

#if UNITY_EDITOR

	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Condition";

        m_rectangle.width = 248;
        m_rectangle.height = 58;

        m_nodeHeight = m_rectangle.height;

        AddOutputPoint(); // true
        AddOutputPoint(); // false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    protected override void DrawNodeWindow(int id)
    {
        Rect fieldRect = new Rect(5, 20, m_rectangle.width * 0.5f - 6, 16);

        DrawBlackboardPopup(ref fieldRect, ref m_currentBlackboardA, ref m_blackboardSelectionA);

        if (DrawVariablePopup(ref fieldRect, ref m_currentBlackboardA, ref m_currentKeyA, ref m_keySelectionA))
        {
            m_rectangle.height = m_nodeHeight + 64;
            DrawConditionPopup(ref fieldRect);
            DrawSourcePopup(ref fieldRect);

            if (m_sourceSelection == 0)
            {
                m_rectangle.height = m_nodeHeight + 80;

                DrawContent(fieldRect);
            }
            else
            {
                m_rectangle.height = m_nodeHeight + 96;

                DrawBlackboardPopup(ref fieldRect, ref m_currentBlackboardB, ref m_blackboardSelectionB);
                DrawVariablePopup(ref fieldRect, ref m_currentBlackboardB, ref m_currentKeyB, ref m_keySelectionB);
            }
        }

        base.DrawNodeWindow(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawBlackboardPopup(ref Rect fieldRect, ref Blackboard blackboard, ref int blackboardSelection)
    {
        List<Blackboard> blackboards = GameData.GetInstance().GetDefaultBlackboards();

        // compile string of blackboard names for popup
        string[] blackboardNames = new string[blackboards.Count];
        for (int i = 0; i < blackboards.Count; i++)
            blackboardNames[i] = blackboards[i].m_blackboardName;

        // take up half of the node window
        fieldRect.width = m_rectangle.width * 0.5f - 6;

        // draw "Blackboard" title and corresponding popup
        GUI.Label(fieldRect, "Blackboard");
        fieldRect.y += 16;
        blackboardSelection = EditorGUI.Popup(fieldRect, blackboardSelection, blackboardNames);
        fieldRect.y -= 16;

        // no blackboards available
        if (blackboards.Count == 0)
        {
            m_rectangle.height = m_nodeHeight - 32;
            return false;
        }

        // ensure no index overflow when blackboards are removed
        blackboardSelection = Mathf.Clamp(blackboardSelection, 0, blackboards.Count - 1);
        blackboard = blackboards[blackboardSelection]; // update current blackboard
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawVariablePopup(ref Rect fieldRect, ref Blackboard blackboard, 
        ref string key, ref int keySelection)
    {
        List<string> keys = blackboard.GetKeys();

        // compile string of variable names for popup
        string[] variableNames = new string[keys.Count];
        for (int i = 0; i < keys.Count; i++)
            variableNames[i] = keys[i];

        // take up the second half of the node window
        fieldRect.x += m_rectangle.width * 0.5f - 4;
        fieldRect.width = m_rectangle.width * 0.5f - 6;

        // draw "Variable" title and corresponding popup
        GUI.Label(fieldRect, "Variable");
        fieldRect.y += 16;
        keySelection = EditorGUI.Popup(fieldRect, keySelection, variableNames);
        fieldRect.y += 16;

        // no variables available in this blackboard
        if (keys.Count == 0)
        {
            m_rectangle.height = m_nodeHeight;
            return false;
        }

        // ensure no index overflow when variables are removed
        keySelection = Mathf.Clamp(keySelection, 0, keys.Count - 1);
        key = keys[keySelection]; // update curent key/variable
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueType"></param>
    private void DrawConditionPopup(ref Rect fieldRect)
    {
        ValueType valueType = m_currentBlackboardA.GetValueType(m_currentKeyA);

        fieldRect.x = 5;
        fieldRect.width = m_rectangle.width - 10;
        GUI.Label(fieldRect, "Condition");
        fieldRect.y += 16;

        switch (valueType)
        {
            case ValueType.Boolean:
                string[] booleanConditions = { "Equal To", "Not Equal To" };
                m_booleanSelection = EditorGUI.Popup(fieldRect, m_booleanSelection, booleanConditions);
                break;

            case ValueType.Float:
                string[] floatConditions = { "Less Than", "Equal To", "Greater Than" };
                m_floatSelection = EditorGUI.Popup(fieldRect, m_floatSelection, floatConditions);
                break;

            case ValueType.String:
                DrawString(fieldRect);
                break;
        }

        fieldRect.y += 16;
    }

    private void DrawContent(Rect fieldRect)
    {
        ValueType valueType = m_currentBlackboardA.GetValueType(m_currentKeyA);

        fieldRect.x = 5;
        fieldRect.y += 2;
        fieldRect.width = m_rectangle.width - 10;
        GUI.Label(fieldRect, "Value:");
        fieldRect.y += 16;

        switch (valueType)
        {
            case ValueType.Boolean:
                break;

            case ValueType.Float:
                break;

            case ValueType.String:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawBoolean(Rect fieldRect)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawFloat(Rect fieldRect)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawString(Rect fieldRect)
    {
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawSourcePopup(ref Rect fieldRect)
    {
        string[] sources = { "Value", "Blackboard" };

        GUI.Label(fieldRect, "Compare To");
        fieldRect.y += 16;
        m_sourceSelection = EditorGUI.Popup(fieldRect, m_sourceSelection, sources);
        fieldRect.y += 16;
    }

#endif
}

}
