using UnityEngine;

namespace DEVN
{

/// <summary>
/// log management class which is responsible for adding to and clearing the text log
/// </summary>
public class LogManager : MonoBehaviour
{
    // log message prefab reference
    [Header("Log")]
    [Tooltip("Plug in the \"Log Message\" prefab found in \"DEVN\\Prefabs\"")]
	[SerializeField] private GameObject m_logPrefab;

    // log content panel
    [Tooltip("Plug in a scroll-view content panel, the log messages will be added to this")]
	[SerializeField] private Transform m_logContent;
        
	/// <summary>
	/// use this to add a piece of dialogue to the text log!
	/// </summary>
	/// <param name="sprite">the sprite of the speaking character</param>
	/// <param name="speaker">the name of the speaking character</param>
	/// <param name="dialogue">the dialogue that the character spoke</param>
	public void LogDialogue(Sprite sprite, string speaker, string dialogue)
	{
		GameObject log = Instantiate(m_logPrefab, m_logContent); // create log message
        LogMessage logMessage = log.GetComponent<LogMessage>();
            
        // only set sprite if a sprite was given
        if (sprite != null)
        {
            logMessage.GetSprite().sprite = sprite; // set sprite
            logMessage.GetSpeaker().color = Color.white; // no transparency
        }

        // set relevant text fields
        logMessage.GetSpeaker().text = speaker;
        logMessage.GetDialogue().text = dialogue;
	}

    /// <summary>
    /// use this to clear the text log!
    /// </summary>
    public void ClearLog()
    {
        // get all logged messages
        GameObject[] logMessages = m_logContent.GetComponentsInChildren<GameObject>();

        // iterate through all logged messages and delete each one
        for (int i = 0; i < logMessages.Length; i++)
            Destroy(logMessages[i]);
    }
}

}