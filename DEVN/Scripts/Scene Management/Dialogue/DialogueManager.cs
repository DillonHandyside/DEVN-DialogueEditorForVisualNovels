using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// singleton manager class responsible for managing all of the dialogue
/// nodes with-in a DEVN scene
/// </summary>
public class DialogueManager
{
	// scene manager ref
	private SceneManager m_sceneManager;

	// references to dialogue box UI elements
	private GameObject m_dialogueBox;
	private Text m_speaker;
	private Text m_dialogue;
		
	// reference to typewrite coroutine, so StopCoroutine can be used
	private IEnumerator m_typewriteEvent;

	// other text typing relevant variables
	private float m_textSpeed;
	private bool m_isTyping = false;

    private float m_autoSpeed;
    private bool m_isAutoEnabled = false;

	#region getters
		
	public bool GetIsTyping() { return m_isTyping; }

	#endregion

	#region setters

	public void SetDialogueSpeed(float speed) { m_textSpeed = speed; }
    public void SetAutoSpeed(float speed) { m_autoSpeed = speed; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sceneManager">reference to the scene manager instance</param>
	/// <param name="dialogueComponent">a dialogue component which houses the relevent UI elements</param>
	public DialogueManager(SceneManager sceneManager, DialogueComponent dialogueComponent)
	{
		m_sceneManager = sceneManager; // assign scene manager reference
			
		// assign references to all the relevant dialogue elements
		m_dialogueBox = dialogueComponent.GetDialogueBox();
		m_speaker = dialogueComponent.GetSpeaker();
		m_dialogue = dialogueComponent.GetDialogue();
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetDialogue(DialogueNode dialogueNode)
	{
		// allow input during dialogue, to allow skip & continue
		InputManager inputManager = m_sceneManager.GetInputManager();
		if (inputManager != null)
			m_sceneManager.GetInputManager().SetIsInputAllowed(true);
			
		// attempt to get this dialogue node's character, log an error if there is none
		Character character = dialogueNode.GetCharacter();
		Debug.Assert(character != null, "DEVN: Dialogue requires a speaking character!");
		
		// update speaker text field
		m_speaker.text = character.m_name; 
		m_sceneManager.StartCoroutine(m_typewriteEvent = TypewriteText(dialogueNode.GetDialogue()));

		Sprite currentSprite = dialogueNode.GetSprite();
		CharacterManager characterManager = m_sceneManager.GetCharacterManager();

		// only update character sprite if a character manager exists
		if (characterManager != null)
		{
			// find the character in the scene
			GameObject characterObject = characterManager.TryGetCharacter(character);
			
			if (currentSprite != null)
			{
				if (characterObject != null)
					characterManager.SetSprite(characterObject, currentSprite);
				else
					Debug.LogWarning("DEVN: Do not attempt to change the sprite of a character that is not in the scene.");
			}
			else if (characterObject != null)
				currentSprite = characterObject.GetComponent<Image>().sprite;
			else
				currentSprite = null;
		
			if (characterObject)
				characterManager.HighlightSpeakingCharacter(character);
		}

		// only log dialogue if a log exists
		LogManager logManager = m_sceneManager.GetLogManager();
		if (logManager != null)
			logManager.LogDialogue(currentSprite, character.m_name, dialogueNode.GetDialogue());
	}

	/// <summary>
	/// 
	/// </summary>
	public void ToggleDialogueBox()
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(!m_dialogueBox.activeSelf);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="toggle"></param>
	public void ToggleDialogueBox(bool toggle)
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(toggle);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="dialogueBoxNode"></param>
	public void SetDialogueBox(DialogueBoxNode dialogueBoxNode)
	{
		if (dialogueBoxNode.GetToggleSelection() == 0)
			ToggleDialogueBox(true);
		else
			ToggleDialogueBox(false);

		m_sceneManager.NextNode();
	}

	/// <summary>
	/// 
	/// </summary>
	private void ClearDialogueBox()
	{
		m_speaker.text = "";
		m_dialogue.text = "";
	}

    /// <summary>
    /// 
    /// </summary>
    public void ToggleAuto()
    {
        m_isAutoEnabled = !m_isAutoEnabled;
        m_sceneManager.GetInputManager().SetIsInputAllowed(!m_isAutoEnabled);

        if (m_sceneManager.GetCurrentNode() is DialogueNode &&
            m_isAutoEnabled && m_isTyping == false)
            m_sceneManager.StartCoroutine(WaitForAuto());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAuto()
    {
        // determine wait time
        float waitTime = m_dialogue.text.Length * ((1.1f - m_autoSpeed) * 0.1f);
        yield return new WaitForSeconds(waitTime); // wait

        // if auto is still enabled at the end of wait, proceed
        if (m_isAutoEnabled)
            m_sceneManager.NextNode();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator TypewriteText(string dialogue)
	{
		m_dialogue.text = "";
		m_isTyping = true;

		for (int i = 0; i < dialogue.Length; i++)
		{
			m_dialogue.text += dialogue[i];
			yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
		}
		
		m_isTyping = false;

        if (m_isAutoEnabled)
            m_sceneManager.StartCoroutine(WaitForAuto());
	}

	/// <summary>
	/// 
	/// </summary>
	public void SkipTypewrite()
	{
		m_sceneManager.StopCoroutine(m_typewriteEvent);
			
		DialogueNode dialogueNode = m_sceneManager.GetCurrentNode() as DialogueNode;
		m_dialogue.text = dialogueNode.GetDialogue();

		m_isTyping = false;
	}
}

}
