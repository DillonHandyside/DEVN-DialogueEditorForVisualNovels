using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CharacterInvertNode : BaseNode
{
	// the character to move
	[SerializeField] private Character m_character;

	#region getters

	public Character GetCharacter() { return m_character; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
			
        m_title = "Character Invert";
			
        m_rectangle.width = 190;
			
        AddOutputPoint(); // linear
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

		CharacterInvertNode characterMoveNode = node as CharacterInvertNode;

		m_character = characterMoveNode.m_character;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="id"></param>
    protected override void DrawNodeWindow(int id)
	{
		float fieldWidth = m_rectangle.width - 10;
		float fieldHeight = 16;

		Rect fieldRect = new Rect(5, 20, fieldWidth, fieldHeight);

		// draw character label and object field
		GUI.Label(fieldRect, "Character");
		fieldRect.y += fieldHeight;
		m_character = EditorGUI.ObjectField(fieldRect, m_character, typeof(Character), false) as Character;
		fieldRect.y += fieldHeight;

		// resize node
		m_rectangle.height = fieldRect.y + 4;

		base.DrawNodeWindow(id);
	}

#endif
}

}
