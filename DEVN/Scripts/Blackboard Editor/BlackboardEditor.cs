using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN;

#if UNITY_EDITOR

/// <summary>
/// An editor window which allows the user to create and edit blackboards.
/// See Blackboards.cs for more details on what a blackboard entails
/// </summary>
public class BlackboardEditor : EditorWindow
{
    private Texture2D m_logoDEVN; // watermark logo

    // a collection of all user-created blackboards
    private List<Blackboard> m_blackboards;

    // selection variables
    private Blackboard m_currentBlackboard;
    private int m_currentSelection = 0;

    // scroll view variables
    private Vector2 m_scrollPosition = Vector2.zero;
    private float m_scrollViewHeight = 32.0f;

    // key-value field variables
    private string m_booleanKey = "";
    private string m_floatKey = "";
    private string m_stringKey = "";

    [MenuItem("Window/DEVN/Blackboards")]
    public static void Init()
    {
        BlackboardEditor window = GetWindow<BlackboardEditor>();
        window.titleContent = new GUIContent("Blackboards");
    }

    /// <summary>
    /// Blackboard editor "awake" function. Initialises all relevant variables
    /// </summary>
    private void OnEnable()
    {
        // retrieve saved list of blackboards from GameData
        m_blackboards = GameData.GetInstance().GetDefaultBlackboards();

        // load logo
        string path = "Assets/DEVN/Textures/logoDEVN.png";
        m_logoDEVN = EditorGUIUtility.Load(path) as Texture2D;
    }

