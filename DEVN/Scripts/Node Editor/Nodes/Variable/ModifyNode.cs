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
    public int m_blackboardSelection = 0;

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

        m_rectangle.width = 200;
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

        base.DrawNodeWindow(id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    /// <returns></returns>
    private bool DrawBlackboardPopup(ref Rect fieldRect)
    {
        List<Blackboard> blackboards = GameData.GetInstance().GetDefaultBlackboards();

        // compile string of blackboard names for popup
        string[] blackboardNames = new string[blackboards.Count];
        for (int i = 0; i < blackboards.Count; i++)
            blackboardNames[i] = blackboards[i].m_blackboardName;

        // draw "Blackboard" title and corresponding popup
        GUI.Label(fieldRect, "Blackboard");
        fieldRect.y += 16;
        m_blackboardSelection = EditorGUI.Popup(fieldRect, m_blackboardSelection, blackboardNames);
        fieldRect.y += 16;
        
        // no blackboards available
        if (blackboards.Count == 0)
        {
            m_rectangle.height = m_nodeHeight - 32;
            return false;
        }

        // ensure no index overflow when blackboards are removed
        m_blackboardSelection = Mathf.Clamp(m_blackboardSelection, 0, blackboards.Count - 1);
        m_currentBlackboard = blackboards[m_blackboardSelection]; // update current blackboard
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
        GUI.Label(fieldRect, "Variable");
        fieldRect.y += 16;
        m_variableSelection = EditorGUI.Popup(fieldRect, m_variableSelection, variableNames);
        fieldRect.y += 16;
            
        // no variables available in this blackboard
        if (keys.Count == 0)
        {
            m_rectangle.height = m_nodeHeight;
            return false;
        }

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

        switch (valueType)
        {
            case ValueType.Boolean:
                DrawBoolean(fieldRect);
                break;

            case ValueType.Float:
                DrawFloat(fieldRect);
                break;

            case ValueType.String:
                DrawString(fieldRect);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawBoolean(Rect fieldRect)
    {
        m_rectangle.height = m_nodeHeight + 32; // adjust rectangle height
        GUI.Label(fieldRect, "Action");
        fieldRect.y += 16;

        // display bool actions in drop-down (Set & Toggle)
        string[] boolAction = { "Set", "Toggle" };
        m_booleanSelection = EditorGUI.Popup(fieldRect, m_booleanSelection, boolAction);
        fieldRect.y += 18;

        // only display toggle if boolAction is "Set"
        if (m_booleanSelection == 0)
        {
            m_rectangle.height = m_nodeHeight + 50; // adjust rectangle height

            // print label and adjust positional values
            GUI.Label(fieldRect, "Value:");
            fieldRect.x = m_rectangle.width - 20;
            fieldRect.width = 16;

            // display toggle
            m_booleanValue = EditorGUI.Toggle(fieldRect, m_booleanValue);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawFloat(Rect fieldRect)
    {
        m_rectangle.height = m_nodeHeight + 50; // adjust rectangle height
        GUI.Label(fieldRect, "Action");
        fieldRect.y += 16;

        // display float action in drop-down (set & increment)
        string[] floatAction = { "Set", "Increment" };
        m_floatSelection = EditorGUI.Popup(fieldRect, m_floatSelection, floatAction);
        fieldRect.y += 18;

        // print label and adjust positional values
        GUI.Label(fieldRect, "Value:");
        fieldRect.x = m_rectangle.width * 0.5f;
        fieldRect.width = m_rectangle.width * 0.5f - 6;

        // display input field
        m_floatValue = EditorGUI.FloatField(fieldRect, m_floatValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldRect"></param>
    private void DrawString(Rect fieldRect)
    {
        m_rectangle.height = m_nodeHeight + 16; // adjust rectangle height
        fieldRect.y += 2;

        // print label and adjust positional values
        GUI.Label(fieldRect, "Set To Value:");
        fieldRect.x = m_rectangle.width * 0.5f;
        fieldRect.width = m_rectangle.width * 0.5f - 6;

        // display input field
        m_stringValue = EditorGUI.TextField(fieldRect, m_stringValue);
    }

#endif
}

}
