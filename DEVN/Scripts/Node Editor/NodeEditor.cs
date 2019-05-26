using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN.Nodes;
using DEVN.ScriptableObjects;

namespace DEVN
{

namespace Editor
{

#if UNITY_EDITOR

/// <summary>
/// the node-based graph editor window. Provides all of the functionality for drawing the various graph elements, 
/// and processing user input
/// </summary>
public class NodeEditor : EditorWindow
{
	private Texture2D m_logoDEVN; // watermark logo
	
    private static Scene m_scene; // the current selected Scene
	private Scene m_previousScene; // used for remembering the scene on assembly reload & entering/exiting play mode

    // references to the node and connection managers
    private static NodeManager m_nodeManager;
    private static ConnectionManager m_connectionManager;

    // reference to the mouse position
    private static Vector2 m_mousePosition;

    // drag variables used for scroll-wheel window dragging
    private Vector2 m_offset;
    private Vector2 m_drag;

	#region getters

	public static Scene GetScene() { return m_scene; }
	public static NodeManager GetNodeManager() { return m_nodeManager; }
	public static ConnectionManager GetConnectionManager() { return m_connectionManager; }
	public static Vector2 GetMousePosition() { return m_mousePosition; }

            #endregion

    #region enums
            
    enum MouseButton
    {
        LeftClick,
        RightClick,
        ScrollWheel
    }

    #endregion

    [MenuItem("Window/DEVN/Scene Editor")]
    public static void Init()
    {
		NodeEditor window = GetWindow<NodeEditor>();
		window.titleContent = new GUIContent("Scene Editor");
	}

	/// <summary>
	/// Node editor "awake" function. Creates a new node manager and connection manager, and loads logo
	/// </summary>
	private void OnEnable()
    {
        // initialise the node and connection managers
        m_nodeManager = new NodeManager();
        m_connectionManager = new ConnectionManager();

		m_scene = m_previousScene; // set scene to the remembered scene

        // load logo
        string path = "Assets/DEVN/Textures/logoDEVN.png";
        m_logoDEVN = EditorGUIUtility.Load(path) as Texture2D;
    }

    /// <summary>
    /// Node editor "update" function. Handles the drawing and processing of nodes and connections
    /// </summary>
    void OnGUI()
    {
        EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
        DrawSceneObjectField();

		// don't draw the graph editor if no scriptable object is selected
		if (m_scene == null)
			return;
		
		DrawPageToolbar(); // page toolbar
		EditorGUILayout.EndHorizontal();
                
        // if the scene has changed, check if it needs initialisation
		if (m_scene != m_previousScene)
        {
            m_scene.Init();
            m_previousScene = m_scene;
        }
        
		m_mousePosition = Event.current.mousePosition; // update mouse position

        // update node and connection managers
		m_nodeManager.Update();
		m_connectionManager.Update(); 
		
        // draw grid, nodes and DEVN logo
		DrawContent();
		DrawLogo();

		ProcessEvents(); // process click/button events

		EditorUtility.SetDirty(m_scene); // save scene changes
		Repaint();
    }

