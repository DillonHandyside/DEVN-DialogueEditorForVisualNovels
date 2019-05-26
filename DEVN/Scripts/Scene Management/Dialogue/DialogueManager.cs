using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DEVN.Nodes;
using DEVN.Components;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// singleton manager class responsible for managing all of the dialogue
/// nodes with-in a DEVN scene
/// </summary>
public class DialogueManager
{
    private DialogueComponent m_dialogueComponent;
        
	// references to dialogue box UI elements
	private GameObject m_dialogueBox;
	private Text m_speaker;
	private Text m_dialogue;
		
	// reference to typewrite coroutine, so StopCoroutine can be used
	private IEnumerator m_typewriteEvent;

	// other text typing relevant variables
    private string m_currentDialogue;
	private float m_textSpeed = 1.0f;
    private float m_autoSpeed;
	private bool m_isTyping = false;
    private bool m_isProceedAllowed = false;
    private bool m_isAutoEnabled = false;

    #region setters

    public void SetDialogueSpeed(float speed) { m_textSpeed = speed; }
    public void SetAutoSpeed(float speed) { m_autoSpeed = speed; }

	#endregion

	/// <summary>
	/// are you sure you want to construct your own DialogueManager? You may want to use 
	/// SceneManager.GetInstance().GetDialogueManager() instead
	/// </summary>
	/// <param name="dialogueComponent">a dialogue component which houses the relevent UI elements</param>
	public DialogueManager(DialogueComponent dialogueComponent)
	{
        m_dialogueComponent = dialogueComponent;

		// assign references to all the relevant dialogue elements
		m_dialogueBox = dialogueComponent.GetDialogueBox();
		m_speaker = dialogueComponent.GetSpeaker();
		m_dialogue = dialogueComponent.GetDialogue();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="character"></param>
    /// <param name="sprite"></param>
    /// <param name="dialogue"></param>
    /// <param name="characterManager"></param>
    /// <param name="logManager"></param>
	public void SetDialogue(Character character, Sprite sprite, string dialogue,
        CharacterManager characterManager = null, LogManager logManager = null)
	{			
		// update speaker text field
        m_currentDialogue = dialogue;
		m_speaker.text = character.GetName();
		m_dialogueComponent.StartCoroutine(m_typewriteEvent = TypewriteText());       

		// only update character sprite if a character manager exists
		if (characterManager != null)
		{
			// find the character in the scene
			GameObject characterObject = characterManager.TryGetCharacter(character);
			
			if (sprite != null)
			{
				if (characterObject != null)
					characterManager.SetSprite(characterObject, sprite);
				else
					Debug.LogWarning("DEVN: Do not attempt to change the sprite of a character that is not in the scene.");
			}
			else if (characterObject != null)
				sprite = characterObject.GetComponent<Image>().sprite;
			else
				sprite = null;
		
			if (characterObject)
				characterManager.HighlightSpeakingCharacter(character);
		}

		// only log dialogue if a log manager exists
		if (logManager != null)
			logManager.LogDialogue(sprite, character.GetName(), dialogue);
	}
    
    /// <summary>
    /// coroutine which incrementally types the dialogue out like a typewriter
    /// </summary>
    private IEnumerator TypewriteText()
	{
		m_dialogue.text = "";
		m_isTyping = true; // is currently typing

		for (int i = 0; i < m_currentDialogue.Length; i++)
		{
			m_dialogue.text += m_currentDialogue[i]; // type one letter at a time
            yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
		}
		
		m_isProceedAllowed = true;
		m_isTyping = false; // no longer typing

        // if auto is enabled, wait for an arbitrary amount of time then proceed
        if (m_isAutoEnabled)
            m_dialogueComponent.StartCoroutine(WaitForAuto());
	}

	/// <summary>
	/// function which forces the typewriter coroutine to stop, and sets the dialogue manually
	/// </summary>
	public void SkipTypewrite()
	{
        // force typewrite to stop and manuallyset the dialogue
		m_dialogueComponent.StopCoroutine(m_typewriteEvent);
		m_dialogue.text = m_currentDialogue;

        // no longer typing, player can proceed
        m_isProceedAllowed = true;
		m_isTyping = false;
	}

	/// <summary>
	/// function which toggles the dialogue box on/off depending on it's current state
	/// </summary>
	public void ToggleDialogueBox()
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(!m_dialogueBox.activeSelf);
	}

	/// <summary>
	/// function which toggles the dialogue box on/off depending on input
	/// </summary>
	/// <param name="toggle">true: dialogue box on, false: dialogue box off</param>
	public void ToggleDialogueBox(bool toggle)
	{
		ClearDialogueBox();
		m_dialogueBox.SetActive(toggle);
	}

	/// <summary>
	/// helper function which clears the contents of the dialogue box
	/// </summary>
	private void ClearDialogueBox()
	{
		m_speaker.text = "";
		m_dialogue.text = "";
	}

    #region SceneManager-reliant functions

    /// <summary>
    /// function which proceeds to the next node upon user input
    /// </summary>
    public void ProceedDialogue()
    {
        if (m_isAutoEnabled)
            return; // disallow manual proceed if auto is enabled

        if (m_isTyping)
            SkipTypewrite(); // currently typing, so skip typewrite
        else if (m_isProceedAllowed)
        {
            m_isProceedAllowed = false;

            SceneManager sceneManager = SceneManager.GetInstance();
            if (sceneManager == null)
                Debug.LogError("DEVN: Need a SceneManager to proceed dialogue!");
            else
                sceneManager.NextNode(); // proceed
        }
    }

    /// <summary>
    /// function which toggles auto on/off depending on it's current state
    /// </summary>
    public void ToggleAuto()
    {
        m_isAutoEnabled = !m_isAutoEnabled; // toggle

        SceneManager sceneManager = SceneManager.GetInstance();
        if (sceneManager == null)
            Debug.LogError("DEVN: Need a SceneManager to use Auto functionality!");
        else if (sceneManager.GetCurrentNode() is DialogueNode && m_isAutoEnabled && m_isTyping == false)
            m_dialogueComponent.StartCoroutine(WaitForAuto()); // perform auto even if current dialogue is complete
    }

    /// <summary>
    /// coroutine which waits for an arbitrary amount of time before proceeding to the next node
    /// </summary>
    private IEnumerator WaitForAuto()
    {
        // determine wait time
        float waitTime = m_dialogue.text.Length * ((1.1f - m_autoSpeed) * 0.1f);
        yield return new WaitForSeconds(waitTime); // wait

        // if auto is still enabled at the end of wait, proceed
        if (m_isAutoEnabled)
        {
            SceneManager sceneManager = SceneManager.GetInstance();
            if (sceneManager != null)
                sceneManager.NextNode();
        }
    }

    #endregion
}

}

}