using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

namespace ScriptableObjects
{

/// <summary>
/// character asset to be created by the user. Contains different character elements such as their name, and
/// a collection of their sprites and audio samples
/// </summary>
[CreateAssetMenu(fileName = "New Character", menuName = "DEVN/Character")]
public class Character : ScriptableObject
{
    // character elements
    [SerializeField] private string m_name;
    [SerializeField] private List<Sprite> m_sprites = new List<Sprite>();
    [SerializeField] private List<AudioClip> m_audioClips = new List<AudioClip>();

    #region getters

    public string GetName() { return m_name; }
    public List<Sprite> GetSprites() { return m_sprites; }
    public List<AudioClip> GetAudioClips() { return m_audioClips; }

    #endregion
}

}

}
