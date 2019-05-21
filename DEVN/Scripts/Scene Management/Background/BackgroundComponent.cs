using UnityEngine;
using UnityEngine.UI;

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

	#region getters

	public Image GetImageBackground() { return m_imageBackground; }
	public Image GetColourBackground() { return m_colourBackground; }

	#endregion
}

}

}