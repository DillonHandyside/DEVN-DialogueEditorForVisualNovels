using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// node used for scaling a particular character, either instantly or lerped over time
/// </summary>
[System.Serializable]
public class CharacterScaleNode : BaseNode
{
	// the character to move
	[SerializeField] private Character m_character;

	// the scale value
	[SerializeField] private Vector2 m_scale = new Vector2(1, 1);

	// lerp values
	[SerializeField] private bool m_isLerp = false;
	[SerializeField] private float m_lerpTime = 0.5f;

	#region getters

	public Character GetCharacter() { return m_character; }
	public Vector2 GetScale() { return m_scale; }
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
			
        m_title = "Character Scale";
			
        m_rectangle.width = 190;
			
        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor
	/// </summary>
	/// <param name="node">node to cpy</param>
	/// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

		CharacterScaleNode characterScaleNode = node as CharacterScaleNode;

        // copy all relevant variables, character, scale, lerp etc.
		m_character = characterScaleNode.m_character;
		m_scale = characterScaleNode.m_scale;
		m_isLerp = characterScaleNode.m_isLerp;
		m_lerpTime = characterScaleNode.m_lerpTime;
    }

	/// <summary>
	/// overridden draw function, draws character object field, vector2 field, and lerp toggle/slider. 
	/// </summary>
	/// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
		// draw character label and object field
		EditorGUILayout.LabelField("Character");
		m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;

		// draw vector2 scale field
		m_scale = EditorGUILayout.Vector2Field("Scale", m_scale);

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