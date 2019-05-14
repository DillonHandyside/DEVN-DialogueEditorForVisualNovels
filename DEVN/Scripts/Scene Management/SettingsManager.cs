using UnityEngine;
using UnityEngine.Audio;

namespace DEVN
{

/// <summary>
/// singleton settings class which houses the game settings such as
/// text speed, game audio, etc.
/// </summary>
public class SettingsManager : MonoBehaviour
{
	// singleton
	private static SettingsManager m_instance;

	// dialogue settings, e.g. text speed and auto speed
	[Header("Dialogue Settings")]
	[Range(0, 1)]
	[SerializeField] private float m_textSpeed = 0.5f;
	[Range(0, 1)]
	[SerializeField] private float m_autoSpeed = 1;

	// audio volume settings
	[Header("Audio Settings")]
	[Range(0.0001f, 1)]
	[SerializeField] private float m_masterVolume = 1;
	[Range(0.0001f, 1)]
	[SerializeField] private float m_bgmVolume = 1;
	[Range(0.0001f, 1)]
	[SerializeField] private float m_ambienceVolume = 1;
	[Range(0.0001f, 1)]
	[SerializeField] private float m_sfxVolume = 1;
	[Range(0.0001f, 1)]
	[SerializeField] private float m_voiceVolume = 1;

	#region getters

	public static SettingsManager GetInstance() { return m_instance; }
	public float GetTextSpeed() { return m_textSpeed; }
	public float GetMasterVolume() { return m_masterVolume; }
	public float GetBGMVolume() { return m_bgmVolume; }
	public float GetAmbientVolume() { return m_ambienceVolume; }
	public float GetSFXVolume() { return m_sfxVolume; }
	public float GetVoiceVolume() { return m_voiceVolume; }
		 
	#endregion

	#region setters

	public void SetTextSpeed(float textSpeed) { m_textSpeed = textSpeed; }
	public void SetAutoSpeed(float autoSpeed) { m_autoSpeed = autoSpeed; }
	public void SetMasterVolume(float masterVolume) { m_masterVolume = masterVolume; }
	public void SetBGMVolume(float bgmVolume) { m_bgmVolume = bgmVolume; }
	public void SetAmbienceVolume(float ambienceVolume) { m_ambienceVolume = ambienceVolume; }
	public void SetSFXVolume(float sfxVolume) { m_sfxVolume = sfxVolume; }
	public void SetVoiceVolume(float voiceVolume) { m_voiceVolume = voiceVolume; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Start ()
	{
		m_instance = this; // initialise singleton
	}
	
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
        DialogueManager.GetInstance().SetDialogueSpeed(m_textSpeed);
        DialogueManager.GetInstance().SetAutoSpeed(m_autoSpeed);

		// update mixer settings. Volume between -80 to 0 db
		AudioMixer audioMixer = AudioManager.GetInstance().GetAudioMixer();

		audioMixer.SetFloat("volumeMaster", Mathf.Log10(m_masterVolume) * 20); // master
		audioMixer.SetFloat("volumeBGM", Mathf.Log10(m_bgmVolume) * 20); // BGM
		audioMixer.SetFloat("volumeAmbience", Mathf.Log10(m_ambienceVolume) * 20); // ambience
		audioMixer.SetFloat("volumeSFX", Mathf.Log10(m_sfxVolume) * 20); // SFX
		audioMixer.SetFloat("volumeVoice", Mathf.Log10(m_voiceVolume) * 20); // voice
	}
}

}
