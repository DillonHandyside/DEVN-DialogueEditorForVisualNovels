using UnityEngine;
using UnityEditor;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// a node which houses sound effect variables such as the SFX itself and a "wait for finish" flag
/// </summary>
[System.Serializable]
public class SFXNode : BaseNode
{
	[SerializeField] private AudioClip m_sfxAudio; // the sound effect

    // wait for sfx to finish before proceeding to next node?
	[SerializeField] private bool m_waitForFinish = false;
        
	#region getters

	public AudioClip GetSFX() { return m_sfxAudio; }
	public bool GetWaitForFinish() { return m_waitForFinish; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
			
        m_title = "SFX";
			
        m_rectangle.width = 170;
        m_rectangle.height = 74;
			
        AddOutputPoint(); // linear
    }

	/// <summary>
	/// copy constructor which copies the SFX audio clip
	/// </summary>
	/// <param name="node">node to copy</param>
	/// <param name="position">the position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

		SFXNode sfxNode = node as SFXNode;

		m_sfxAudio = sfxNode.m_sfxAudio;
    }

	/// <summary>
	/// overridden draw function which draws the SFX object field
	/// </summary>
	/// <param name="id">the ID of the node window</param>
    protected override void DrawNodeWindow(int id)
	{
		// draw SFX audio label and object field
		EditorGUILayout.LabelField("SFX");
		m_sfxAudio = EditorGUILayout.ObjectField(m_sfxAudio, typeof(AudioClip), false) as AudioClip;

		// draw "wait for finish" label and boolean toggle
		EditorGUILayout.LabelField("Wait For Finish");
        Rect toggleRect = GUILayoutUtility.GetLastRect();
		toggleRect.x = m_rectangle.width - 18;
		m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);

		base.DrawNodeWindow(id);
	}

#endif
}

}

}