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

    // dialogue
    [SerializeField] private string m_dialogue;

	private Rect m_fieldRect;
	private float m_spriteWidth;

	#region getters

	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public AudioClip GetCharacterAudio() { return m_characterAudio; }
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

		m_rectangle.width = 340;

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

		// copy character object
		m_character = dialogueNode.m_character;
		m_spriteSelection = dialogueNode.m_spriteSelection; // copy sprite
		m_audioSelection = dialogueNode.m_audioSelection; // copy audio

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
		m_fieldRect = new Rect(5, 20, m_rectangle.width - m_spriteWidth - 14, 16);
		m_spriteWidth = 0;

		// character
		DrawCharacterObjectField();

		// if a character is selected, draw all the other node stuffs
		if (m_character != null)
		{
			// sprite
			DrawSpritePopup();

			// audio
			DrawAudioPopup();

			// dialogue
			DrawDialogueTextArea();
		}
		
		// resize window height
		m_rectangle.height = m_fieldRect.y + 4;

        base.DrawNodeWindow(id);
    }

	/// <summary>
	/// 
	/// </summary>
	private void DrawCharacterObjectField()
	{
		// draw label
		GUI.Label(m_fieldRect, "Character");
		m_fieldRect.y += m_fieldRect.height;

		// draw object field
		m_character = EditorGUI.ObjectField(m_fieldRect, m_character, typeof(Character), false) as Character;
		m_fieldRect.y += m_fieldRect.height;
	}

	/// <summary>
	/// 
	/// </summary>
	private void DrawSpritePopup()
	{
		// draw sprite label
		GUI.Label(m_fieldRect, "Sprite Change");
		m_fieldRect.y += m_fieldRect.height;

		// get sprites and sprite names
		List<Sprite> sprites = m_character.m_sprites;
		string[] spriteNames = new string[sprites.Count + 1];
		spriteNames[0] = "None";
		for (int i = 0; i < sprites.Count; i++)
			spriteNames[i + 1] = sprites[i].name;

		// draw drop-down sprite selection menu
		m_spriteSelection = EditorGUI.Popup(m_fieldRect, m_spriteSelection, spriteNames);
		m_fieldRect.y += m_fieldRect.height + 2;
			
		if (m_spriteSelection == 0)
			m_sprite = null; // sprite selection is "None"
		else 
		{
			// determine sprite width and height
			m_sprite = sprites[m_spriteSelection - 1];
			float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
			float spriteHeight = /*m_rectangle.height - 24*/ m_fieldRect.height * 7;
			m_spriteWidth = spriteHeight * aspectRatio;

			// draw sprite
			Rect spriteRect = new Rect(m_fieldRect.width + 8, 20, m_spriteWidth, spriteHeight);
			GUI.Box(spriteRect, m_sprite.texture);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private void DrawAudioPopup()
	{
		// character audio
		GUI.Label(m_fieldRect, "Character Audio");
		m_fieldRect.y += m_fieldRect.height;

		// get audio clips and audio clip names
		List<AudioClip> characterAudio = m_character.m_audioClips;
		string[] audioNames = new string[characterAudio.Count + 1];
		audioNames[0] = "None";
		for (int i = 0; i < characterAudio.Count; i++)
			audioNames[i + 1] = characterAudio[i].name;

		// draw drop-down audio selection menu
		m_audioSelection = EditorGUI.Popup(m_fieldRect, m_audioSelection, audioNames);
		m_fieldRect.y += m_fieldRect.height + 2;
			
		if (m_audioSelection == 0)
			m_characterAudio = null; // audio selection is "None"
		else 
			m_characterAudio = characterAudio[m_audioSelection - 1];
	}

	/// <summary>
	/// 
	/// </summary>
	private void DrawDialogueTextArea()
	{
		// draw label
		GUI.Label(m_fieldRect, "Dialogue");
		m_fieldRect.y += m_fieldRect.height;

		// adjust rectangle to window width and 3x the height
		m_fieldRect.width = m_rectangle.width - 10;
		m_fieldRect.height = m_fieldRect.height * 3;

		// draw text area
		m_dialogue = EditorGUI.TextArea(m_fieldRect, m_dialogue);
		m_fieldRect.y += m_fieldRect.height;
	}
	
#endif
}

}
