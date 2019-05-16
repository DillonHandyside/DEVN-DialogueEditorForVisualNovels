using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class DialogueComponent : MonoBehaviour
{
	// references to dialogue box UI elements
	[Header("Dialogue")]
	[Tooltip("Plug in a parent panel, this is used to enable/disable the dialogue box when required")]
	[SerializeField] private GameObject m_dialogueBox;
	[Tooltip("Plug in the text field that displays the speaking character's name")]
	[SerializeField] private Text m_speaker;
	[Tooltip("Plug in the text field that displays the dialogue")]
	[SerializeField] private Text m_dialogue;

	
	#region getters

	public GameObject GetDialogueBox() { return m_dialogueBox; }
	public Text GetSpeaker() { return m_speaker; }
	public Text GetDialogue() { return m_dialogue; }

	#endregion

	#region setters

	public void ToggleAuto() { GetComponent<SceneManager>().GetDialogueManager().ToggleAuto(); }

	#endregion
}

}