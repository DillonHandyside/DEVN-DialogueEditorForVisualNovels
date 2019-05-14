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

	// scene manager ref
	private SceneManager m_sceneManager;

	// reference to mixer & relevant mixer groups
	[Header("Audio Mixer")]
	[SerializeField] private AudioMixer m_audioMixer;
    [Header("Mixer Groups")]
	[SerializeField] private AudioMixerGroup m_BGM;
	[SerializeField] private AudioMixerGroup m_ambience;
	[SerializeField] private AudioMixerGroup m_SFX;
    [SerializeField] private AudioMixerGroup m_voice;

	// reference to the current looping audio in the scene
	[HideInInspector]
	[SerializeField] private AudioSource m_bgmAudio;
	[HideInInspector]
	[SerializeField] private List<AudioSource> m_ambientAudio;

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

		// cache scene manager reference
		m_sceneManager = GetComponent<SceneManager>();
		Debug.Assert(m_sceneManager != null, "SceneManager cache unsuccessful!");

		// initialise empty lists to reference ambience and SFX tracks
		m_ambientAudio = new List<AudioSource>();
	}
	
	/// <summary>
	/// use this to set new background music and optional ambient tracks!
	/// </summary>
	/// <param name="bgmClip">the background music to play, only one at a time!</param>
	/// <param name="ambientClips">a list of all the ambient tracks you want to play</param>
	/// <param name="nextNode">do you want to proceed to the next node of the current scene?
	/// If you're calling this function manually, you probably want to leave this as false</param>
	public void SetBGM(AudioClip bgmClip, List<AudioClip> ambientClips = null, bool nextNode = false)
	{
		ClearBGM();

        // background music
        GameObject bgmObject = new GameObject();
		m_bgmAudio = bgmObject.AddComponent<AudioSource>(); // create

		// set clip, mixer group and loop
		m_bgmAudio.clip = bgmClip;
		m_bgmAudio.outputAudioMixerGroup = m_BGM;
		m_bgmAudio.loop = true;

		m_bgmAudio.Play(); // play
			
		// ambience
		if (ambientClips != null)
		{
			for (int i = 0; i < ambientClips.Count; i++)
			{
				GameObject ambienceObject = new GameObject();
				AudioSource ambience = ambienceObject.AddComponent<AudioSource>(); // create
				m_ambientAudio.Add(ambience);
		
				// set clip, mixer group and loop
				ambience.clip = ambientClips[i];
				ambience.outputAudioMixerGroup = m_ambience;
				ambience.loop = true;

				ambience.Play(); // play
			}
		}

		if (nextNode)
			m_sceneManager.NextNode(); // continue to next node
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="desiredVolume"></param>
	/// <param name="fadeInTime"></param>
	/// <returns></returns>
	public IEnumerator FadeInBGM(float desiredVolume = 1.0f, float fadeInTime = 0.5f)
	{
		SettingsManager settingsManager = SettingsManager.GetInstance();
		Debug.Assert(settingsManager != null, "DEVN: SettingsManager singleton not found!");

		if (desiredVolume < 0.0001f || desiredVolume > 1.0f)
			Debug.LogWarning("DEVN: Do not attempt to see volume outside of 0.0001f to 1.0f");

		desiredVolume = Mathf.Clamp(desiredVolume, 0.0001f, 1.0f);

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeInTime)
		{
			// set volume
			float percentage = elapsedTime / fadeInTime;
			settingsManager.SetBGMVolume(Mathf.Lerp(0.0001f, desiredVolume, percentage));

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="fadeOutTime"></param>
	/// <returns></returns>
	public IEnumerator FadeOutBGM(float fadeOutTime = 0.5f)
	{
		SettingsManager settingsManager = SettingsManager.GetInstance();
		Debug.Assert(settingsManager != null, "DEVN: SettingsManager singleton not found!");
		float currentVolume = settingsManager.GetBGMVolume();

		float elapsedTime = 0.0f;

		while (elapsedTime < fadeOutTime)
		{
			// set volume
			float percentage = 1 - (elapsedTime / fadeOutTime);
			settingsManager.SetBGMVolume(Mathf.Lerp(0.0001f, currentVolume, percentage));

			// increment time
			elapsedTime += Time.deltaTime;

			yield return null;
		}
	}

	/// <summary>
	/// call this function if you want to silence all of the background music and ambience!
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
	/// SFX coroutine which plays a one-shot audio clip. Call StartCoroutine on this to
	/// play sound effects!
	/// </summary>
	/// <param name="sfxClip">the sound effect to play</param>
	/// <param name="nextNode">do you want to proceed to the next node of the current scene?
	/// If you're calling this function manually, you probably want to leave this as false</param>
	/// <param name="waitForFinish">this only matters if nextNode is true. If true, the scene waits
	/// until the background has finished fading in before proceeding to the next node</param>
	public IEnumerator PlaySFX(AudioClip sfxClip, bool nextNode = false, bool waitForFinish = false)
	{
		// create new audio source
		GameObject sfxObject = new GameObject();
		AudioSource sfx = sfxObject.AddComponent<AudioSource>();

		sfx.outputAudioMixerGroup = m_SFX;
		sfx.PlayOneShot(sfxClip); // play

		// instantly proceed to next node if "wait for finish" is false
		if (!waitForFinish && nextNode)
			m_sceneManager.NextNode();

		while (sfx.isPlaying)
			yield return null; // wait until SFX has stopped playing
		
		// destroy the SFX
		Destroy(sfx.gameObject);

		// proceed to next node if "wait for finish" is true
		if (waitForFinish && nextNode)
			m_sceneManager.NextNode();
	}

	/// <summary>
	/// voice clip coroutine which plays a one-shot audio clip. Call StartCoroutine on this
	/// to play voice clips!
	/// </summary>
	/// <param name="voiceClip">the voice clip to play</param>
	public IEnumerator PlayVoiceClip(AudioClip voiceClip)
	{
		// create new audio source
		GameObject voiceObject = new GameObject();
		AudioSource voice = voiceObject.AddComponent<AudioSource>();

		voice.outputAudioMixerGroup = m_voice;
		voice.PlayOneShot(voiceClip); // play

		while (voice.isPlaying)
			yield return null; // wait until voice has stopped playing

		// destroy the voice clip
		Destroy(voice.gameObject);
	}
}

}
