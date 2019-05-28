using UnityEngine;
using UnityEngine.UI;
using DEVN.Components;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// log management class which is responsible for adding to and clearing a text log
/// </summary>
public class LogManager
{
    // log message prefab reference
	private readonly GameObject m_logPrefab;

    // log content panel
	private readonly Transform m_logContent;
        
	/// <summary>
	/// are you sure you want to construct your own LogManager? You may want to use 
	/// SceneManager.GetInstance().GetLogManager() instead
	/// </summary>
	/// <param name="logComponent">a log component which houses the relevent prefab/UI elements</param>
	public LogManager(LogComponent logComponent)
	{
		m_logPrefab = logComponent.GetLogPrefab();
		m_logContent = logComponent.GetLogContent();
	}

	/// <summary>
	/// use this to add a piece of dialogue to the text log!
	/// </summary>
	/// <param name="sprite">the sprite of the speaking character</param>
	/// <param name="speaker">the name of the speaking character</param>
	/// <param name="dialogue">the dialogue that the character spoke</param>
	public void LogDialogue(Sprite sprite, string speaker, string dialogue)
	{
		GameObject log = Object.Instantiate(m_logPrefab, m_logContent); // create log message
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

        // resize the logged message depending on the amount of new-lines in the dialogue
        int noOfLines = dialogue.Split('\n').Length;
        log.GetComponent<LayoutElement>().preferredHeight = 18 + noOfLines * 18;
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
            Object.Destroy(logMessages[i]);
    }
}

}

}