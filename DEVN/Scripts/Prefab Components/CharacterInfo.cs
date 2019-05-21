using UnityEngine;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Components
{

/// <summary>
/// character info component which is stored on the character prefab. Used to set and get information of the
/// current character
/// </summary>
public class CharacterInfo : MonoBehaviour
{
	[HideInInspector]
	[SerializeField] private Character m_character;

	#region getters

	public Character GetCharacter() { return m_character; }

		#endregion

	#region setters

	public void SetCharacter(Character character) { m_character = character; }

	#endregion
}

}

}
