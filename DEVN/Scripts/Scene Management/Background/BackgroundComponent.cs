using UnityEngine;
using UnityEngine.UI;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class BackgroundComponent : MonoBehaviour
{
	// references to the background elements in the scene
	[Header("Background Images")]
	public Image m_imageBackground;
	public Image m_colourBackground;

	#region getters

	public Image GetImageBackground() { return m_imageBackground; }
	public Image GetColourBackground() { return m_colourBackground; }

	#endregion
}

}