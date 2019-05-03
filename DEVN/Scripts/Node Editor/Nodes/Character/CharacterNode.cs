using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// node used to determine the entering and exiting of a character, including
/// position, fade time and default sprite
/// </summary>
[System.Serializable]
public class CharacterNode : BaseNode
{
    // enter/exit pop up menu
    private string[] m_toggle = { "Enter", "Exit" };
	[SerializeField] private int m_toggleSelection = 0;

	// the entering/exiting character
	[SerializeField] private Character m_character;

	// the desired sprite on entry/exit
	[SerializeField] private Sprite m_sprite;

	
	[SerializeField] private float m_xPosition = 50.0f; // character alignment
	[SerializeField] private float m_fadeTime = 0.5f; // fade time
	[SerializeField] private bool m_isInverted = false; // inversion

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public float GetXPosition() { return m_xPosition; }
	public float GetFadeTime() { return m_fadeTime; }
	public bool GetIsInverted() { return m_isInverted; }

		#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Character";

        m_rectangle.width = 272;
        m_rectangle.height = 154;

        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor, copies relevant character variables
	/// such as the default sprite, position and fade time
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		CharacterNode characterNode = node as CharacterNode;

		// copy enter/exit toggle
		m_toggleSelection = characterNode.m_toggleSelection;

		// copy character object and default sprite
		m_character = characterNode.m_character;
		m_sprite = characterNode.m_sprite;

		// copy position, fade time and invert status
		m_xPosition = characterNode.m_xPosition;
		m_fadeTime = characterNode.m_fadeTime;
		m_isInverted = characterNode.m_isInverted;
	}

	/// <summary>
	/// overridden draw function, draws character object field, sprite
	/// object field, dropdowns and sliders etc.
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
    {
		float spriteWidth = m_rectangle.width / 3;
		float spriteHeight = m_rectangle.height - 24;
		float fieldWidth = m_rectangle.width - spriteWidth - 16;
		float fieldHeight = 16;

        Rect spriteRect = new Rect(5, 20, spriteWidth, spriteHeight);
        Rect fieldRect = new Rect(spriteWidth + 10, 20, fieldWidth, fieldHeight);

		// sprite
		m_sprite = EditorGUI.ObjectField(spriteRect, m_sprite, typeof(Sprite), false) as Sprite;

		// enter/exit toggle
		m_toggleSelection = EditorGUI.Popup(fieldRect, m_toggleSelection, m_toggle);
        fieldRect.y += fieldHeight;

		// character
		GUI.Label(fieldRect, "Character");
        fieldRect.y += fieldHeight;
		m_character = EditorGUI.ObjectField(fieldRect, m_character, typeof(Character), false) as Character;
        fieldRect.y += fieldHeight;

		// alignment
        GUI.Label(fieldRect, "X Position (%)");
        fieldRect.y += fieldHeight;
        m_xPosition = EditorGUI.Slider(fieldRect, m_xPosition, 0.0f, 100.0f);
        fieldRect.y += fieldHeight;

        // fade time
        GUI.Label(fieldRect, "Fade Time");
		fieldRect.y += fieldHeight;
		m_fadeTime = EditorGUI.Slider(fieldRect, m_fadeTime, 0.0f, 3.0f);
		fieldRect.y += fieldHeight + 2;

		// inverted?
		GUI.Label(fieldRect, "Invert");
		fieldRect.x = m_rectangle.width - 20;
		fieldRect.width = fieldHeight;
		m_isInverted = EditorGUI.Toggle(fieldRect, m_isInverted);

		base.DrawNodeWindow(id);
    }

#endif
}

}
