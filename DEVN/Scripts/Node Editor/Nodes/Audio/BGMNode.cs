using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// node which determines which background music and ambient audio to play
/// </summary>
[System.Serializable]
public class BGMNode : BaseNode
{
    [SerializeField] private AudioClip m_bgmAudio; // background music/audio
    [SerializeField] private List<AudioClip> m_ambientAudio; // background ambience

	#region getters

	public AudioClip GetBGM() { return m_bgmAudio; }
	public List<AudioClip> GetAmbientAudio() { return m_ambientAudio; }

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// overridden constructor
	/// </summary>
	/// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        m_title = "BGM";

        m_rectangle.width = 192;
        m_rectangle.height = 96;

        AddOutputPoint(); // linear

		// initialise ambient audio track list
        m_ambientAudio = new List<AudioClip>();
        AddAmbientAudio(); // add one ambient field by default
    }

	/// <summary>
	/// overridden copy constructor, copies BGM and ambient audio
	/// </summary>
	/// <param name="node">node to copy</param>
	/// <param name="position">position to copy to</param>
	public override void Copy(BaseNode node, Vector2 position)
	{
		base.Copy(node, position);

		BGMNode bgmNode = node as BGMNode;

		// copy background music
		m_bgmAudio = bgmNode.m_bgmAudio;

		// copy ambient audio tracks
		m_ambientAudio = new List<AudioClip>();
		for (int i = 0; i < bgmNode.m_ambientAudio.Count; i++)
			m_ambientAudio.Add(bgmNode.m_ambientAudio[i]);
	}

	/// <summary>
	/// overridden draw function
	/// </summary>
	/// <param name="id">the ID of the node window</param>
	protected override void DrawNodeWindow(int id)
    {
		// draw BGM audio label & object field
		EditorGUILayout.LabelField("Background Music");
        m_bgmAudio = EditorGUILayout.ObjectField(m_bgmAudio, typeof(AudioClip), false) as AudioClip;
            
		// draw as many ambient audio object fields as required
        EditorGUILayout.LabelField("Ambient Tracks");
        for (int i = 0; i < m_ambientAudio.Count; i++)
            m_ambientAudio[i] = EditorGUILayout.ObjectField(m_ambientAudio[i], typeof(AudioClip), false) as AudioClip;
        
        // determine button position
        Rect buttonRect = new Rect(m_rectangle.width - 48, 0, 21, 16);
        buttonRect.y = GUILayoutUtility.GetLastRect().y + buttonRect.height + 4;

		// draw the add button
		if (GUI.Button(buttonRect, "+"))
			AddAmbientAudio();

		buttonRect.x += 22;

		// draw the remove button
		if (GUI.Button(buttonRect, "-"))
			RemoveAmbientAudio();

		base.DrawNodeWindow(id);
    }

    /// <summary>
    /// helper function which adds an empty ambient track and adjusts 
	/// node height
    /// </summary>
    private void AddAmbientAudio()
    {
        // ad ambient audio track
        m_ambientAudio.Add(null);

        // resize the node
        m_rectangle.height += 18;
    }

    /// <summary>
    /// helper function which removes the last ambient track and 
	/// re-adjusts node height
    /// </summary>
    private void RemoveAmbientAudio()
    {
		// leave at least one field for ambient audio
        if (m_ambientAudio.Count > 1)
        {
            // remove the last ambient audio track
            m_ambientAudio.RemoveAt(m_ambientAudio.Count - 1);

            // resive the node
            m_rectangle.height -= 18;
        }
    }

#endif
}

}
