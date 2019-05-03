using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

[CreateAssetMenu(fileName = "New Character", menuName = "DEVN/Character")]
public class Character : ScriptableObject
{
    public string m_name;
    public List<Sprite> m_sprites = new List<Sprite>();
    public List<AudioClip> m_audioClips = new List<AudioClip>();
}

}
