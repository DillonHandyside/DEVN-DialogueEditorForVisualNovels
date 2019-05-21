using UnityEngine;

namespace DEVN
{

namespace ScriptableObjects
{

/// <summary>
/// background asset to be created by the user. Contains different elements of the background such as the name 
/// of the location, the background sprite itself, and whether it is a CG or not
/// </summary>
[CreateAssetMenu(fileName = "New Background", menuName = "DEVN/Background")]
public class Background : ScriptableObject
{
    // background elements
    [SerializeField] private string m_name; 
    [SerializeField] private Sprite m_background;
    [SerializeField] private bool m_isCG;

    #region getters

    public string GetName() { return m_name; }
    public Sprite GetBackground() { return m_background; }
    public bool GetIsCG() { return m_isCG; }

    #endregion
}

}

}

