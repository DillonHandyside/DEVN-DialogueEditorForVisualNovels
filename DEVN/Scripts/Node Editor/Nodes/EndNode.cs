using UnityEngine;
using UnityEditor;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// a node which dictates the end of a scene. No outputs
/// </summary>
[System.Serializable]
public class EndNode : BaseNode
{
    [SerializeField] private Scene m_nextScene;
    public Scene GetNextScene() { return m_nextScene; }

#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor
    /// </summary>
    /// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);
        
        m_rectangle.width = 100;
        m_rectangle.height = 58;

        m_title = "End";
    }

    /// <summary>
    /// overridden copy constructor, copies "Next Scene" reference
    /// </summary>
    /// <param name="node">node to copy</param>
    /// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);
        
        // copy relevant variable/s
        m_nextScene = (node as EndNode).GetNextScene();
    }

    /// <summary>
    /// overridden draw function, prints a label and object field for
    /// the next scene
    /// </summary>
    /// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
    {
		// draw label and object field for "next scene"
		EditorGUILayout.LabelField("Next Scene");
        m_nextScene = EditorGUILayout.ObjectField(m_nextScene, typeof(Scene), false) as Scene;

        base.DrawNodeWindow(id);
	}

#endif
}

}

}