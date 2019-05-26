using UnityEngine;
using UnityEngine.UI;
using DEVN.SceneManagement;

namespace DEVN
{
	
namespace Components
{

/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to the dialogue box and it's child text elements
/// </summary>
public class DialogueComponent : MonoBehaviour
{
	// references to dialogue box UI elements
	[Header("Dialogue UI Element/s")]
	[Tooltip("Plug in a parent panel, this is used to enable/disable the dialogue box when required")]
	[SerializeField] private GameObject m_dialogueBox;
	[Tooltip("Plug in the text field that displays the speaking character's name")]
	[SerializeField] private Text m_speaker;
	[Tooltip("Plug in the text field that displays the dialogue")]
	[SerializeField] private Text m_dialogue;

    private DialogueManager m_dialogueManager;
	
	#region getters

	public GameObject GetDialogueBox() { return m_dialogueBox; }
	public Text GetSpeaker() { return m_speaker; }
	public Text GetDialogue() { return m_dialogue; }

    public DialogueManager GetDialogueManager()
    {
        if (m_dialogueManager == null)
            m_dialogueManager = new DialogueManager(this);

        return m_dialogueManager;
    }

	#endregion
      
    /// <summary>
    /// proceed dialogue function which calls DialogueManager's ProceedDialogue function 
    /// </summary>
    public void ProceedDialogue()
    {
        m_dialogueManager.ProceedDialogue();
    }

    /// <summary>
    /// toggle auto function which calls DialogueManager's ToggleAuto function 
    /// </summary>
    public void ToggleAuto()
    {
        m_dialogueManager.ToggleAuto();
    }
}

}

}