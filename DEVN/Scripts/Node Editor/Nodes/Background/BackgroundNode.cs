using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// node which determines background behaviour, including enter/exit,
/// fade time and colour
/// </summary>
public class BackgroundNode : BaseNode
{
    // enter/exit pop up menu
    private string[] m_toggle = { "Enter", "Exit" };
    [SerializeField] private int m_toggleSelection = 0;
		
    [SerializeField] private Sprite m_background; // background sprite

	// background colour during fade-in/fade-out
	[SerializeField] private Color m_fadeColour = Color.black;

    // background fade-in/fade-out time
    [SerializeField] private float m_fadeTime = 0.5f;

	// node height used for resizing node
	[SerializeField] private float m_nodeHeight = 0;

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Sprite GetBackground() { return m_background; }
	public Color GetFadeColour() { return m_fadeColour; }
	public float GetFadeTime() { return m_fadeTime; }

		#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Background";

        m_rectangle.width = 170;
        m_rectangle.height = 108;

		m_nodeHeight = m_rectangle.height;

        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor, background and other associated 
	/// properties
	/// </summary>
	/// <param name="node">node to copy</param>
	/// <param name="position">position to copy to</param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		BackgroundNode backgroundNode = node as BackgroundNode;

		// copy enter/exit toggle
		m_toggleSelection = backgroundNode.m_toggleSelection;

		// copy sprite selection
		m_background = backgroundNode.m_background; 

		// copy fade attributes
		m_fadeColour = backgroundNode.m_fadeColour;
		m_fadeTime = backgroundNode.m_fadeTime;

		m_nodeHeight = backgroundNode.m_nodeHeight;
	}

	/// <summary>
	/// overridden draw function
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
	{
		m_rectangle.height = m_nodeHeight;

		float width = m_rectangle.width - 10.0f;
        float fieldHeight = 16;

		Rect fieldRect = new Rect(5, 20, width, fieldHeight);

		// enter/exit toggle
		m_toggleSelection = EditorGUI.Popup(fieldRect, m_toggleSelection, m_toggle);
		fieldRect.y += fieldHeight + 2;

		// if "Enter" is selected, show background sprite object field
		if (m_toggleSelection == 0)
		{
			Rect spriteRect = fieldRect;
			spriteRect.height = 90;

			// adjust node window height
			m_rectangle.height = m_nodeHeight + spriteRect.height;

			// draw background object field
			m_background = EditorGUI.ObjectField(spriteRect, m_background, typeof(Sprite), false) as Sprite;
			fieldRect.y += spriteRect.height;
		}

		// fade colour field
		GUI.Label(fieldRect, "Fade Colour");
        fieldRect.y += fieldHeight;
        m_fadeColour = EditorGUI.ColorField(fieldRect, m_fadeColour);
        fieldRect.y += fieldHeight;

        // fade time
        GUI.Label(fieldRect, "Fade Time");
        fieldRect.y += fieldHeight;
        m_fadeTime = EditorGUI.Slider(fieldRect, m_fadeTime, 0.01f, 3);

        base.DrawNodeWindow(id);
    }

#endif
}

}
