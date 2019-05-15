using System.Collections.Generic;
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
	[SerializeField] private int m_spriteSelection = 0;
		
	[SerializeField] private float m_xPosition = 50.0f; // character alignment
	[SerializeField] private float m_fadeTime = 0.5f; // fade time
	[SerializeField] private bool m_isInverted = false; // y scale invert?
	[SerializeField] private bool m_waitForFinish = true; // wait before next node?

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public float GetXPosition() { return m_xPosition; }
	public float GetFadeTime() { return m_fadeTime; }
	public bool GetIsInverted() { return m_isInverted; }
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

        m_title = "Character";

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
		m_spriteSelection = characterNode.m_spriteSelection;

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
		float fieldWidth = 160;
		float fieldHeight = 16;
		float windowWidth = fieldWidth;
			
        Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

		// enter/exit toggle
		m_toggleSelection = EditorGUI.Popup(fieldRect, m_toggleSelection, m_toggle);
        fieldRect.y += fieldHeight;

		// character
		GUI.Label(fieldRect, "Character");
        fieldRect.y += fieldHeight;
		m_character = EditorGUI.ObjectField(fieldRect, m_character, typeof(Character), false) as Character;
        fieldRect.y += fieldHeight;

		if (m_character != null)
		{
			// draw sprite label
			GUI.Label(fieldRect, "Sprite");
			fieldRect.y += fieldHeight;

			// get sprites and sprite names
			List<Sprite> sprites = m_character.m_sprites;
			string[] spriteNames = new string[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i] == null)
                    spriteNames[i] = "None";
                else
                    spriteNames[i] = sprites[i].name;
            }

			// draw drop-down sprite selection menu
			m_spriteSelection = EditorGUI.Popup(fieldRect, m_spriteSelection, spriteNames);
			fieldRect.y += fieldHeight + 2;

			if (spriteNames[m_spriteSelection] != "None" && spriteNames[m_spriteSelection] != null)
			{
				// determine sprite width and height
				m_sprite = sprites[m_spriteSelection];
				float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
				float spriteHeight = m_rectangle.height - 24;
				float spriteWidth = spriteHeight * aspectRatio;
				windowWidth += spriteWidth + 4;

				// draw sprite
				Rect spriteRect = new Rect(fieldWidth + 10, 20, spriteWidth, spriteHeight);
				GUI.Box(spriteRect, m_sprite.texture);

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

				Rect toggleRect = new Rect(fieldWidth - 10, fieldRect.y, fieldHeight, fieldHeight);

				if (m_toggleSelection == 0)
				{
					// inverted?
					GUI.Label(fieldRect, "Invert");
					m_isInverted = EditorGUI.Toggle(toggleRect, m_isInverted);
					fieldRect.y += fieldHeight;
					toggleRect.y += fieldHeight;
				}

				// wait for finish?
				GUI.Label(fieldRect, "Wait For Finish");
				m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);
				fieldRect.y += fieldHeight;
			}
		}

		m_rectangle.width = windowWidth + 12;
		m_rectangle.height = fieldRect.y + 4;

		base.DrawNodeWindow(id);
    }

#endif
}

}
