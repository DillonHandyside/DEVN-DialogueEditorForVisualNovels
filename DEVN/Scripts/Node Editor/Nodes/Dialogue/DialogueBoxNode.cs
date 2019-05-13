using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// node used to toggle (show/hide) the dialogue box
/// </summary>
[System.Serializable]
public class DialogueBoxNode : BaseNode
{
    // show/hide pop up menu variables
    private string[] m_toggle = { "Show", "Hide" };
    [SerializeField] private int m_toggleSelection = 0;

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }

		#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Dialogue Box";

        m_rectangle.width = 128;
        m_rectangle.height = 40;

        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor. Copies whether the dialogue
	/// box is set to 'show' or 'hide'
	/// </summary>
	/// <param name="node">the node to copy</param>
	/// <param name="position">the position to copy to</param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		DialogueBoxNode dialogueBoxNode = node as DialogueBoxNode;

		// copy show/hide selection
		m_toggleSelection = dialogueBoxNode.m_toggleSelection;
	}

	/// <summary>
	/// overridden draw function, draws a dropdown for toggling between
	/// hide/show dialogue node
	/// </summary>
	/// <param name="id"></param>
	protected override void DrawNodeWindow(int id)
    {
        // draw show/hide toggle
		m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);

        base.DrawNodeWindow(id);
    }

#endif
}

}
