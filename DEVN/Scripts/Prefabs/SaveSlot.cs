using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// save slot component used to house references to save slot UI
/// elements
/// </summary>
public class SaveSlot : MonoBehaviour
{
    // snapshot of saved scene
    [SerializeField] private Image m_snapshot;

    // text elements for scene name, play-time and date
    [SerializeField] private Text m_scene;
    [SerializeField] private Text m_playTime;
    [SerializeField] private Text m_date;

    #region getters

    public Image GetSnapshot() { return m_snapshot; }
    public Text GetSlotScene() { return m_scene; }
    public Text GetPlayTime() { return m_playTime; }
    public Text GetDate() { return m_date; }

    #endregion
}

}