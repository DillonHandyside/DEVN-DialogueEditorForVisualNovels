using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.Editor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// dialogue node which stores all of the relevant dialogue variables needed for each in-game line, such as 
/// the speaking character, their sprite and audio, and the dialogue itself
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
    [SerializeField] private string m_dialogue = "";
            
    private int m_newDialogueID;
    private bool m_newDialogueCreated = false;

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
	/// overridden copy constructor, copies relevant variables such as character object, sprite, audio and 
    /// the dialogue itself
	/// </summary>
	/// <param name="node">the node to copy</param>
	/// <param name="position">the position to copy to</param>
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
	/// overridden draw function, draws object fields for sprite, audio clips, etc. And draws text area for dialogue
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
	{ 
        // draw the sprite in the background, if one is selected
        DrawSpriteBackground();
		DrawCharacterObjectField(); // draw character object field

		// if a character is selected, draw all the other node stuffs
		if (m_character != null)
		{
			DrawSpritePopup(); // sprite selection
			DrawAudioPopup(); // audio selection
            
            if (m_sprite != null)
                EditorGUILayout.EndVertical();
            
			DrawDialogueTextArea(); // dialogue text area
		}

        // draw a button which allows the user to easily create new, already-connected dialogue nodes
        if (GUI.Button(new Rect(m_rectangle.width - 40, 0, 20, 15), "+"))
            CreateNextDialogue();
		
        // resize node
        if (Event.current.type == EventType.Repaint)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            m_rectangle.height = lastRect.y + lastRect.height + 4;
        }

        base.DrawNodeWindow(id);
    }
            
    /// <summary>
    /// function which draws a semi-transparent sprite in the background of the node
    /// </summary>
    private void DrawSpriteBackground()
    {
		if (m_sprite == null)
		    return; // no sprite selected, don't draw
		
		// determine sprite width and height
		float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
		float spriteWidth = m_rectangle.width;
		float spriteHeight = spriteWidth / aspectRatio;

        // draw sprite
        GUI.color = new Color(1, 1, 1, 0.45f);
		GUI.DrawTexture(new Rect(72, 16, spriteWidth, spriteHeight), m_sprite.texture);
        GUI.color = Color.white;
                
        // begin vertical layout such that the rest of the GUI elements are half the width of the node
        EditorGUILayout.BeginVertical(GUILayout.Width(m_rectangle.width * 0.5f));
    }

	/// <summary>
	/// function which draws an object field for selecting a character asset
	/// </summary>
	private void DrawCharacterObjectField()
	{
		// draw label & object field
		EditorGUILayout.LabelField("Character");
		m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;

        if (m_character == null)
            m_sprite = null; // reset sprite if no character is selected
	}
            
    /// <summary>
    /// function which draws a dropdown list for all available sprites
    /// </summary>
	private void DrawSpritePopup()
	{
		// get sprites and initialise sprite name array
		List<Sprite> sprites = m_character.GetSprites();
		string[] spriteNames = new string[sprites.Count + 1];
		spriteNames[0] = "None"; // allow user to select "None" for no sprite change

        // build sprite name list
		for (int i = 0; i < sprites.Count; i++)
		{
			Sprite sprite = sprites[i];

            // log an error if any null sprites are found
			if (sprite == null)
				Debug.LogError("DEVN: Character contains null sprites!");
			else
				spriteNames[i + 1] = sprite.name;
		}
        
		// draw label and drop-down sprite selection menu
		EditorGUILayout.LabelField("Sprite Change");
		m_spriteSelection = EditorGUILayout.Popup(m_spriteSelection, spriteNames);

        // if "None" is selected, set sprite to null
        if (m_spriteSelection == 0)
            m_sprite = null; 
        else
            m_sprite = sprites[m_spriteSelection - 1];

		// clamp to prevent index out of range errors
		m_spriteSelection = Mathf.Clamp(m_spriteSelection, 0, sprites.Count);
	}

	/// <summary>
	/// function which draws a dropdown list for all available audio clips
	/// </summary>
	private void DrawAudioPopup()
	{
		// get audio clips and audio clip names
		List<AudioClip> characterAudio = m_character.GetAudioClips();
		string[] audioNames = new string[characterAudio.Count + 1];
		audioNames[0] = "None"; // allow user to select "None" for no audio

        // build audio name list
		for (int i = 0; i < characterAudio.Count; i++)
		{
			AudioClip audio = characterAudio[i];

            // log an error if any null audio is found
			if (audio == null)
				Debug.LogError("DEVN: Character contains null audio!");
			else
				audioNames[i + 1] = audio.name;
		}

		// draw label and drop-down audio selection menu
		EditorGUILayout.LabelField("Character Audio");
		m_audioSelection = EditorGUILayout.Popup(m_audioSelection, audioNames);

		// clamp to prevent index out of range errors
		m_audioSelection = Mathf.Clamp(m_audioSelection, 0, characterAudio.Count);
			
		if (m_audioSelection == 0 || characterAudio[m_audioSelection - 1] == null)
			m_characterAudio = null; // audio selection is "None"
		else 
			m_characterAudio = characterAudio[m_audioSelection - 1];
	}

	/// <summary>
	/// function which draws a dialogue text input field
	/// </summary>
	private void DrawDialogueTextArea()
	{
		// draw label
		EditorGUILayout.LabelField("Dialogue");

		// draw text area
        GUI.SetNextControlName("Dialogue " + m_nodeID);
		m_dialogue = EditorGUILayout.TextArea(m_dialogue, GUILayout.MaxHeight(42));

        // if a new dialogue box was created last frame, then automatically focus on it's text field
        if (m_newDialogueCreated)
        {
            EditorGUI.FocusTextInControl("Dialogue " + m_newDialogueID);
            m_newDialogueCreated = false;
        }
        
        if (m_dialogue.Length > 0)
        {
            // upon pressing "tab" when editing dialogue, a new dialogue node will be created automatically
            if (m_dialogue[m_dialogue.Length - 1] == '\t')
            {
                m_dialogue = m_dialogue.Remove(m_dialogue.Length - 1); // remove the tab to prevent infinite loop
                CreateNextDialogue(); // create the new dialogue
            }
        }
	}

    /// <summary>
    /// quality of life function, used to instantly create and connect a new dialogue node to this one
    /// </summary>
    private void CreateNextDialogue()
    {
        // get node and connection managers
        NodeManager nodeManager = NodeEditor.GetNodeManager();
        ConnectionManager connectionManager = NodeEditor.GetConnectionManager();

        // create the new dialogue node and connect it with this node
        DialogueNode dialogueNode = nodeManager.AddNode(typeof(DialogueNode)) as DialogueNode;
        connectionManager.CreateLinearConnection(this, dialogueNode);

        // adjust it's position to be slightly to the right of this dialogue node
        dialogueNode.m_rectangle = m_rectangle;
        dialogueNode.m_rectangle.x += m_rectangle.width + 48;
                
        dialogueNode.m_character = m_character; // copy speaking character

        // save focus-relevant variables (used to auto-focussing on the new dialogue text area)
        m_newDialogueID = dialogueNode.m_nodeID;
        m_newDialogueCreated = true;
    }
	
#endif
}

}

}
