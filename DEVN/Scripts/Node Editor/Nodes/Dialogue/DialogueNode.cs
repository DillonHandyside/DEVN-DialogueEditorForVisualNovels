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
    [SerializeField] private Sprite m_sprite;

    // audio variables
    [SerializeField] private AudioClip m_characterAudio;
	[SerializeField] private AudioClip m_dialogueAudio;

    // dialogue
    [SerializeField] private string m_dialogue;

	#region getters

	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public AudioClip GetCharacterAudio() { return m_characterAudio; }
	public AudioClip GetDialogueAudio() { return m_dialogueAudio; }
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

        m_rectangle.width = 364;
        m_rectangle.height = 186;

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
		m_sprite = dialogueNode.m_sprite;

		// copy audio clips
		m_characterAudio = dialogueNode.m_characterAudio;
		m_dialogueAudio = dialogueNode.m_dialogueAudio;

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
        float spriteWidth = m_rectangle.width / 3.25f;
        float spriteHeight = m_rectangle.height - 25.0f;
        float fieldWidth = m_rectangle.width - spriteWidth - 16.0f;
        float fieldHeight = 16;

		Rect spriteRect = new Rect(5, 20, spriteWidth, spriteHeight);
		Rect fieldRect = new Rect(spriteWidth + 10, 20, fieldWidth, fieldHeight);

        // sprite
        m_sprite = EditorGUI.ObjectField(spriteRect, m_sprite, typeof(Sprite), false) as Sprite;

        // character
        GUI.Label(fieldRect, "Character");
		fieldRect.y += fieldHeight;
        m_character = EditorGUI.ObjectField(fieldRect, m_character, typeof(Character), false) as Character;
		fieldRect.y += fieldHeight;

        // character audio
        GUI.Label(fieldRect, "Character Audio");
		fieldRect.y += fieldHeight;
		m_characterAudio = EditorGUI.ObjectField(fieldRect, m_characterAudio, typeof(AudioClip), false) as AudioClip;
		fieldRect.y += fieldHeight;

		// dialogue audio
		GUI.Label(fieldRect, "Dialogue Audio");
		fieldRect.y += fieldHeight;
		m_dialogueAudio = EditorGUI.ObjectField(fieldRect, m_dialogueAudio, typeof(AudioClip), false) as AudioClip;
		fieldRect.y += fieldHeight;

		// dialogue
		GUI.Label(fieldRect, "Dialogue");
		fieldRect.y += fieldHeight;
		fieldRect.height = 48;
        m_dialogue = EditorGUI.TextArea(fieldRect, m_dialogue);

        base.DrawNodeWindow(id);
    }

#endif
}

}
