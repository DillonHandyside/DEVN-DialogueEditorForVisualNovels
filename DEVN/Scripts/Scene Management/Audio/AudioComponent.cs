using UnityEngine;
using UnityEngine.Audio;

namespace DEVN
{

namespace Components
{

/// <summary>
/// component to be placed on the gameobject containing "SceneManager", contains
/// references to an audio mixer and it's different groups
/// </summary>
public class AudioComponent : MonoBehaviour
{
	// reference to mixer
	[Header("Audio Mixer")]
	[Tooltip("Plug in the DEVN Mixer found in \"DEVN\\Audio\"")]
	[SerializeField] private AudioMixer m_audioMixer;

	// reference to mixer groups
	[Header("Mixer Groups")]
	[Tooltip("Plug in the BGM mixer group found in the DEVN mixer")]
	[SerializeField] private AudioMixerGroup m_BGM;
	[Tooltip("Plug in the ambience mixer group found in the DEVN mixer")]
	[SerializeField] private AudioMixerGroup m_ambience;
	[Tooltip("Plug in the SFX mixer group found in the DEVN mixer")]
	[SerializeField] private AudioMixerGroup m_SFX;
	[Tooltip("Plug in the voice mixer group found in the DEVN mixer")]
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

}