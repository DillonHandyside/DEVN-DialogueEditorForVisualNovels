using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// helper class which is stored on the "Log Message" prefab. Used to retrieve
/// references to the log messages different UI elements, e.g. sprites, dialogue, etc.
/// </summary>
public class LogMessage : MonoBehaviour
{
    // the different UI elements in the log message prefab
    [SerializeField] private Image m_sprite;
    [SerializeField] private Text m_speaker;
    [SerializeField] private Text m_dialogue;

    #region getters

    public Image GetSprite() { return m_sprite; }
    public Text GetSpeaker() { return m_speaker; }
    public Text GetDialogue() { return m_dialogue; }

    #endregion
}

}