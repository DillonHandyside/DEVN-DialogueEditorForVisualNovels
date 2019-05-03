using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

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

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}

}
