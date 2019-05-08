using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class CharacterTranslateNode : BaseNode
{
	// the character to move
	[SerializeField] private Character m_character;

	// the translation value
	[SerializeField] private Vector2 m_translation;

	// lerp values
	[SerializeField] private bool m_isLerp = false;
	[SerializeField] private float m_lerpTime = 0.5f;

	#region getters

	public Character GetCharacter() { return m_character; }
	public Vector2 GetTranslation() { return m_translation; }
	public bool GetIsLerp() { return m_isLerp; }
	public float GetLerpTime() { return m_lerpTime; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
			
        m_title = "Character Translate";
			
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

		CharacterTranslateNode characterMoveNode = node as CharacterTranslateNode;

		m_character = characterMoveNode.m_character;
		m_translation = characterMoveNode.m_translation;
		m_isLerp = characterMoveNode.m_isLerp;
		m_lerpTime = characterMoveNode.m_lerpTime;
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

		//
		m_translation = EditorGUI.Vector2Field(fieldRect, "Translation", m_translation);
		fieldRect.y += fieldHeight * 2;

		// draw lerp toggle
		GUI.Label(fieldRect, "Lerp");
		fieldRect.x = m_rectangle.width - 20;
		fieldRect.width = fieldHeight;
		m_isLerp = EditorGUI.Toggle(fieldRect, m_isLerp);
		fieldRect.x = 5;
		fieldRect.width = fieldWidth;
		fieldRect.y += fieldHeight;

		// if lerp is toggled, draw lerp time slider
		if (m_isLerp)
		{
			GUI.Label(fieldRect, "Lerp Time");
			fieldRect.y += fieldHeight;
			m_lerpTime = EditorGUI.Slider(fieldRect, m_lerpTime, 0, 3);
			fieldRect.y += fieldHeight;
		}

		// resize node
		m_rectangle.height = fieldRect.y + 4;

		base.DrawNodeWindow(id);
	}

#endif
}

}
