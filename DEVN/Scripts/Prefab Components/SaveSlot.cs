using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

namespace Components
{

/// <summary>
/// save slot component which is stored on the save slot prefab. Used to retrieve references to save slot UI 
/// image and text elements
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

}