    /// <summary>
    /// Blackboard editor "update" function. Perform drawing of all relevant
    /// labels, fields and sprites
    /// </summary>
    void OnGUI()
    {
        DrawAddAndRemove();

        // only draw blackboard contents/elements if a blackboard is selected
        if (m_currentBlackboard != null)
        {
            DrawNameField();
            DrawScrollView();
        }

        DrawLogo(); // draw DEVN water-mark
		GameData.GetInstance().SetDefaultBlackboards(m_blackboards);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawAddAndRemove()
    {
        // define positional variables
        Rect popupRect = new Rect(5.0f, 5.0f, Screen.width - 10.0f, 16.0f);
        Rect removeRect = new Rect(Screen.width - 29.0f, 24.0f, 24.0f, 24.0f);
        Rect addRect = new Rect(Screen.width - 56.0f, 24.0f, 24.0f, 24.0f);

        // compile string of blackboard names for popup
        string[] blackboards = new string[m_blackboards.Count];
        for (int i = 0; i < m_blackboards.Count; i++)
            blackboards[i] = m_blackboards[i].m_blackboardName;

        // draw popup (drop-down menu)
        m_currentSelection = EditorGUI.Popup(popupRect, m_currentSelection, blackboards);

        // draw "remove" button
        if (GUI.Button(removeRect, "-"))
        {
            if (m_currentBlackboard != null)
                m_blackboards.Remove(m_currentBlackboard); // perform removal

            // ensure no index out-of-range errors when deleting end blackboard
            m_currentSelection = Mathf.Clamp(m_currentSelection, 0, m_blackboards.Count - 1);
        }

        // draw "add" button
        if (GUI.Button(addRect, "+"))
        {
            Blackboard blackboard = CreateInstance<Blackboard>();

            // create scriptable object in the format of "New Blackboard (1, 2, etc.)"
            if (m_blackboards.Count == 0)
                blackboard.m_blackboardName = "New Blackboard";
            else
                blackboard.m_blackboardName = "New Blackboard " + m_blackboards.Count;

            m_blackboards.Add(blackboard);
            m_currentSelection = m_blackboards.IndexOf(blackboard);
            ClearKeyFields();
        }

        // update the current blackboard selection
        if (m_blackboards.Count > 0)
            m_currentBlackboard = m_blackboards[m_currentSelection];
        else
            m_currentBlackboard = null; // no blackboards, no selection
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawNameField()
    {
        Rect textRect = new Rect(5.0f, 28.0f, 48.0f, 16.0f);
        Rect fieldRect = new Rect(48.0f, 28.0f, Screen.width - 108.0f, 16.0f);

        GUI.Label(textRect, "Name");
        m_currentBlackboard.m_blackboardName = EditorGUI.TextField(fieldRect, m_currentBlackboard.m_blackboardName);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawScrollView()
    {
        Rect scrollViewPosition = new Rect(0.0f, 52.0f, Screen.width, Screen.height - 74.0f);
        Rect scrollViewRect = new Rect(0.0f, 52.0f, Screen.width - 16.0f, m_scrollViewHeight);

        GUI.Box(scrollViewPosition, "");

        // begin scroll view
        m_scrollPosition = GUI.BeginScrollView(scrollViewPosition, m_scrollPosition, scrollViewRect, false, true);

        if (m_currentBlackboard != null)
        {
            // content 
            float yPos = 56;
            DrawBlackboardContent(ValueType.Boolean, ref yPos);
            DrawBlackboardContent(ValueType.Float, ref yPos);
            DrawBlackboardContent(ValueType.String, ref yPos);
            m_scrollViewHeight = yPos;
        }

        // end scroll view
        GUI.EndScrollView();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawBlackboardContent(ValueType valueType, ref float yPos)
    {
        float width = Screen.width - 16;

        // draw content heading
        GUI.Label(new Rect(5, yPos, width, 16), valueType.ToString());
        yPos += 16;

        // get all keys (variables) in blackboard
        List<string> keys = m_currentBlackboard.GetAllOfValueType(valueType);

        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            Value value = m_currentBlackboard.GetValue(key);

            Rect textRect = new Rect(5.0f, yPos, Screen.width - 106, 16);
            Rect fieldRect = new Rect(Screen.width - 102, yPos, 86, 16);

            if (i > 0)
            {
                Handles.color = Color.grey;
                Handles.DrawLine(new Vector3(5, yPos), new Vector3(Screen.width - 20, yPos));
                Handles.color = Color.white;
            }

            GUI.Label(textRect, key);

            switch (valueType)
            {
                case ValueType.Boolean:
                    textRect.width += 70;
                    fieldRect.x += 70;
                    fieldRect.width -= 70;
                    m_currentBlackboard.SetValue(key, EditorGUI.Toggle(fieldRect, value.m_boolean));
                    break;

                case ValueType.Float:
                    m_currentBlackboard.SetValue(key, EditorGUI.FloatField(fieldRect, value.m_float));
                    break;

                case ValueType.String:
                    m_currentBlackboard.SetValue(key, EditorGUI.TextField(fieldRect, value.m_string));
                    break;
            }

            yPos += 17;
        }

        yPos += 2;

        string keyToAdd;
        switch (valueType)
        {
            default:
                m_booleanKey = EditorGUI.TextField(new Rect(5, yPos + 4, Screen.width - 78, 16), m_booleanKey);
                keyToAdd = m_booleanKey;
                break;

            case ValueType.Float:
                m_floatKey = EditorGUI.TextField(new Rect(5, yPos + 4, Screen.width - 78, 16), m_floatKey);
                keyToAdd = m_floatKey;
                break;

            case ValueType.String:
                m_stringKey = EditorGUI.TextField(new Rect(5, yPos + 4, Screen.width - 78, 16), m_stringKey);
                keyToAdd = m_stringKey;
                break;
        }

        keys.Clear();
        keys = m_currentBlackboard.GetKeys();

        // "remove" button
        if (GUI.Button(new Rect(Screen.width - 42, yPos, 24, 24), "-"))
        {
            if (keys.Contains(keyToAdd))
            {
                m_currentBlackboard.RemoveKey(keyToAdd);
                ClearKeyField(valueType);
            }
        }

        // "add" button
        if (GUI.Button(new Rect(Screen.width - 70, yPos, 24, 24), "+"))
        {
            if (!keys.Contains(keyToAdd) && keyToAdd.Length > 0)
            {
                m_currentBlackboard.AddKey(keyToAdd, valueType);
                ClearKeyField(valueType);
            }
        }

        yPos += 32.0f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueType"></param>
    private void ClearKeyField(ValueType valueType)
    {
        GUI.FocusControl(null);

        switch (valueType)
        {
            case ValueType.Boolean:
                m_booleanKey = "";
                break;

            case ValueType.Float:
                m_floatKey = "";
                break;

            case ValueType.String:
                m_stringKey = "";
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ClearKeyFields()
    {
        GUI.FocusControl(null);

        m_booleanKey = "";
        m_floatKey = "";
        m_stringKey = "";
    }

    /// <summary>
    /// draws the DEVN water-mark logo. For fanciness
    /// </summary>
    private void DrawLogo()
    {
        float xPosLogo = Screen.width - 171.0f;
        float yPosLogo = Screen.height - 100.0f;
        float xPosText = Screen.width - 211.0f;
        float yPosText = Screen.height - 40.0f;

        // adjust transparency
        GUI.color = new Color(1, 1, 1, 0.25f);

        // draw logo & text
        GUI.DrawTexture(new Rect(xPosLogo, yPosLogo, 150.0f, 75.0f), m_logoDEVN);
        GUI.Label(new Rect(xPosText, yPosText, 300.0f, 20.0f), "Dialogue Editor for Visual Novels");

        // reset transparency
        GUI.color = new Color(1, 1, 1, 1.0f);
    }
}

#endif