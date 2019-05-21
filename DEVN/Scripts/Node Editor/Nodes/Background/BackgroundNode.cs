using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// node which determines background behaviour, including enter/exit, fade time and colour
/// </summary>
public class BackgroundNode : BaseNode
{
    // enter/exit pop up menu
    private string[] m_toggle = { "Enter", "Exit" };
    [SerializeField] private int m_toggleSelection = 0;
		
    [SerializeField] private Background m_background;
    //[SerializeField] private Sprite m_background; // background sprite

	// background fade variables
	[SerializeField] private Color m_fadeColour = Color.black;
    [SerializeField] private float m_fadeTime = 0.5f;
	[SerializeField] private bool m_waitForFinish = true;

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Background GetBackground() { return m_background; }
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
	/// overridden copy constructor, background and other associated properties
	/// </summary>
	/// <param name="node">node to copy</param>
	/// <param name="position">position to copy to</param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		BackgroundNode backgroundNode = node as BackgroundNode;

		// copy enter/exit toggle
		m_toggleSelection = backgroundNode.m_toggleSelection;

		// copy background selection
		m_background = backgroundNode.m_background; 

		// copy fade attributes
		m_fadeColour = backgroundNode.m_fadeColour;
		m_fadeTime = backgroundNode.m_fadeTime;
		m_waitForFinish = backgroundNode.m_waitForFinish;
	}

	/// <summary>
	/// overridden draw function, draws relevant background fields, such as Enter/Exit popup, background object
    /// and preview, colour and fade time
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
	{
		// enter/exit toggle
		m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);
        
        // if "Enter" is selected, show background object field and preview
        if (m_toggleSelection == 0)
        {
            // background object field
            EditorGUILayout.LabelField("Background");
            m_background = EditorGUILayout.ObjectField(m_background, typeof(Background), false) as Background;

            // if a background is selected, display a preview of it
            if (m_background != null)
            {
                GUILayoutOption[] options = { GUILayout.Width(160), GUILayout.Height(90) };
                GUILayout.Box(m_background.GetBackground().texture, options);
            }
        }

		// fade colour field
		EditorGUILayout.LabelField("Fade Colour");
        m_fadeColour = EditorGUILayout.ColorField(m_fadeColour);

        // fade time
        EditorGUILayout.LabelField("Fade Time");
        m_fadeTime = EditorGUILayout.Slider(m_fadeTime, 0.01f, 3);

		// if "Enter" is selected, show "wait for finish" toggle
		if (m_toggleSelection == 0)
		{
			// draw "wait for finish" label and toggle
			EditorGUILayout.LabelField("Wait For Finish");
            Rect toggleRect = GUILayoutUtility.GetLastRect();
            toggleRect.x = m_rectangle.width - 20;

			m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);
		}
        
		// resize node
        if (Event.current.type == EventType.Repaint)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            m_rectangle.height = lastRect.y + lastRect.height + 4;
        }

        base.DrawNodeWindow(id);
    }

#endif
}

}

}
