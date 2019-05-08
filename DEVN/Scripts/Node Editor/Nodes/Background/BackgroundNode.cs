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

	// background fade variables
	[SerializeField] private Color m_fadeColour = Color.black;
    [SerializeField] private float m_fadeTime = 0.5f;
	[SerializeField] private bool m_waitForFinish = true;

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Sprite GetBackground() { return m_background; }
	public Color GetFadeColour() { return m_fadeColour; }
	public float GetFadeTime() { return m_fadeTime; }
	public bool GetWaitForFinish() { return m_waitForFinish; }

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
		m_waitForFinish = backgroundNode.m_waitForFinish;
	}

	/// <summary>
	/// overridden draw function
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
	{
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
		fieldRect.y += fieldHeight;

		// if "Enter" is selected, show "wait for finish" toggle
		if (m_toggleSelection == 0)
		{
			// draw "wait for finish" label and toggle
			GUI.Label(fieldRect, "Wait For Finish");
			fieldRect.x = m_rectangle.width - 20;
			fieldRect.width = fieldHeight;
			m_waitForFinish = EditorGUI.Toggle(fieldRect, m_waitForFinish);
			fieldRect.y += fieldHeight;
		}

		// adjust node height
		m_rectangle.height = fieldRect.y + 4;

        base.DrawNodeWindow(id);
    }

#endif
}

}
