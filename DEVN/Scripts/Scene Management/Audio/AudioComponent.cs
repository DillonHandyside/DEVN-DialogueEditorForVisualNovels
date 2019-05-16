using UnityEngine;
using UnityEngine.Audio;

namespace DEVN
{

/// <summary>
/// 
/// </summary>
public class AudioComponent : MonoBehaviour
{
	// reference to mixer & relevant mixer groups
	[Header("Audio Mixer")]
	[SerializeField] private AudioMixer m_audioMixer;
	[Header("Mixer Groups")]
	[SerializeField] private AudioMixerGroup m_BGM;
	[SerializeField] private AudioMixerGroup m_ambience;
	[SerializeField] private AudioMixerGroup m_SFX;
	[SerializeField] private AudioMixerGroup m_voice;
		
	#region getters

	public AudioMixer GetAudioMixer() { return m_audioMixer; }
	public AudioMixerGroup GetBGM() { return m_BGM; }
	public AudioMixerGroup GetAmbience() { return m_ambience; }
	public AudioMixerGroup GetSFX() { return m_SFX; }
	public AudioMixerGroup GetVoice() { return m_voice; }

	#endregion
}

}