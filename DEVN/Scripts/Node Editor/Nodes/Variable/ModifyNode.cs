using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{
    
[System.Serializable]
public class ModifyNode : BaseNode
{
    // blackboard selection variables
    private Blackboard m_currentBlackboard;

    // variable selection
    private string m_currentKey;
    public int m_variableSelection = 0;

    // boolean modification
    public bool m_booleanValue = false;
    public int m_booleanSelection = 0;

    // float modification
    public float m_floatValue = 0.0f;
    public int m_floatSelection = 0;
    
    // string modification
    public string m_stringValue = "";

    private float m_nodeHeight = 0;

#if UNITY_EDITOR

	public override void Init(Vector2 position)
    {
        base.Init(position);
        
        m_title = "Modify";

        m_rectangle.width = 180;
        m_rectangle.height = 90;

        m_nodeHeight = m_rectangle.height;

        AddOutputPoint(); // linear
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    protected override void DrawNodeWindow(int id)
    {
        Rect fieldRect = new Rect(5, 20, m_rectangle.width - 10, 16);

        if (DrawBlackboardPopup(ref fieldRect))
        {
            if (DrawVariablePopup(ref fieldRect))
				DrawContent(fieldRect);
        }

		if (Event.current.type == EventType.Repaint)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			m_rectangle.height = lastRect.y + lastRect.height + 4;
		}

		base.DrawNodeWindow(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawBlackboardPopup(ref Rect fieldRect)
    {
        // draw "Blackboard" title and corresponding popup
		EditorGUILayout.LabelField("Blackboard");
		m_currentBlackboard = EditorGUILayout.ObjectField(m_currentBlackboard, typeof(Blackboard), false) as Blackboard;

		// no blackboards available
		if (m_currentBlackboard == null)
			return false;
		
		return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawVariablePopup(ref Rect fieldRect)
    {
        List<string> keys = m_currentBlackboard.GetKeys();

        // compile string of variable names for popup
        string[] variableNames = new string[keys.Count];
        for (int i = 0; i < keys.Count; i++)
            variableNames[i] = keys[i];

        // draw "Variable" title and corresponding popup
        EditorGUILayout.LabelField("Variable");
        m_variableSelection = EditorGUILayout.Popup(m_variableSelection, variableNames);
            
        // no variables available in this blackboard
        if (keys.Count == 0)
            return false;

        // ensure no index overflow when variables are removed
        m_variableSelection = Mathf.Clamp(m_variableSelection, 0, keys.Count - 1);
        m_currentKey = keys[m_variableSelection]; // update curent key/variable
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueType"></param>
    private void DrawContent(Rect fieldRect)
    {
        ValueType valueType = m_currentBlackboard.GetValueType(m_currentKey);
		EditorGUIUtility.labelWidth = 48;

        switch (valueType)
        {
            case ValueType.Boolean:
                DrawBoolean();
                break;

            case ValueType.Float:
                DrawFloat();
                break;

            case ValueType.String:
                DrawString();
                break;
		}
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawBoolean()
    {
        EditorGUILayout.LabelField("Action");

        // display bool actions in drop-down (Set & Toggle)
        string[] boolAction = { "Set", "Toggle" };
        m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, boolAction);

        // only display toggle if boolAction is "Set"
        if (m_booleanSelection == 0)
        {
            // print label and adjust positional values
            EditorGUILayout.LabelField("Value:");

			// display toggle
			Rect toggleRect = GUILayoutUtility.GetLastRect();
			toggleRect.x = m_rectangle.width - 20;
            m_booleanValue = EditorGUI.Toggle(toggleRect, m_booleanValue);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawFloat()
    {
        EditorGUILayout.LabelField("Action");

        // display float action in drop-down (set & increment)
        string[] floatAction = { "Set", "Increment" };
        m_floatSelection = EditorGUILayout.Popup(m_floatSelection, floatAction);

        // display input field
        m_floatValue = EditorGUILayout.FloatField("Value: ", m_floatValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawString()
    {
        // display input field
        m_stringValue = EditorGUILayout.TextField("Set To: ", m_stringValue);
    }

#endif
}

}
