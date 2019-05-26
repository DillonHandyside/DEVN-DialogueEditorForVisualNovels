using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DEVN.Components;

namespace DEVN
{

namespace SceneManagement
{

/// <summary>
/// audio manager class responsible for playing, fading, pausing and stopping background
/// music and ambient tracks. 
/// </summary>
public class AudioManager
{
	// reference to mixer & relevant mixer groups
	private AudioMixer m_audioMixer;
	private AudioMixerGroup m_BGM;
	private AudioMixerGroup m_ambience;
	private AudioMixerGroup m_SFX;
    private AudioMixerGroup m_voice;

	// reference to the current audio objects in the scene
	private GameObject m_bgmAudio;
	private GameObject m_ambientAudio;
    private GameObject m_sfxAudio;
    private GameObject m_voiceAudio;

	#region getters

	public AudioMixer GetAudioMixer() { return m_audioMixer; }

	#endregion

	/// <summary>
	/// constructor
	/// </summary>
	/// <param name="audioComponent">an audio component which houses the relevent audio elements</param>
	public AudioManager(AudioComponent audioComponent)
	{
		// assign references to all the relevant audio elements
		m_audioMixer = audioComponent.GetAudioMixer();
		m_BGM = audioComponent.GetBGM();
		m_ambience = audioComponent.GetAmbience();
		m_SFX = audioComponent.GetSFX();
		m_voice = audioComponent.GetVoice();

        // create a new object to house each different type of audio source
		m_bgmAudio = new GameObject("BGM");
        m_ambientAudio = new GameObject("Ambience");
        m_sfxAudio = new GameObject("SFX");
        m_voiceAudio = new GameObject("Voice");

        // BGM, SFX and voice object all only need one AudioSource
        AudioSource bgmSource = m_bgmAudio.AddComponent<AudioSource>();
		AudioSource sfxSource = m_sfxAudio.AddComponent<AudioSource>();
        AudioSource voiceSource = m_voiceAudio.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = m_BGM;
        sfxSource.outputAudioMixerGroup = m_SFX;
        voiceSource.outputAudioMixerGroup = m_voice;
        bgmSource.loop = true; // loop the BGM
    }

	/// <summary>
	/// use this to set new background music and optional ambient tracks!
	/// </summary>
	/// <param name="bgmClip">the background music to play, only one at a time!</param>
	/// <param name="ambientClips">a list of all the ambient tracks you want to play</param>
	public void SetBGM(AudioClip bgmClip, List<AudioClip> ambientClips = null)
	{
        if (m_ambientAudio.GetComponents<AudioSource>().Length > 0)
		    ClearBGM();
        
        // set BGM clip and play
        AudioSource bgmAudio = m_bgmAudio.GetComponent<AudioSource>();
        bgmAudio.clip = bgmClip;
        bgmAudio.Play();
			
		// ambience
		if (ambientClips != null)
		{
			for (int i = 0; i < ambientClips.Count; i++)
			{
                AudioSource ambienceSource = m_ambientAudio.AddComponent<AudioSource>(); // create
				ambienceSource.outputAudioMixerGroup = m_ambience;
                ambienceSource.loop = true;

                // set clip, mixer group and loop
                ambienceSource.clip = ambientClips[i];
                ambienceSource.outputAudioMixerGroup = m_ambience;
                ambienceSource.loop = true;

                ambienceSource.Play(); // play
			}
		}
	}

    /// <summary>
    /// 
    /// </summary>
    public void ResumeBGM()
    {
        AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
        AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

        bgmSource.UnPause();

        for (int i = 0; i < ambientSources.Length; i++)
            ambientSources[i].UnPause();
    }

    /// <summary>
    /// 
    /// </summary>
    public void PauseBGM()
    {
        AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
        AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

        bgmSource.Pause();

        for (int i = 0; i < ambientSources.Length; i++)
            ambientSources[i].Pause();
    }

    /// <summary>
    /// function which silences all of the background music/ambience
    /// </summary>
    private void ClearBGM()
    {
        AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
        AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

        bgmSource.Stop();
        bgmSource.clip = null; // clear BGM

        for (int i = 0; i < ambientSources.Length; i++)
            Object.Destroy(ambientSources[i]); // remove ambient source
    }

    #region fade in/out functions

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="desiredVolume"></param>
    ///// <param name="fadeInTime"></param>
    ///// <returns></returns>
    //public IEnumerator FadeInBGM(float desiredVolume = 1.0f, float fadeInTime = 0.5f)
    //{
    //	SettingsManager settingsManager = SettingsManager.GetInstance();
    //	Debug.Assert(settingsManager != null, "DEVN: SettingsManager singleton not found!");

    //	if (desiredVolume < 0.0001f || desiredVolume > 1.0f)
    //		Debug.LogWarning("DEVN: Do not attempt to set volume outside of 0.0001f to 1.0f");

    //	desiredVolume = Mathf.Clamp(desiredVolume, 0.0001f, 1.0f);

    //	float elapsedTime = 0.0f;

    //	while (elapsedTime < fadeInTime)
    //	{
    //		// set volume
    //		float percentage = elapsedTime / fadeInTime;
    //		settingsManager.SetBGMVolume(Mathf.Lerp(0.0001f, desiredVolume, percentage));

    //		// increment time
    //		elapsedTime += Time.deltaTime;

    //		yield return null;
    //	}
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="fadeOutTime"></param>
    ///// <returns></returns>
    //public IEnumerator FadeOutBGM(float fadeOutTime = 0.5f)
    //{
    //	SettingsManager settingsManager = SettingsManager.GetInstance();
    //	Debug.Assert(settingsManager != null, "DEVN: SettingsManager singleton not found!");
    //	float currentVolume = settingsManager.GetBGMVolume();

    //	float elapsedTime = 0.0f;

    //	while (elapsedTime < fadeOutTime)
    //	{
    //		// set volume
    //		float percentage = 1 - (elapsedTime / fadeOutTime);
    //		settingsManager.SetBGMVolume(Mathf.Lerp(0.0001f, currentVolume, percentage));

    //		// increment time
    //		elapsedTime += Time.deltaTime;

    //		yield return null;
    //	}
    //}

    #endregion

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
		m_sfxAudio.GetComponent<AudioSource>().PlayOneShot(sfxClip); // play

        SceneManager sceneManager = SceneManager.GetInstance();

		// instantly proceed to next node if "wait for finish" is false
		if (sceneManager != null && !waitForFinish && nextNode)
			sceneManager.NextNode();

        yield return new WaitForSeconds(sfxClip.length); // wait until SFX has stopped playing

		// proceed to next node if "wait for finish" is true
		if (sceneManager != null && waitForFinish && nextNode)
			sceneManager.NextNode();
	}

	/// <summary>
	/// call this function to play a voice clip one shot!
	/// </summary>
	/// <param name="voiceClip">the voice clip to play</param>
	public void PlayVoiceClip(AudioClip voiceClip)
	{
		m_voiceAudio.GetComponent<AudioSource>().PlayOneShot(voiceClip); // play
	}
}

}

}