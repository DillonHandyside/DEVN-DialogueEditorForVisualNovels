using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// node used to determine the entering and exiting of a character, including position, fade time and default sprite
/// </summary>
[System.Serializable]
public class CharacterNode : BaseNode
{
    // enter/exit pop up menu
    private string[] m_toggle = { "Enter", "Exit" };
	[SerializeField] private int m_toggleSelection = 0;

	// the entering/exiting character
	[SerializeField] private Character m_character;

	// the desired sprite on entry/exit
	[SerializeField] private Sprite m_sprite;
	[SerializeField] private int m_spriteSelection = 0;
		
	[SerializeField] private float m_xPosition = 50.0f; // character alignment
	[SerializeField] private float m_fadeTime = 0.5f; // fade time
	[SerializeField] private bool m_isInverted = false; // y scale invert?
	[SerializeField] private bool m_waitForFinish = true; // wait before next node?

	#region getters

	public int GetToggleSelection() { return m_toggleSelection; }
	public Character GetCharacter() { return m_character; }
	public Sprite GetSprite() { return m_sprite; }
	public float GetXPosition() { return m_xPosition; }
	public float GetFadeTime() { return m_fadeTime; }
	public bool GetIsInverted() { return m_isInverted; }
	public bool GetWaitForFinish() { return m_waitForFinish; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "Character";

		m_rectangle.width = 340;

        AddOutputPoint(); // linear
    }

	/// <summary>
	/// overridden copy constructor, copies relevant character variables such as the default sprite, position 
    /// and fade time
	/// </summary>
	/// <param name="node"></param>
	/// <param name="position"></param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		CharacterNode characterNode = node as CharacterNode;

		// copy enter/exit toggle
		m_toggleSelection = characterNode.m_toggleSelection;

		// copy character object and default sprite
		m_character = characterNode.m_character;
		m_spriteSelection = characterNode.m_spriteSelection;

		// copy position, fade time and invert status
		m_xPosition = characterNode.m_xPosition;
		m_fadeTime = characterNode.m_fadeTime;
		m_isInverted = characterNode.m_isInverted;
	}

	/// <summary>
	/// overridden draw function, draws character object field, sprite object field, dropdowns and sliders etc.
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
    {
        // draw the sprite in the background, if one is selected
        DrawSpriteBackground();

		// enter/exit popup
		m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);
                
        DrawCharacterObjectField(); // character object field

		if (m_character != null)
		{
            // draw all of the relevant GUI elements
            if (DrawSpritePopup())
            {
                DrawAlignmentSlider(); 
                DrawFadeTimeSlider(); 
                DrawInvertToggle(); 
                DrawWaitForFinishToggle(); 
            }

            // end vertical layout group started in DrawSpriteBackground, if necessary
            if (m_sprite != null)
                EditorGUILayout.EndVertical();
		}

        // resize node
        if (Event.current.type == EventType.Repaint)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            m_rectangle.height = lastRect.y + lastRect.height + 4;
        }

		base.DrawNodeWindow(id);
    }

    /// <summary>
    /// function which draws a semi-transparent sprite in the background of the node
    /// </summary>
    private void DrawSpriteBackground()
    {
		if (m_sprite == null)
		    return; // no sprite selected, don't draw
		
		// determine sprite width and height
		float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
		float spriteWidth = m_rectangle.width;
		float spriteHeight = spriteWidth / aspectRatio;

        // draw sprite
        GUI.color = new Color(1, 1, 1, 0.45f);
		GUI.DrawTexture(new Rect(72, 16, spriteWidth, spriteHeight), m_sprite.texture);
        GUI.color = Color.white;
                
        // begin vertical layout such that the rest of the GUI elements are half the width of the node
        EditorGUILayout.BeginVertical(GUILayout.Width(m_rectangle.width * 0.5f));
    }

	/// <summary>
	/// function which draws an object field for selecting a character asset
	/// </summary>
	private void DrawCharacterObjectField()
	{
		// draw label & object field
		EditorGUILayout.LabelField("Character");
		m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;

        if (m_character == null)
            m_sprite = null; // reset sprite if no character is selected
	}
     
    /// <summary>
    /// function which draws a dropdown list for all available sprites
    /// </summary>
	private bool DrawSpritePopup()
	{
		// get sprites
		List<Sprite> sprites = m_character.GetSprites();

        // character contains no sprites, therefore no need to enter/exit the character
        if (sprites.Count == 0)
        {
            Debug.LogError("DEVN: Character has no sprites! If you want an off-screen character, there is no need to enter/exit.");
            m_sprite = null;
            return false;
        }

        // get a list of all the character's sprite names
		string[] spriteNames = new string[sprites.Count];
		for (int i = 0; i < sprites.Count; i++)
		{
			Sprite sprite = sprites[i];

            // log an error and stop function if null sprites are found
			if (sprite == null)
			{
				Debug.LogError("DEVN: Character contains null sprites!");
                return false;
			}
			else
				spriteNames[i] = sprite.name;
		}
        
		// draw label and drop-down sprite selection menu
		EditorGUILayout.LabelField("Sprite");
		m_spriteSelection = EditorGUILayout.Popup(m_spriteSelection, spriteNames);
        m_sprite = sprites[m_spriteSelection];

		// clamp to prevent index out of range errors
		m_spriteSelection = Mathf.Clamp(m_spriteSelection, 0, sprites.Count - 1);
        return true;
	}

    /// <summary>
    /// function which draws a slider for setting a character's alignment (x-position) on entry
    /// </summary>
    private void DrawAlignmentSlider()
    {
        // only draw alignment slider on character "Enter"
        if (m_toggleSelection == 0)
        {
            EditorGUILayout.LabelField("X Position (%)");
            m_xPosition = EditorGUILayout.Slider(m_xPosition, 0.0f, 100.0f);
        }
    }

    /// <summary>
    /// function which draws a slider for setting a character's fade in/out time on entry/exit
    /// </summary>
    private void DrawFadeTimeSlider()
    {
        EditorGUILayout.LabelField("Fade Time");
        m_fadeTime = EditorGUILayout.Slider(m_fadeTime, 0.0f, 3.0f);
    }

    /// <summary>
    /// function which draws a toggle for determining whether or not a character is inverted on entry
    /// </summary>
    private void DrawInvertToggle()
    {
        // only draw toggle on character "Enter"
        if (m_toggleSelection == 0)
        {
            // draw label and toggle field on same line
            EditorGUILayout.LabelField("Invert");
            Rect toggleRect = GUILayoutUtility.GetLastRect();
            toggleRect.x = toggleRect.width - 8;
            m_isInverted = EditorGUI.Toggle(toggleRect, m_isInverted);
        }
    }

    /// <summary>
    /// function which draws a toggle for setting "wait for finish"
    /// </summary>
    private void DrawWaitForFinishToggle()
    {
        // draw label and toggle field on same line
        EditorGUILayout.LabelField("Wait For Finish");
        Rect toggleRect = GUILayoutUtility.GetLastRect();
        toggleRect.x = toggleRect.width - 8;
        m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);
    }

#endif
}

}

}
