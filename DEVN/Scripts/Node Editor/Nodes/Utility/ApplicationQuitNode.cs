using UnityEngine;
using UnityEditor;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// a utility node which determines when the application should quit
/// </summary>
[System.Serializable]
public class ApplicationQuitNode : BaseNode
{
#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor
    /// </summary>
    /// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
                
        m_title = "Quit";
                
        m_rectangle.width = 100;
        m_rectangle.height = 50;
    }

#endif
}

}

}