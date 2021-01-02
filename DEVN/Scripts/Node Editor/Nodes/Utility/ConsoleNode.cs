using UnityEngine;
using UnityEditor;

namespace DEVN
{

namespace Nodes
{

public enum ConsoleType
{
	Log,
	Warning,
	Error
}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class ConsoleNode : BaseNode
{
    // consoleType
	[SerializeField] private int m_consoleSelection;

    // message
    [SerializeField] private string m_message = "";
			
	#region getters

	public ConsoleType GetConsoleType() { return (ConsoleType)m_consoleSelection; }
	public string GetMessage() { return m_message; }

    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor
    /// </summary>
    /// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Console";

        m_rectangle.width = 200;
        m_rectangle.height = 92;

        AddOutputPoint();
    }

    /// <summary>
    /// overridden copy constructor
    /// </summary>
    /// <param name="node">node to copy</param>
    /// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

		ConsoleNode consoleNode = node as ConsoleNode;
                
		// copy console log
		m_consoleSelection = consoleNode.m_consoleSelection;
		m_message = consoleNode.m_message;
    }

    /// <summary>
    /// overridden draw function
    /// </summary>
    /// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
		DrawConsolePopup();
        DrawCommentTextArea();

		base.DrawNodeWindow(id);
	}

	/// <summary>
	/// function which draws a dropdown list for console type
	/// </summary>
	private void DrawConsolePopup()
	{
		int consoleTypeCount = System.Enum.GetValues(typeof(ConsoleType)).Length;
		string[] consoleTypes = new string[consoleTypeCount];

        // build console type list
		for (int i = 0; i < consoleTypeCount; i++)
		{
			ConsoleType consoleType = (ConsoleType)i;
			consoleTypes[i] = consoleType.ToString();
		}

		// draw label and drop-down console selection menu
		m_consoleSelection = EditorGUILayout.Popup(m_consoleSelection, consoleTypes);

		// clamp to prevent index out of range errors
		m_consoleSelection = Mathf.Clamp(m_consoleSelection, 0, consoleTypeCount);
	}

	/// <summary>
	/// function which draws a dialogue text input field
	/// </summary>
	private void DrawCommentTextArea()
	{
		// draw text area
        GUI.SetNextControlName("Comment " + m_nodeID);
		m_message = EditorGUILayout.TextArea(m_message, GUILayout.MaxHeight(46));
	}


#endif
}

}

}
