using UnityEngine;
using UnityEditor;

namespace DEVN
{

namespace Nodes
{

/// <summary>
/// a template node which showcases all of the required code to
/// initialise, copy and draw a node
/// </summary>
[System.Serializable]
public class TemplateNode : BaseNode
{
	// -- place variables here --
	// (ensure any variables you want to save are either public or tagged
	// with [SerializeField])

#if UNITY_EDITOR

    /// <summary>
    /// overridden constructor
    /// </summary>
    /// <param name="position">position of creation</param>
	public override void Init(Vector2 position)
    {
        base.Init(position);

        // -- set node window title here --
        m_title = "Node";

        // -- set node width and height here --
        m_rectangle.width = 100;
        m_rectangle.height = 50;

        // -- add as many output points as required here --
        AddOutputPoint();

        // -- do any other initialisation stuffs here --
    }

    /// <summary>
    /// overridden copy constructor
    /// </summary>
    /// <param name="node">node to copy</param>
    /// <param name="position">position to copy to</param>
    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        // -- copy any data you want copied here --
        // YourNodeType yourNode = node as YourNodeType
    }

    /// <summary>
    /// overridden draw function
    /// </summary>
    /// <param name="id">the node window ID</param>
    protected override void DrawNodeWindow(int id)
	{
		// -- draw node stuffs here --

		base.DrawNodeWindow(id);
	}

#endif
}

}

}
