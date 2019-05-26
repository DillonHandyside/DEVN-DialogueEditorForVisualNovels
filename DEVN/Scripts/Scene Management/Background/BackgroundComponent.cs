using UnityEngine;
using UnityEngine.UI;
using DEVN.SceneManagement;

namespace DEVN
{

namespace Components
{

/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to the background UI elements
/// </summary>
public class BackgroundComponent : MonoBehaviour
{
	// references to the background elements in the scene
	[Header("Background Images")]
	[Tooltip("Plug in the image that will display the background sprite here")]
	[SerializeField] private Image m_imageBackground;
	[Tooltip("Plug in the image that will act as the fade colour here")]
	[SerializeField] private Image m_colourBackground;

    private BackgroundManager m_backgroundManager;

	#region getters

	public Image GetImageBackground() { return m_imageBackground; }
	public Image GetColourBackground() { return m_colourBackground; }
            
    public BackgroundManager GetBackgroundManager()
    {
        if (m_backgroundManager == null)
            m_backgroundManager = new BackgroundManager(this);

        return m_backgroundManager;
    }

	#endregion
}

}

}