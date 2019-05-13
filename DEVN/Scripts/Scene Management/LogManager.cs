using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

public class LogManager : MonoBehaviour
{
	// singleton
	private static LogManager m_instance;

	[SerializeField] private Transform m_logContent;
	[SerializeField] private GameObject m_logContentPrefab;

	#region getters

	public static LogManager GetInstance() { return m_instance; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Awake ()
	{
		m_instance = this;
	}
		
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sprite"></param>
	/// <param name="name"></param>
	/// <param name="dialogue"></param>
	public void LogDialogue(Sprite sprite, string name, string dialogue)
	{
		GameObject logContent = Instantiate(m_logContentPrefab, m_logContent);

		// set the sprite of the log content panel
		Image[] images = logContent.GetComponentsInChildren<Image>();
		for (int i = 0; i < images.Length; i++)
		{
			if (images[i].name == "Sprite")
			{
				if (sprite != null)
				{
					images[i].sprite = sprite;
					images[i].color = Color.white;
				}
				break;
			}
		}

		// set the relevant text fields of the log content panel
		Text[] textFields = logContent.GetComponentsInChildren<Text>();
		for (int i = 0; i < textFields.Length; i++)
		{
			if (textFields[i].name == "Name")
				textFields[i].text = name; // set speaker name

			if (textFields[i].name == "Dialogue")
				textFields[i].text = dialogue; // set speaker dialogue
		}
	}
}

}

