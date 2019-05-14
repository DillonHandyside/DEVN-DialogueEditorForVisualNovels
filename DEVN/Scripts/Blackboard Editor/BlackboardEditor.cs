using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using DEVN;

#if UNITY_EDITOR

/// <summary>
/// An editor window which allows the user to create and edit blackboards.
/// See Blackboards.cs for more details on what a blackboard entails
/// </summary>
public class BlackboardEditor : EditorWindow
{
    private Texture2D m_logoDEVN; // watermark logo
    
    private Blackboard m_blackboard; // the current blackboard

    // reorderable lists for each variable type
    private ReorderableList m_booleanList;
    private ReorderableList m_floatList;
    private ReorderableList m_stringList;
    
    private Vector2 m_scrollPosition = Vector2.zero; // scrollview position

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
		if (m_blackboard != null)
			InitialiseReorderableLists();

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
		Blackboard previousBlackboard = m_blackboard;

		EditorGUILayout.Space();
        m_blackboard = EditorGUILayout.ObjectField("Current Blackboard", m_blackboard,
                       typeof(Blackboard), false) as Blackboard;

		// only draw blackboard contents/elements if a blackboard is selected
		if (m_blackboard != null)
		{
			if (previousBlackboard != m_blackboard)
				InitialiseReorderableLists();

			DrawContent(); // draw scroll view and reorderable lists
			DrawLogo(); // draw DEVN watermark

			EditorUtility.SetDirty(m_blackboard);
		}
    }

	/// <summary>
	/// 
	/// </summary>
	private void DrawContent()
    {
		m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
        
		DrawReorderableList(m_booleanList, ValueType.Boolean);
		DrawReorderableList(m_floatList, ValueType.Float);
		DrawReorderableList(m_stringList, ValueType.String);
		
		EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 
    /// </summary>
	private void InitialiseReorderableLists()
	{
		m_booleanList = new ReorderableList(m_blackboard.GetBooleans(), typeof(KeyValue), false, true, true, true);
		m_floatList = new ReorderableList(m_blackboard.GetFloats(), typeof(KeyValue), false, true, true, true);
		m_stringList = new ReorderableList(m_blackboard.GetStrings(), typeof(KeyValue), false, true, true, true);
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reorderableList"></param>
    /// <param name="valueType"></param>
	private void DrawReorderableList(ReorderableList reorderableList, ValueType valueType)
	{
		List<string> keys = m_blackboard.GetAllOfValueType(valueType);

        // header callback
		reorderableList.drawHeaderCallback = rect =>
		{
            DrawHeader(rect, valueType);
		};

        // draw callback
		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
            DrawElement(rect, index, keys[index], valueType);
		};

        // add callback
		reorderableList.onAddCallback = (ReorderableList list) =>
		{
            AddElement(valueType);
		};

        // remove callback
		reorderableList.onRemoveCallback = (ReorderableList list) =>
		{
            RemoveElement(list.index, valueType);
		};

		// 
		GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
		scrollViewStyle.margin = new RectOffset(8, 8, 8, 8);

		//
		EditorGUILayout.BeginVertical(scrollViewStyle);
		reorderableList.DoLayoutList();
		EditorGUILayout.EndVertical();
		
		EditorUtility.SetDirty(m_blackboard);
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="valueType"></param>
    private void DrawHeader(Rect rect, ValueType valueType)
    {
        // print header as "Booleans", "Floats", etc.
        string label = valueType.ToString() + "s";
        EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="key"></param>
    /// <param name="valueType"></param>
    private void DrawElement(Rect rect, int index, string key, ValueType valueType)
    {
        // determine rectangle positions so each element is two evenly spaced fields
        Rect firstHalf = rect;
        firstHalf.width = firstHalf.width * 0.5f - 2;
        Rect secondHalf = firstHalf;
        secondHalf.x += secondHalf.width + 4;

        // draw the "key" text field
        m_blackboard.SetKey(key, EditorGUI.TextField(firstHalf, key), valueType);

        // draw different fields depending on the value type
        switch (valueType)
        {
            case ValueType.Boolean: // toggle
                secondHalf.x = rect.width;
                secondHalf.width = 16;
                m_blackboard.SetValue(key, EditorGUI.Toggle(secondHalf, m_blackboard.GetValue(key).m_boolean));
                break;

            case ValueType.Float: // float field
                m_blackboard.SetValue(key, EditorGUI.FloatField(secondHalf, m_blackboard.GetValue(key).m_float));
                break;

            case ValueType.String: // string field
                m_blackboard.SetValue(key, EditorGUI.TextField(secondHalf, m_blackboard.GetValue(key).m_string));
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueType"></param>
    private void AddElement(ValueType valueType)
    {
        string keyToAdd = "";

        switch (valueType)
        {
            case ValueType.Boolean:
                keyToAdd = "Boolean " + m_blackboard.NewBooleanID();
                break;

            case ValueType.Float:
                keyToAdd = "Float " + m_blackboard.NewFloatID();
                break;

            case ValueType.String:
                keyToAdd = "String " + m_blackboard.NewStringID();
                break;
        }

        m_blackboard.AddKey(keyToAdd, valueType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="valueType"></param>
    private void RemoveElement(int index, ValueType valueType)
    {
        // display warning when attempting removal
        if (!EditorUtility.DisplayDialog("Wait!",
            "Are you sure you want to delete this variable?", "Yes", "No"))
            return;

        string keyToRemove = "";

        switch (valueType)
        {
            case ValueType.Boolean: // remove element from boolean list
                keyToRemove = m_blackboard.GetBooleans()[index].m_key;
                break;

            case ValueType.Float: // remove element from float list
                keyToRemove = m_blackboard.GetFloats()[index].m_key;
                break;

            case ValueType.String: // remove element from string list
                keyToRemove = m_blackboard.GetStrings()[index].m_key;
                break;
        }

        m_blackboard.RemoveKey(keyToRemove, valueType);
    }

    /// <summary>
    /// draws the DEVN water-mark logo. For fanciness
    /// </summary>
    private void DrawLogo()
    {
        float xPosLogo = GUILayoutUtility.GetLastRect().width - 80;
        float yPosLogo = GUILayoutUtility.GetLastRect().height - 36;
        float xPosText = xPosLogo - 118;
        float yPosText = yPosLogo + 40;

        // adjust transparency
        GUI.color = new Color(1, 1, 1, 0.25f);

        // draw logo & text
        GUI.DrawTexture(new Rect(xPosLogo, yPosLogo, 100, 50), m_logoDEVN);
        GUI.Label(new Rect(xPosText, yPosText, 300, 20), "Dialogue Editor for Visual Novels");

        // reset transparency
        GUI.color = new Color(1, 1, 1, 1);
    }
}

#endif