	/// <summary>
	/// handles the processing of the right-click context menu, including options
	/// to Add/Remove/Copy/Paste nodes, Add/Remove pages
	/// </summary>
	/// <param name="newNodeOnly">if true, only allow creation of new nodes, and no copy/pasting or pages</param>
	private void ProcessContextMenu(bool newNodeOnly = false)
	{
		GenericMenu contextMenu = new GenericMenu();

		// -- add new nodes to this function! --
		// contextMenu.AddItem(new GUIContent("New Node/Category/Your Node"), false, () => m_nodeManager.AddNode(typeof(YourNode)));
		
		#region in-built DEVN nodes

		// audio nodes
		contextMenu.AddItem(new GUIContent("New Node/Audio/BGM"), false, () => m_nodeManager.AddNode(typeof(BGMNode)));
		contextMenu.AddItem(new GUIContent("New Node/Audio/SFX"), false, () => m_nodeManager.AddNode(typeof(SFXNode)));

		// background nodes
		contextMenu.AddItem(new GUIContent("New Node/Background/Background"), false, () => m_nodeManager.AddNode(typeof(BackgroundNode)));

		// character nodes
		contextMenu.AddItem(new GUIContent("New Node/Character/Scale"), false, () => m_nodeManager.AddNode(typeof(CharacterScaleNode)));
		contextMenu.AddItem(new GUIContent("New Node/Character/Translate"), false, () => m_nodeManager.AddNode(typeof(CharacterTranslateNode)));
		contextMenu.AddSeparator("New Node/Character/");
		contextMenu.AddItem(new GUIContent("New Node/Character/Character"), false, () => m_nodeManager.AddNode(typeof(CharacterNode)));

		// dialogue nodes
		contextMenu.AddItem(new GUIContent("New Node/Dialogue/Branch"), false, () => m_nodeManager.AddNode(typeof(BranchNode)));
		contextMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue"), false, () => m_nodeManager.AddNode(typeof(DialogueNode)));
		contextMenu.AddSeparator("New Node/Dialogue/");
		contextMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue Box"), false, () => m_nodeManager.AddNode(typeof(DialogueBoxNode)));

		// utility nodes
		contextMenu.AddItem(new GUIContent("New Node/Utility/Delay"), false, () => m_nodeManager.AddNode(typeof(DelayNode)));
		contextMenu.AddItem(new GUIContent("New Node/Utility/Page"), false, () => m_nodeManager.AddNode(typeof(PageNode)));

		// variable nodes
		contextMenu.AddItem(new GUIContent("New Node/Variable/Condition"), false, () => m_nodeManager.AddNode(typeof(ConditionNode)));
		contextMenu.AddItem(new GUIContent("New Node/Variable/Modify"), false, () => m_nodeManager.AddNode(typeof(ModifyNode)));

		// end node
		contextMenu.AddItem(new GUIContent("New Node/End"), false, () => m_nodeManager.AddNode(typeof(EndNode)));

	    #endregion

		#region copy/paste/delete nodes

		// house-keeping (copy, paste, delete, etc.)
		contextMenu.AddDisabledItem(new GUIContent("Copy Node")); // copy
		if (m_nodeManager.GetClipboard() != null && !newNodeOnly)
			contextMenu.AddItem(new GUIContent("Paste Node"), false, m_nodeManager.PasteNode); // paste
		else
			contextMenu.AddDisabledItem(new GUIContent("Paste Node"));
		contextMenu.AddDisabledItem(new GUIContent("Remove Node")); // remove

		#endregion

		#region add/remove pages

		// new page
		contextMenu.AddSeparator("");
		if (!newNodeOnly)
			contextMenu.AddItem(new GUIContent("New Page"), false, m_scene.NewPage);
		else
			contextMenu.AddDisabledItem(new GUIContent("New Page"));

		// remove page, disallow removal if only one page exists
		if (m_scene.GetPages().Count > 1 && !newNodeOnly)
			contextMenu.AddItem(new GUIContent("Remove Page"), false, m_scene.DeletePage);
		else
			contextMenu.AddDisabledItem(new GUIContent("Remove Page"));

		#endregion

		contextMenu.ShowAsContext();
	}

    /// <summary>
    /// process mouse click events, such as right-clicking for context menu, connecting nodes, etc.
    /// </summary>
    private void ProcessEvents()
    {
        Event current = Event.current; // retrieve current event
        m_drag = Vector2.zero;

        switch (current.type)
        {
            case EventType.MouseDown: // mouse click event handling

				// left click event/s
				if (current.button == (int)MouseButton.LeftClick)
				{
					m_connectionManager.ClearConnectionSelection(); // clear connection
                    GUI.FocusControl(null); // unfocus any selected input fields
				}

				// right click event/s
				if (current.button == (int)MouseButton.RightClick)
					ProcessContextMenu();

				break;

            case EventType.MouseDrag: // mouse movement event handling
                        
                if (current.button == (int)MouseButton.ScrollWheel)
                    DragNodes(current.delta); // scroll wheel to drag all nodes

                if (current.button == (int)MouseButton.LeftClick && current.alt)
                    DragNodes(current.delta); // or alt+left click to drag all nodes

                break;
        }
    }

    /// <summary>
    /// function which draws an object field for selecting a scene to edit
    /// </summary>
    private void DrawSceneObjectField()
    {
        // this object field shares a toolbar with the pages, so it needs horizontal groups
		EditorGUILayout.BeginHorizontal(GUILayout.Width(256));
		EditorGUIUtility.labelWidth = 96;

        m_scene = EditorGUILayout.ObjectField("Current Scene ", m_scene, typeof(Scene), true) as Scene;

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator(); // provide spacing between the object field and the page toolbar
    }

    /// <summary>
    /// function which draws a toolbar for indicating and selecting the current page the user is editing
    /// </summary>
    private void DrawPageToolbar()
    {
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(48)); // limit page button width to 48 pixels

        // build a list of page strings in the form of "Page 1, Page 2, etc."
        string[] pages = new string[m_scene.GetPages().Count];
        for (int i = 0; i < pages.Length; i++)
            pages[i] = "Page " + (i + 1);
        
