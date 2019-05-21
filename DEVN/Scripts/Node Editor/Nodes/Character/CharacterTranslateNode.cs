using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// node used for translating a particular character, either instantly or lerped over time
/// </summary>
[System.Serializable]
public class CharacterTranslateNode : BaseNode
{
	// the character to move
	[SerializeField] private Character m_character;

	// the translation value
	[SerializeField] private float m_xPosition = 50;

	// lerp values
	[SerializeField] private bool m_isLerp = false;
	[SerializeField] private float m_lerpTime = 0.5f;

	#region getters

	public Character GetCharacter() { return m_character; }
	public float GetXPosition() { return m_xPosition; }
	public bool GetIsLerp() { return m_isLerp; }
	public float GetLerpTime() { return m_lerpTime; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
			
        m_title = "Character Translate";
			
        m_rectangle.width = 190;
			
        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor
	/// </summary>
	/// <param name="node">node to copy</param>
	/// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

		CharacterTranslateNode characterMoveNode = node as CharacterTranslateNode;

        // copy relevant variables, character, translation, lerp etc.
		m_character = characterMoveNode.m_character;
		m_xPosition = characterMoveNode.m_xPosition;
		m_isLerp = characterMoveNode.m_isLerp;
		m_lerpTime = characterMoveNode.m_lerpTime;
    }

	/// <summary>
	/// overridden draw function, draws character object field, vector2 translation field, lerp toggle/slider
	/// </summary>
	/// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
		// draw character label and object field
		EditorGUILayout.LabelField("Character");
		m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;

		// draw vector2 translate field
        EditorGUILayout.LabelField("X Position (%)");
        m_xPosition = EditorGUILayout.Slider(m_xPosition, 0, 100);

		// draw lerp toggle
		EditorGUILayout.LabelField("Lerp");
        Rect toggleRect = GUILayoutUtility.GetLastRect();
        toggleRect.x = m_rectangle.width - 18;
		m_isLerp = EditorGUI.Toggle(toggleRect, m_isLerp);

		// if lerp is toggled, draw lerp time slider
		if (m_isLerp)
		{
			EditorGUILayout.LabelField("Lerp Time");
			m_lerpTime = EditorGUILayout.Slider(m_lerpTime, 0, 3);
		}

        // resize node
        if (Event.current.type == EventType.Repaint)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            m_rectangle.height = lastRect.y + lastRect.height + 4;
        }

        base.DrawNodeWindow(id);
	}

#endif
}

}

}