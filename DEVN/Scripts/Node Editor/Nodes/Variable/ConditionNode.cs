using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

[System.Serializable]
public class ConditionNode : BaseNode
{
    [SerializeField] private ValueType m_valueType;

    // blackboard selection variables
    [SerializeField] private Blackboard m_blackboardA;
    [SerializeField] private Blackboard m_blackboardB;

    // variable selection
    [SerializeField] private string m_currentKeyA;
    [SerializeField] private string m_currentKeyB;
    [SerializeField] private int m_keySelectionA = 0;
    [SerializeField] private int m_keySelectionB = 0;

    private string[] m_sourceOptions = { "Value", "Blackboard" };
    [SerializeField] private int m_sourceSelection = 0;

    private string[] m_booleanConditions = { "Equal To" };
    private string[] m_floatConditions = { "Less Than", "Equal To", "Greater Than" };
    [SerializeField] private int m_booleanSelection = 0;
    [SerializeField] private int m_floatSelection = 1;

    [SerializeField] private bool m_booleanValue = false;
    [SerializeField] private float m_floatValue = 0;
    [SerializeField] private string m_stringValue = "";

	#region getters

    public ValueType GetValueType() { return m_valueType; }
	public Blackboard GetBlackboardA() { return m_blackboardA; }
	public Blackboard GetBlackboardB() { return m_blackboardB; }
	public string GetKeyA() { return m_currentKeyA; }
	public string GetKeyB() { return m_currentKeyB; }
	public int GetSourceSelection() { return m_sourceSelection; }
	public int GetBooleanSelection() { return m_booleanSelection; }
	public int GetFloatSelection() { return m_floatSelection; }
    public bool GetBooleanValue() { return m_booleanValue; }
    public float GetFloatValue() { return m_floatValue; }
    public string GetStringValue() { return m_stringValue; }

	#endregion

#if UNITY_EDITOR

	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Condition";

        m_rectangle.width = 256;
        m_rectangle.height = 58;

        AddOutputPoint(); // true
        AddOutputPoint(); // false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    protected override void DrawNodeWindow(int id)
    {
        EditorGUIUtility.labelWidth = 16;

        EditorGUILayout.BeginHorizontal();
        bool selectionA = DrawBlackboardVariableSelection(ref m_blackboardA, 
            ref m_currentKeyA, ref m_keySelectionA);
        EditorGUILayout.EndHorizontal();
        
        if (selectionA)
        {
            m_valueType = m_blackboardA.GetValueType(m_currentKeyA);
            DrawConditionPopup();
            EditorGUILayout.BeginHorizontal();
            DrawSourcePopup();

            if (m_sourceSelection == 0)
            {
                //EditorGUIUtility.labelWidth = 64;
                DrawValueField();
                EditorGUILayout.EndHorizontal();
            }
            else if (m_sourceSelection == 1)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                DrawBlackboardVariableSelection(ref m_blackboardB, 
                    ref m_currentKeyB, ref m_keySelectionB , true);
                EditorGUILayout.EndHorizontal();
            }
        }
        
        // resize node height
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
    /// <param name="blackboard"></param>
    /// <param name="key"></param>
    /// <param name="keySelection"></param>
    /// <returns></returns>
    private bool DrawBlackboardVariableSelection(ref Blackboard blackboard, 
        ref string key, ref int keySelection, bool limitValueType = false)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Blackboard");
        blackboard = EditorGUILayout.ObjectField(blackboard, typeof(Blackboard), false) as Blackboard;
        EditorGUILayout.EndVertical();

        if (blackboard == null)
            return false;

        List<string> keys = null;
        if (limitValueType)
            keys = blackboard.GetAllOfValueType(m_valueType);
        else
            keys = blackboard.GetKeys();

        // compile string of variable names for popup
        string[] variableNames = new string[keys.Count];
        for (int i = 0; i < keys.Count; i++)
            variableNames[i] = keys[i];
    
        // draw "Variable" title and corresponding popup
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Variable");
        keySelection = EditorGUILayout.Popup(keySelection, variableNames);
        EditorGUILayout.EndVertical();

        // no variables available in this blackboard
        if (keys.Count == 0)
            return false;
    
        // ensure no index overflow when variables are removed
        keySelection = Mathf.Clamp(keySelection, 0, keys.Count - 1);
        key = keys[keySelection]; // update curent key/variable
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawConditionPopup()
    {
        EditorGUILayout.LabelField("Condition");

        switch (m_valueType)
        {
            case ValueType.Boolean:
                m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, m_booleanConditions);
                break;

            case ValueType.Float:
                m_floatSelection = EditorGUILayout.Popup(m_floatSelection, m_floatConditions);
                break;

            case ValueType.String:
                m_booleanSelection = EditorGUILayout.Popup(m_booleanSelection, m_booleanConditions);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawSourcePopup()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Compare To");
        m_sourceSelection = EditorGUILayout.Popup(m_sourceSelection, m_sourceOptions);
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void DrawValueField()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Value");

        switch (m_valueType)
        {
            case ValueType.Boolean:
                m_booleanValue = EditorGUILayout.Toggle(m_booleanValue);
                break;

            case ValueType.Float:
                m_floatValue = EditorGUILayout.FloatField(m_floatValue);
                break;

            case ValueType.String:
                m_stringValue = EditorGUILayout.TextField(m_stringValue);
                break;
        }

        EditorGUILayout.EndVertical();
    }
        
#endif
}

}