		int currentPage = m_scene.GetCurrentPageID();
		GUILayoutOption maxWidth = GUILayout.MaxWidth(Screen.width - 272); // 272 to take into account the object field
		m_scene.SetCurrentPage(GUILayout.Toolbar(currentPage, pages, maxWidth)); // draw the toolbar

		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// function which draws the grid, and all of the node windows
	/// </summary>
	private void DrawContent()
	{
		Rect contentRect = GUILayoutUtility.GetLastRect();
        contentRect.y += contentRect.height;
        contentRect.height = Screen.height - 56;

                //Texture2D texture = new Texture2D(1, 1);
                //texture.SetPixel(0, 0, Color.green);
                //texture.wrapMode = TextureWrapMode.Repeat;
                //texture.Apply();
                //GUI.DrawTexture(contentRect, texture);
        GUI.BeginScrollView(contentRect, new Vector2(0, 0), contentRect);

        // draw grid
        DrawGrid(20, Color.grey, 0.2f);
		DrawGrid(100, Color.grey, 0.4f);

		// draw nodes
		BeginWindows();
		m_nodeManager.DrawNodes();
		EndWindows();
                
        GUI.EndScrollView();

        #region scroll view to implement
        //GUI.BeginScrollView(scrollViewPosition, m_scrollPosition, scrollViewPosition);

        //float minX, maxX, minY, maxY;
        //GetMinMaxXY(out minX, out maxX, out minY, out maxY);
        //minX = Mathf.Clamp(minX, Mathf.NegativeInfinity, 0);
        //maxX = Mathf.Clamp(maxX, scrollViewPosition.width, Mathf.Infinity);
        //minY = Mathf.Clamp(minY, Mathf.NegativeInfinity, 0);
        //maxY = Mathf.Clamp(maxY, scrollViewPosition.height, Mathf.Infinity);

        //float width = maxX - minX;
        //float height = maxY - minY;
        //Rect scrollViewRect = new Rect(0, 0, width, height);

        //m_scrollPosition = GUI.BeginScrollView(scrollViewPosition, m_scrollPosition, scrollViewRect);

        //

		//GUI.EndScrollView();
        #endregion
	}
            
    /// <summary>
    /// draws the DEVN water-mark logo. For fanciness
    /// </summary>
    private void DrawLogo()
    {
        float xPosLogo = Screen.width - 80;
        float yPosLogo = Screen.height - 80;
        float xPosText = xPosLogo - 118;
        float yPosText = yPosLogo + 40;

        // adjust transparency
        GUI.color = new Color(1, 1, 1, 0.25f);

        // draw logo & text
        GUI.DrawTexture(new Rect(xPosLogo, yPosLogo, 100, 50), m_logoDEVN);
        GUI.Label(new Rect(xPosText, yPosText, 300, 20), "Dialogue Editor for Visual Novels");

        // reset transparency
        GUI.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// function which draws the grid using Handles.DrawLine
    /// </summary>
    /// <param name="spacing">the space between lines</param>
    /// <param name="colour">the colour of the lines</param>
    /// <param name="transparency">the transparency of the lines</param>
    private void DrawGrid(float spacing, Color colour, float transparency)
    {
        int widthDivs = Mathf.CeilToInt(Screen.width / spacing);
        int heightDivs = Mathf.CeilToInt(Screen.height / spacing);

        // begin drawing
        Handles.BeginGUI();
        Handles.color = new Color(colour.r, colour.g, colour.b, transparency);

        m_offset += m_drag * 0.5f; // offset the grid when all nodes are dragged
        Vector3 newOffset = new Vector3(m_offset.x % spacing, m_offset.y % spacing, 0);

        // draw vertical lines
        for (int i = 0; i <= widthDivs; i++)
            Handles.DrawLine(new Vector3(spacing * i, -spacing, 0) + newOffset,
                             new Vector3(spacing * i, Screen.height + spacing, 0f) + newOffset);

        // draw horizontal
        for (int j = 0; j <= heightDivs; j++)
            Handles.DrawLine(new Vector3(-spacing, spacing * j, 0) + newOffset,
                             new Vector3(Screen.width + spacing, spacing * j, 0f) + newOffset);

        // end drawing
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    /// <summary>
    /// function which iterates over all nodes and drags each one by delta
    /// </summary>
    /// <param name="delta">the incremental value to drag by</param>
    void DragNodes(Vector2 delta)
    {
        List<BaseNode> nodes = m_nodeManager.GetNodes(); // retrieve all nodes
        m_drag = delta; // save delta for dragging grid

        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Drag(delta); // drag each node
    }

    /// <summary>
    /// helper function which retrieves the min and max x and y positions of all the nodes in the editor
    /// </summary>
    /// <param name="minX">feed in a float parametre here using "out yourFloatName"</param>
    /// <param name="maxX">feed in a float parametre here using "out yourFloatName"</param>
    /// <param name="minY">feed in a float parametre here using "out yourFloatName"</param>
    /// <param name="maxY">feed in a float parametre here using "out yourFloatName"</param>
    private void GetMinMaxXY(out float minX, out float maxX, out float minY, out float maxY)
    {
        List<BaseNode> nodes = m_nodeManager.GetNodes(); // retrieve all nodes

        // set min to infinity, and max to negative infinity
        minX = Mathf.Infinity;
        maxX = Mathf.NegativeInfinity;
        minY = Mathf.Infinity;
        maxY = Mathf.NegativeInfinity;
                
        for (int i = 0; i < nodes.Count; i++)
        {
            Rect nodeRect = nodes[i].GetNodeRect();

            if (minX > nodeRect.x)
                minX = nodeRect.x; // new minX
            if (maxX < nodeRect.x + nodeRect.width)
                maxX = nodeRect.x + nodeRect.width; // new maxX
            if (minY > nodeRect.y)
                minY = nodeRect.y; // new minY
            if (maxY < nodeRect.y + nodeRect.height)
                maxY = nodeRect.y + nodeRect.height; // new maxY
        }
    }
}

#endif

}

}