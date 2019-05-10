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

    // a collection of all user-created blackboards
    private List<Blackboard> m_blackboards;

    // selection variables
    private Blackboard m_blackboard;
    private int m_currentSelection = 0;

    // scroll view variables
    private Vector2 m_scrollPosition = Vector2.zero;
    private float m_scrollViewHeight = 32.0f;
	
	private ReorderableList m_booleanList;
	private ReorderableList m_floatList;
	private ReorderableList m_stringList;

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
		EditorGUILayout.Separator();

        // only draw blackboard contents/elements if a blackboard is selected
        if (m_blackboard != null)
		{
			if (previousBlackboard != m_blackboard)
				InitialiseReorderableLists();

			DrawScrollView();
		}

        DrawLogo(); // draw DEVN water-mark
		EditorUtility.SetDirty(m_blackboard);
		GameData.GetInstance().SetDefaultBlackboards(m_blackboards);
    }

	private void InitialiseReorderableLists()
	{
		m_booleanList = new ReorderableList(m_blackboard.m_booleans, typeof(KeyValue), true, true, true, true);
		m_floatList = new ReorderableList(m_blackboard.m_floats, typeof(KeyValue), true, true, true, true);
		m_stringList = new ReorderableList(m_blackboard.m_strings, typeof(KeyValue), true, true, true, true);
	}

	/// <summary>
	/// 
	/// </summary>
	private void DrawScrollView()
    {
		m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

        if (m_blackboard != null)
		{
			DrawReorderableList(m_booleanList, ValueType.Boolean);
			DrawReorderableList(m_floatList, ValueType.Float);
			DrawReorderableList(m_stringList, ValueType.String);
		}

		// end scroll view
		EditorGUILayout.EndScrollView();
    }

	private void DrawReorderableList(ReorderableList reorderableList, ValueType valueType)
	{
		reorderableList.drawHeaderCallback = rect =>
		{
			EditorGUI.LabelField(rect, valueType.ToString() + "s", EditorStyles.boldLabel);
		};

		List<string> keys = m_blackboard.GetAllOfValueType(valueType);

		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			string key = keys[index];
			Rect firstHalf = rect;
			firstHalf.width = firstHalf.width * 0.5f - 2;
			Rect secondHalf = firstHalf;
			secondHalf.x += secondHalf.width + 4;

			m_blackboard.SetKey(key, EditorGUI.TextField(firstHalf, key), valueType);

			switch (valueType)
			{
				case ValueType.Boolean:
					secondHalf.x = rect.width + 8;
					secondHalf.width = 16;
					m_blackboard.SetValue(key, EditorGUI.Toggle(secondHalf, m_blackboard.GetValue(key).m_boolean));
					break;

				case ValueType.Float:
					m_blackboard.SetValue(key, EditorGUI.FloatField(secondHalf, m_blackboard.GetValue(key).m_float));
					break;

				case ValueType.String:
					m_blackboard.SetValue(key, EditorGUI.TextField(secondHalf, m_blackboard.GetValue(key).m_string));
					break;
			}
		};

		reorderableList.onAddCallback = (ReorderableList list) =>
		{
			string defaultKey = "";

			switch (valueType)
			{
				case ValueType.Boolean:
					defaultKey = "Boolean " + m_blackboard.m_booleans.Count;
					break;

				case ValueType.Float:
					defaultKey = "Float " + m_blackboard.m_floats.Count;
					break;

				case ValueType.String:
					defaultKey = "String " + m_blackboard.m_strings.Count;
					break;
			}

			m_blackboard.AddKey(defaultKey, valueType);
		};

		reorderableList.onRemoveCallback = (ReorderableList list) =>
		{
			if (EditorUtility.DisplayDialog("Wait!", 
				"Are you sure you want to delete this variable?", "Yes", "No"))
			{
				switch (valueType)
				{
					case ValueType.Boolean:
						m_blackboard.RemoveKey(m_blackboard.m_booleans[reorderableList.index].m_key);
						break;

					case ValueType.Float:
						m_blackboard.RemoveKey(m_blackboard.m_floats[reorderableList.index].m_key);
						break;

					case ValueType.String:
						m_blackboard.RemoveKey(m_blackboard.m_strings[reorderableList.index].m_key);
						break;
				}
			}
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