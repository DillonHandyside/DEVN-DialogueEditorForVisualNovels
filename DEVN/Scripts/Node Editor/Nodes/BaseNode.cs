using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DEVN
{

/// <summary>
/// abstract node class which handles all of the dirty work for
/// all of the derived nodes
/// </summary>
public abstract class BaseNode : ScriptableObject
{
    [SerializeField] private int m_nodeID; // node ID

    // node window properties
    [SerializeField] protected Rect m_rectangle;
    [SerializeField] protected string m_title;
    
    // inputs
    public List<int> m_inputs;
    public Rect m_inputPoint;

    // outputs
    public List<int> m_outputs;
    public List<Rect> m_outputPoints;

    #region getters
    
    public int GetNodeID() { return m_nodeID; }
    
    #endregion

#if UNITY_EDITOR

    /// <summary>
    /// override-able node initialisation function
    /// </summary>
    /// <param name="position">the x-y co-ordinates to place the node</param>
    public virtual void Init(Vector2 position)
    {
        Scene scene = NodeEditor.GetScene();

        // assign node ID and increment
        m_nodeID = scene.GetCurrentNodeID();
        scene.SetCurrentNodeID(m_nodeID + 1);

        // initialise node window
        m_rectangle = new Rect(position.x, position.y, 50, 100);

        // initialize list of input nodes, and single input rect
        m_inputs = new List<int>();
        m_inputPoint = new Rect(0, 0, 10, 20);

        // initialize list of output nodes, and list of output rect
        m_outputs = new List<int>();
        m_outputPoints = new List<Rect>();
    }

    /// <summary>
    /// override-able node copy function. Used for copy/pasting
    /// </summary>
    /// <param name="node">the node to make a copy of</param>
    /// <param name="position">the x-y co-ordinates to place the node</param>
    public virtual void Copy(BaseNode node, Vector2 position)
    {
        Scene scene = NodeEditor.GetScene();

        // assign node ID and increment
        m_nodeID = scene.GetCurrentNodeID();
        scene.SetCurrentNodeID(m_nodeID + 1);

        // initialise node window
        m_rectangle = new Rect(position.x, position.y, 50, 100);

        m_title = node.m_title; // set title

		// adjust WxH
		m_rectangle.width = node.m_rectangle.width;
		m_rectangle.height = node.m_rectangle.height;

		// initialize list of input nodes, and single input rect
		m_inputs = new List<int>();
		m_inputPoint = new Rect(0, 0, 10, 20);

		// initialize list of output nodes, and list of output rect
		m_outputs = new List<int>();
		m_outputPoints = new List<Rect>();

        // add an output point for each output point in the copied node
        for (int i = 0; i < node.m_outputPoints.Count; i++)
            AddOutputPoint();
    }

    /// <summary>
    /// draws a node to the node editor
    /// </summary>
    public virtual void Draw()
    {
        // draw all input/output points
        DrawInputPoint();
        DrawOutputPoints();

        // draw all connections to this node
        DrawBeziers();

        // draw the node window
        int windowID = NodeEditor.GetNodeManager().GetNodes().IndexOf(this);
        m_rectangle = GUI.Window(windowID, m_rectangle, DrawNodeWindow, m_title);
    }

    /// <summary>
    /// override-able node draw function. 
    /// </summary>
    /// <param name="id">the ID of the window?</param>
    protected virtual void DrawNodeWindow(int id)
    {
        // disallow any event processing on the starting node
        if (!(this is StartNode))
        {
            ProcessEvents(Event.current);

            // draw "remove" button to allow node deletion
            if (GUI.Button(new Rect(m_rectangle.width - 20, 0, 20, 15), "X"))
                NodeEditor.GetNodeManager().RemoveNode(this);
        }

        GUI.DragWindow(); // allow node dragging
    }

    /// <summary>
    /// add a new output point to the node
    /// </summary>
    protected void AddOutputPoint()
    {
        m_outputPoints.Add(new Rect(0, 0, 10, 20));
        m_outputs.Add(-1); // output points to nothing (node ID of -1)
    }

    /// <summary>
    /// determines the appropriate position for the input point and draws it
    /// </summary>
    void DrawInputPoint()
    {
        if (this is StartNode)
            return; // start node contains no input point

        // determine x position (left of node)
        float xOffset = m_rectangle.x - m_inputPoint.width;
        m_inputPoint.x = xOffset;

        // determine y position (halfway down node)
        float yOffset = m_rectangle.height * 0.5f;
        m_inputPoint.y = m_rectangle.y + yOffset - m_inputPoint.height * 0.5f;

		// draw the input point as a button
		ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
        if (GUI.Button(m_inputPoint, "", connectionManager.m_inputStyle))
			connectionManager.OnClickInput(this);
    }

    /// <summary>
    /// determines the appropriate positions for each output point and draws them
    /// </summary>
    void DrawOutputPoints()
    {
        for (int i = 0; i < m_outputPoints.Count; i++)
        {
            Rect rectangle = m_outputPoints[i];

            // determine x position (right of node)
            float xOffset = m_rectangle.x + m_rectangle.width;
            rectangle.x = xOffset;

            // determine y position (depends on number of output points)
            float yOffset = m_rectangle.height * ((i + 1) / (float)(m_outputPoints.Count + 1));
            rectangle.y = m_rectangle.y + yOffset - rectangle.height * 0.5f;

            m_outputPoints[i] = rectangle;

			// draw the output point as a button
			ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
			if (GUI.Button(m_outputPoints[i], "", connectionManager.m_outputStyle))
				connectionManager.OnClickOutput(this, i);
        }
    }

    /// <summary>
    /// function which determines the start and end point, using this and another node's
    /// input/output points and draws a bezier between them
    /// </summary>
    void DrawBeziers()
    {
        Color colour = Color.white;

        for (int i = 0; i < m_outputs.Count; i++)
        {
            if (m_outputs[i] == -1)
                continue; // skip empty outputs

            BaseNode outputNode = NodeEditor.GetNodeManager().GetNode(m_outputs[i]);

            // define positional variables
            Vector3 startPos = m_outputPoints[i].center;
            Vector3 endPos = outputNode.m_inputPoint.center;
            Vector3 startTangent = m_outputPoints[i].center + Vector2.right * 50;
            Vector3 endTangent = outputNode.m_inputPoint.center + Vector2.left * 50;

            // different bezier colours for condition nodes
            if (this is ConditionNode)
            {
                if (i == 0)
                    colour = Color.green; // true
                else
                    colour = Color.red; // false
            }

            // draw the connection
            Handles.color = colour;
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Handles.color, null, 2);

            // draw a square on bezier to handle connection removal
            if (Handles.Button((startPos + endPos) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                NodeEditor.GetConnectionManager().RemoveConnection(this, i);
        }
    }

    /// <summary>
    /// handles right-click events, such as the creation of new nodes, copy & paste, etc.
    /// </summary>
    protected virtual void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu(); // initialise context menu

        genericMenu.AddDisabledItem(new GUIContent("New Node"));
        genericMenu.AddSeparator("");

        // house-keeping (copy, paste, delete, etc.)
        genericMenu.AddItem(new GUIContent("Copy"), false, () => NodeEditor.GetNodeManager().CopyNode(this));
        genericMenu.AddDisabledItem(new GUIContent("Paste"));
        genericMenu.AddItem(new GUIContent("Delete"), false, () => NodeEditor.GetNodeManager().RemoveNode(this));

        genericMenu.ShowAsContext(); // print context menu
    }

    /// <summary>
    /// event processing function. Used to handle user input. For example; right-click,
    /// and buttons such as "Delete"
    /// </summary>
    /// <param name="e">the input event</param>
    public void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            // on mouse down (mouse hold click)
            case EventType.MouseDown:
                
                // node right click events
                if (e.button == (int)MouseButton.RightClick)
                {
                    ProcessContextMenu(); // open right-click menu
                    e.Use();
                }

                break;

            // on key down
            case EventType.KeyDown:

                // handle deletion using delete key
                if (e.keyCode == KeyCode.Delete)
                {
                    NodeEditor.GetNodeManager().RemoveNode(this); // perform removal
                    GUI.changed = true;
                }

                break;
        }
    }

	/// <summary>
	/// performs translation on the node
	/// </summary>
	/// <param name="translation">the delta value to translate by</param>
	public void Drag(Vector2 translation)
	{
		m_rectangle.position += translation;
	}

#endif
}

}