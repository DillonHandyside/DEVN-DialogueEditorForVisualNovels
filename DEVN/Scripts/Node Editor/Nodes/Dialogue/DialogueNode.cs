using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// dialogue node which stores all of the relevant dialogue variables
/// needed for each in-game line, such as the speaking character, 
/// their sprite and audio, and the dialogue itself
/// </summary>
[System.Serializable]
public class DialogueNode : BaseNode
{
	// the speaking character
    [SerializeField] private Character m_character;

    // sprite variables
    private Sprite m_sprite;
	[SerializeField] private int m_spriteSelection;

    // audio variables
    private AudioClip m_characterAudio;
	[SerializeField] private int m_audioSelection;
	//[SerializeField] private AudioClip m_dialogueAudio;

    // dialogue
    [SerializeField] private string m_dialogue;

	#region getters

	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public AudioClip GetCharacterAudio() { return m_characterAudio; }
	//public AudioClip GetDialogueAudio() { return m_dialogueAudio; }
	public string GetDialogue() { return m_dialogue; }

		#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Dialogue";

        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor, copies relevant variables such as
	/// character object, sprite, audio and the dialogue itself
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		DialogueNode dialogueNode = node as DialogueNode;

		// copy character object and sprite
		m_character = dialogueNode.m_character;
		m_spriteSelection = dialogueNode.m_spriteSelection;

		// copy audio clips
		m_audioSelection = dialogueNode.m_audioSelection;
		//m_dialogueAudio = dialogueNode.m_dialogueAudio;

		// copy dialogue
		m_dialogue = dialogueNode.m_dialogue;
	}

	/// <summary>
	/// overridden draw function, draws object fields for sprite, audio 
	/// clips, etc. And draws text area for dialogue
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
    {
        float fieldWidth = 280;
        float fieldHeight = 16;
		float windowWidth = fieldWidth;
			
		Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

        // character
        GUI.Label(fieldRect, "Character");
		fieldRect.y += fieldHeight;
        m_character = EditorGUI.ObjectField(fieldRect, m_character, typeof(Character), false) as Character;
		fieldRect.y += fieldHeight;

		if (m_character != null)
		{
			// draw sprite label
			GUI.Label(fieldRect, "Sprite Change");
			fieldRect.y += fieldHeight;

			// get sprites and sprite names
			List<Sprite> sprites = m_character.m_sprites;
			string[] spriteNames = new string[sprites.Count + 1];
			spriteNames[0] = "None";
			for (int i = 0; i < sprites.Count; i++)
				spriteNames[i + 1] = sprites[i].name;

			// draw drop-down sprite selection menu
			m_spriteSelection = EditorGUI.Popup(fieldRect, m_spriteSelection, spriteNames);
			fieldRect.y += fieldHeight + 2;

			if (m_spriteSelection != 0)
			{
				// determine sprite width and height
				m_sprite = sprites[m_spriteSelection - 1];
				float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
				float spriteHeight = m_rectangle.height - 24;
				float spriteWidth = spriteHeight * aspectRatio;
				windowWidth += spriteWidth + 4;

				// draw sprite
				Rect spriteRect = new Rect(fieldWidth + 10, 20, spriteWidth, spriteHeight);
				GUI.Box(spriteRect, m_sprite.texture);
			}

			// character audio
			GUI.Label(fieldRect, "Character Audio");
			fieldRect.y += fieldHeight;

			List<AudioClip> characterAudio = m_character.m_audioClips;
			string[] audioNames = new string[characterAudio.Count + 1];
			audioNames[0] = "None";
			for (int i = 0; i < characterAudio.Count; i++)
				audioNames[i + 1] = characterAudio[i].name;

			m_audioSelection = EditorGUI.Popup(fieldRect, m_audioSelection, audioNames);
			fieldRect.y += fieldHeight + 2;

			if (m_audioSelection > 1)
				m_characterAudio = characterAudio[m_audioSelection - 1];

			// dialogue audio
			//GUI.Label(fieldRect, "Dialogue Audio");
			//fieldRect.y += fieldHeight;
			//m_dialogueAudio = EditorGUI.ObjectField(fieldRect, m_dialogueAudio, typeof(AudioClip), false) as AudioClip;
			//fieldRect.y += fieldHeight;

			// dialogue
			GUI.Label(fieldRect, "Dialogue");
			fieldRect.y += fieldHeight;
			fieldRect.height = 48;
			m_dialogue = EditorGUI.TextArea(fieldRect, m_dialogue);
			fieldRect.y += fieldHeight * 3;
		}

		m_rectangle.width = windowWidth + 12;
		m_rectangle.height = fieldRect.y + 4;

        base.DrawNodeWindow(id);
    }

#endif
}

}
