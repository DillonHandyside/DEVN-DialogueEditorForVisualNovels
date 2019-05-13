using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// singleton manager class responsible for managing all of the dialogue
/// nodes with-in a DEVN scene
/// </summary>
public class DialogueManager : MonoBehaviour
{
	// singleton
	private static DialogueManager m_instance;

	// scene manager ref
	private SceneManager m_sceneManager;
	private DialogueNode m_currentNode;

	// reference to dialogue box UI elements
    [Header("Dialogue Box Object")]
	[SerializeField] private GameObject m_dialogueBox;

    [Header("Text Fields")]
	[SerializeField] private Text m_speaker;
	[SerializeField] private Text m_dialogue;
		
	// reference to typewrite coroutine, so StopCoroutine can be used
	private IEnumerator m_typewriteEvent;

	// other text typing relevant variables
	private float m_textSpeed;
	private bool m_isTyping = false;

    private float m_autoSpeed;
    private bool m_isAutoEnabled = false;

	#region getters
		
	public static DialogueManager GetInstance() { return m_instance; }
	public bool GetIsTyping() { return m_isTyping; }

	#endregion

	#region setters

	public void SetDialogueSpeed(float speed) { m_textSpeed = speed; }
    public void SetAutoSpeed(float speed) { m_autoSpeed = speed; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Awake ()
	{
		m_instance = this; // intialise singleton

		// cache reference to scene manager
		m_sceneManager = GetComponent<SceneManager>();
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetDialogue()
	{
		// allow input during dialogue, to allow skip & continue
		m_sceneManager.SetIsInputAllowed(true);
			
		m_currentNode = m_sceneManager.GetCurrentNode() as DialogueNode;
			
		// attempt to get this dialogue node's character, log an error if there is none
		Character character = m_currentNode.GetCharacter();
		Debug.Assert(character != null, "DEVN: Dialogue requires a speaking character!");

		// update speaker text field
		m_speaker.text = character.m_name; 
		
		CharacterManager characterManager = CharacterManager.GetInstance();
		Debug.Assert(characterManager != null, "DEVN: A CharacterManager must exist!");
		
		Sprite currentSprite = m_currentNode.GetSprite();
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

		LogManager.GetInstance().LogDialogue(currentSprite, character.m_name, m_currentNode.GetDialogue());
		

		StartCoroutine(m_typewriteEvent = TypewriteText());
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
        m_sceneManager.SetIsInputAllowed(!m_isAutoEnabled);

        if (m_sceneManager.GetCurrentNode() is DialogueNode &&
            m_isAutoEnabled && m_isTyping == false)
            StartCoroutine(WaitForAuto());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForAuto()
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
    IEnumerator TypewriteText()
	{
		m_dialogue.text = "";
		m_isTyping = true;

		for (int i = 0; i < m_currentNode.GetDialogue().Length; i++)
		{
			m_dialogue.text += m_currentNode.GetDialogue()[i];
			yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
		}
		
		m_isTyping = false;

        if (m_isAutoEnabled)
            StartCoroutine(WaitForAuto());
	}

	/// <summary>
	/// 
	/// </summary>
	public void SkipTypewrite()
	{
		StopCoroutine(m_typewriteEvent);
		m_dialogue.text = m_currentNode.GetDialogue();
		m_isTyping = false;
	}
}

}
