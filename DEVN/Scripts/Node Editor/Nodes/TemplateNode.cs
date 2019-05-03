using UnityEngine;
using UnityEditor;

namespace DEVN
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

	public override void Init(Vector2 position)
    {
        base.Init(position);

        // -- set node window title here --
        m_title = "Node";

        // -- set node width and height here --
        m_rectangle.width = 50;
        m_rectangle.height = 100;

        // -- add as many output points as required here --
        AddOutputPoint();

        // -- do any other initialisation stuffs here --
    }

    public override void Copy(BaseNode node, Vector2 position)
    {
        base.Copy(node, position);

        // -- copy any data you want copied here --
    }

    protected override void DrawNodeWindow(int id)
	{
		// -- draw node stuffs here --

		base.DrawNodeWindow(id);
	}

    protected override void ProcessContextMenu()
    {
        //  -- can override a node's right-click context menu here --
        // (optional) 

        base.ProcessContextMenu();
    }

#endif
}

}
