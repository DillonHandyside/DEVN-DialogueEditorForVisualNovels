using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{
    
[System.Serializable]
public class ModifyNode : BaseNode
{
    [SerializeField] private ValueType m_valueType;

    // blackboard selection variables
    [SerializeField] private Blackboard m_blackboard;

    // variable selection
    [SerializeField] private string m_currentKey;
    [SerializeField] private int m_variableSelection = 0;

    // boolean modification
    [SerializeField] private bool m_booleanValue = false;
    [SerializeField] private int m_booleanSelection = 0;

    // float modification
    [SerializeField] private float m_floatValue = 0.0f;
    [SerializeField] private int m_floatSelection = 0;
    
    // string modification
    [SerializeField] private string m_stringValue = "";

    #region getters

    public ValueType GetValueType() { return m_valueType; }
    public Blackboard GetBlackboard() { return m_blackboard; }
    public string GetKey() { return m_currentKey; }
    public bool GetBooleanValue() { return m_booleanValue; }
    public int GetBooleanSelection() { return m_booleanSelection; }
    public float GetFloatValue() { return m_floatValue; }
    public int GetFloatSelection() { return m_floatSelection; }
    public string GetStringValue() { return m_stringValue; }

    #endregion

#if UNITY_EDITOR

    public override void Init(Vector2 position)
    {
        base.Init(position);
        
        m_title = "Modify";

        m_rectangle.width = 180;
        m_rectangle.height = 90;

        AddOutputPoint(); // linear
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    protected override void DrawNodeWindow(int id)
    {
        DrawBlackboardPopup();

        if (m_blackboard)
        {
            if (DrawVariablePopup())
				DrawContent();
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
    private void DrawBlackboardPopup()
    {
        // draw "Blackboard" title and object field
		EditorGUILayout.LabelField("Blackboard");
        m_blackboard = EditorGUILayout.ObjectField(m_blackboard, typeof(Blackboard), false) as Blackboard;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawVariablePopup()
    {
        List<string> keys = m_blackboard.GetKeys();

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
    private void DrawContent()
    {
        m_valueType = m_blackboard.GetValueType(m_currentKey);
		EditorGUIUtility.labelWidth = 48;

        switch (m_valueType)
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
