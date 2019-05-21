using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

#if UNITY_EDITOR

using UnityEditorInternal;

namespace DEVN
{

namespace Editor
{

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
    /// labels and reorderable lists
    /// </summary>
    void OnGUI()
    {
		Blackboard previousBlackboard = m_blackboard;
            
		EditorGUILayout.Space();
        m_blackboard = EditorGUILayout.ObjectField("Current Blackboard", m_blackboard, typeof(Blackboard), false) 
                       as Blackboard; // title and blackboard object field

		// only draw blackboard contents/elements if a blackboard is selected
		if (m_blackboard != null)
		{
            // re-initialise reorderable lists if the blackboard was switched
			if (previousBlackboard != m_blackboard)
				InitialiseReorderableLists();

			DrawContent(); // draw scroll view and reorderable lists
			DrawLogo(); // draw DEVN watermark

			EditorUtility.SetDirty(m_blackboard); // save the blackboard changes
		}
    }

	/// <summary>
	/// function which begins a scrollview and draws each reorderable list
	/// </summary>
	private void DrawContent()
    {
		m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
        
        // draw reorderable lists for booleans, floats and strings
		DrawReorderableList(m_booleanList, ValueType.Boolean);
		DrawReorderableList(m_floatList, ValueType.Float);
		DrawReorderableList(m_stringList, ValueType.String);
		
		EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// helper function which initialises new reorderable lists for each variable type, when necessary
    /// </summary>
	private void InitialiseReorderableLists()
	{
		m_booleanList = new ReorderableList(m_blackboard.GetBooleans(), typeof(KeyValue), false, true, true, true);
		m_floatList = new ReorderableList(m_blackboard.GetFloats(), typeof(KeyValue), false, true, true, true);
		m_stringList = new ReorderableList(m_blackboard.GetStrings(), typeof(KeyValue), false, true, true, true);
	}

    /// <summary>
    /// draw function responsible for drawing a reorderable list. Assigns the appropriate callback functions
    /// for drawing the header, drawing the list elements, and performing adding and removal
    /// </summary>
    /// <param name="reorderableList">the reorderablelist to draw</param>
    /// <param name="valueType">the value type of the list, e.g. Boolean, Float, etc.</param>
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

		// format margins such that the reorderable list doesn't touch the sides of the window
		GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
		scrollViewStyle.margin = new RectOffset(8, 8, 8, 8);

		// wrap the list in a vertical group with margins, then draw the list
		EditorGUILayout.BeginVertical(scrollViewStyle);
		reorderableList.DoLayoutList();
		EditorGUILayout.EndVertical();
	}

    /// <summary>
    /// header callback function which draws a reorderable list header label in bold
    /// </summary>
    /// <param name="rect">the positional values of the reorderable list header</param>
    /// <param name="valueType">the value type to draw, e.g. Boolean, Float, etc.</param>
    private void DrawHeader(Rect rect, ValueType valueType)
    {
        // print header as "Booleans", "Floats", etc.
        string label = valueType.ToString() + "s";
        EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);
    }

    /// <summary>
    /// draw callback function which draws a reorderable list element. Draws the key value pairing of a
    /// blackboard element in input fields like so: [ Key (variable name) ] [ Value (variable value) ]
    /// </summary>
    /// <param name="rect">the positional values of the reorderable list element</param>
    /// <param name="index">the index of the list element</param>
    /// <param name="key">the key (variable name) of the particular blackboard element</param>
    /// <param name="valueType">the value type of the blackboard element, e.g. Boolean, Float, etc.</param>
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
            case ValueType.Boolean: // boolean toggle
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
    /// add callback function which adds a new element to a blackboard
    /// </summary>
    /// <param name="valueType">the value type of the element to add, e.g. Boolean, Float, etc.</param>
    private void AddElement(ValueType valueType)
    {
        string keyToAdd;

        // depending on the valueType, add a new Boolean, Float or String, with a unique ID to prevent duplicate keys
        switch (valueType)
        {
            default:
                keyToAdd = "Boolean " + m_blackboard.NewBooleanID();
                break;

            case ValueType.Float:
                keyToAdd = "Float " + m_blackboard.NewFloatID();
                break;

            case ValueType.String:
                keyToAdd = "String " + m_blackboard.NewStringID();
                break;
        }

        m_blackboard.AddKey(keyToAdd, valueType); // perform add
    }

    /// <summary>
    /// remove callback function which removes a particular element from a blackboard
    /// </summary>
    /// <param name="index">the index of the element to remove</param>
    /// <param name="valueType">the value type of the element to remove, e.g. Boolean, Float, etc.</param>
    private void RemoveElement(int index, ValueType valueType)
    {
        // display warning when attempting removal
        if (!EditorUtility.DisplayDialog("Wait!",
            "Are you sure you want to delete this variable?", "Yes", "No"))
            return;

        string keyToRemove;

        // depending on the valueType, access the corresponding list and find the key to remove
        switch (valueType)
        {
            default:
                keyToRemove = m_blackboard.GetBooleans()[index].m_key;
                break;

            case ValueType.Float:
                keyToRemove = m_blackboard.GetFloats()[index].m_key;
                break;

            case ValueType.String:
                keyToRemove = m_blackboard.GetStrings()[index].m_key;
                break;
        }

        m_blackboard.RemoveKey(keyToRemove, valueType); // perform removal
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

        GUI.color = new Color(1, 1, 1, 0.25f); // adjust transparency

        // draw logo & text
        GUI.DrawTexture(new Rect(xPosLogo, yPosLogo, 100, 50), m_logoDEVN);
        GUI.Label(new Rect(xPosText, yPosText, 300, 20), "Dialogue Editor for Visual Novels");

        GUI.color = new Color(1, 1, 1, 1); // reset transparency
    }
}

}

}

#endif