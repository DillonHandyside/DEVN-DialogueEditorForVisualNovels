using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DEVN
{

/// <summary>
/// singleton manager class responsible for managing all of the audio
/// nodes with-in a DEVN scene
/// </summary>
public class AudioManager : MonoBehaviour
{
	// singleton
	private static AudioManager m_instance;

	private SceneManager m_sceneManager;

	// reference to mixer & relevant mixer groups
	[SerializeField] private AudioMixer m_audioMixer;
	[SerializeField] private AudioMixerGroup m_BGM;
	[SerializeField] private AudioMixerGroup m_ambience;
	[SerializeField] private AudioMixerGroup m_SFX;

	[SerializeField] private AudioSource m_emptyAudioSource;

	// reference to the current audio in the scene
	[HideInInspector]
	[SerializeField] private AudioSource m_bgmAudio;
	[HideInInspector]
	[SerializeField] private List<AudioSource> m_ambientAudio;
	[HideInInspector]
	[SerializeField] private List<AudioSource> m_sfxAudio;

	#region getters

	public static AudioManager GetInstance() { return m_instance; }
	public AudioMixer GetAudioMixer() { return m_audioMixer; }
	public AudioMixerGroup GetBGM() { return m_BGM; }
	public AudioMixerGroup GetAmbience() { return m_ambience; }
	public AudioMixerGroup GetSFX() { return m_SFX; }

	#endregion	

	/// <summary>
	/// awake function which initialises audio relevant variables
	/// </summary>
	void Awake ()
	{
		m_instance = this; // initialise singleton
		
		// cache scene manager
		m_sceneManager = GetComponent<SceneManager>();

		// initialise empty lists to reference ambience and SFX tracks
		m_ambientAudio = new List<AudioSource>();
		m_sfxAudio = new List<AudioSource>();
	}
	
	/// <summary>
	/// BGM set function which clears current background audio elements
	/// and sets new background audio elements. (BGM and ambience)
	/// </summary>
	public void SetBGM()
	{
		BGMNode bgmNode = m_sceneManager.GetCurrentNode() as BGMNode;
		ClearBGM();

		// background music
		m_bgmAudio = Instantiate(m_emptyAudioSource); // create

		// set clip, mixer group and loop
		m_bgmAudio.clip = bgmNode.GetBGM();
		m_bgmAudio.outputAudioMixerGroup = m_BGM;
		m_bgmAudio.loop = true;

		m_bgmAudio.Play(); // play
			
		// ambience
		for (int i = 0; i < bgmNode.GetAmbientAudio().Count; i++)
		{
			AudioSource ambience = Instantiate(m_emptyAudioSource); // create
			m_ambientAudio.Add(ambience);
			
			// set clip, mixer group and loop
			ambience.clip = bgmNode.GetAmbientAudio()[i];
			ambience.outputAudioMixerGroup = m_ambience;
			ambience.loop = true;

			ambience.Play(); // play
		}

		m_sceneManager.NextNode(); // continue to next node
	}

	/// <summary>
	/// helper function which deletes all background audio elements in
	/// the scene
	/// </summary>
	public void ClearBGM()
	{ 
		Destroy(m_bgmAudio); // delete BGM

		int ambienceCount = m_ambientAudio.Count;
		for (int i = 0; i < ambienceCount; i++)
		{
			AudioSource ambience = m_ambientAudio[i]; // get reference
			m_ambientAudio.Remove(ambience); // remove from list
			Destroy(ambience.gameObject); // delete ambient track
		}
	}

	/// <summary>
	/// SFX coroutine which plays a one-shot audio clip, and either continues
	/// to the next node instantly or waits until audio clip is finished
	/// </summary>
	public IEnumerator PlaySFX()
	{
		SFXNode sfxNode = m_sceneManager.GetCurrentNode() as SFXNode;

		AudioSource sfx = Instantiate(m_emptyAudioSource); // create
		m_sfxAudio.Add(sfx);

		sfx.outputAudioMixerGroup = m_SFX;
		sfx.PlayOneShot(sfxNode.GetSFX()); // play

		// instantly proceed to next node if "wait for finish" is false
		if (!sfxNode.GetWaitForFinish())
			m_sceneManager.NextNode();

		while (sfx.isPlaying)
			yield return null; // wait until SFX has stopped playing
		
		// destroy the SFX
		m_sfxAudio.Remove(sfx);
		Destroy(sfx.gameObject);

		// proceed to next node if "wait for finish" is true
		if (sfxNode.GetWaitForFinish())
			m_sceneManager.NextNode();
	}
}

}
