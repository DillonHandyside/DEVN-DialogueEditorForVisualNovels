using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DEVN;

#if UNITY_EDITOR

enum MouseButton
{
    LeftClick,
    RightClick,
    ScrollWheel
}

public class NodeEditor : EditorWindow
{
	private Texture2D m_logoDEVN; // watermark logo
	
    private static Scene m_scene; // the current selected Scene

	// this scene is used for remembering the current scene on assembly reload & 
	// entering/exiting play mode
	private Scene m_previousScene;

    // references to the node and connection managers
    private static NodeManager m_nodeManager;
    private static ConnectionManager m_connectionManager;

    // reference to the mouse position
    private static Vector2 m_mousePosition;

    // drag variables used for scroll-wheel window dragging
    private Vector2 m_offset;
    private Vector2 m_drag;

    private Vector2 m_scrollPosition = Vector2.zero;

	#region getters

	public static Scene GetScene() { return m_scene; }
	public static NodeManager GetNodeManager() { return m_nodeManager; }
	public static ConnectionManager GetConnectionManager() { return m_connectionManager; }
	public static Vector2 GetMousePosition() { return m_mousePosition; }

	#endregion

	[MenuItem("Window/DEVN/Dialogue Editor")]
    public static void Init()
    {
		NodeEditor window = GetWindow<NodeEditor>();
		window.titleContent = new GUIContent("Dialogue Editor");
    }

    /// <summary>
    /// Node editor "awake" function. Initialises all relevant variables
    /// </summary>
    private void OnEnable()
    {
        // initialise the node and connection managers
        m_nodeManager = new NodeManager();
        m_connectionManager = new ConnectionManager();

		m_scene = m_previousScene;

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
		EditorGUILayout.BeginHorizontal(GUILayout.Width(256));
		EditorGUIUtility.labelWidth = 96;
        m_scene = EditorGUILayout.ObjectField("Current Scene ", m_scene, typeof(Scene), true) as Scene;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();

		// don't draw the graph editor if no scriptable object is selected
		if (m_scene == null)
			return;
		
		m_scene.Init();
		m_nodeManager.UpdateNodes();

		if (m_scene != m_previousScene)
			m_previousScene = m_scene;
		
		// update mouse position reference
		m_mousePosition = Event.current.mousePosition;

		// draw all editor elements in appropriate order
		DrawToolbar();
		EditorGUILayout.EndHorizontal();
		DrawContent();
		DrawLogo();
		
		ProcessEvents(Event.current); // process click/button events

		EditorUtility.SetDirty(m_scene); // save scene changes
		Repaint();
    }

    /// <summary>
    /// handles right-click events, such as the creation of new nodes, copy & paste, etc.
    /// </summary>
    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu(); // initialise context menu

		// -- add new nodes to this function! --
		// genericMenu.AddItem(new GUIContent("New Node/Category/Your Node"), false, () => m_nodeManager.AddNode(typeof(YourNode)));

		#region in-built DEVN nodes

		// audio nodes
		genericMenu.AddItem(new GUIContent("New Node/Audio/BGM"), false, () => m_nodeManager.AddNode(typeof(BGMNode)));
		genericMenu.AddItem(new GUIContent("New Node/Audio/SFX"), false, () => m_nodeManager.AddNode(typeof(SFXNode)));

		// background nodes
		genericMenu.AddItem(new GUIContent("New Node/Background/Background"), false, () => m_nodeManager.AddNode(typeof(BackgroundNode)));

		// character nodes
		genericMenu.AddItem(new GUIContent("New Node/Character/Scale"), false, () => m_nodeManager.AddNode(typeof(CharacterScaleNode)));
		genericMenu.AddItem(new GUIContent("New Node/Character/Translate"), false, () => m_nodeManager.AddNode(typeof(CharacterTranslateNode)));
		genericMenu.AddSeparator("New Node/Character/");
		genericMenu.AddItem(new GUIContent("New Node/Character/Character"), false, () => m_nodeManager.AddNode(typeof(CharacterNode)));

		// dialogue nodes
		genericMenu.AddItem(new GUIContent("New Node/Dialogue/Branch"), false, () => m_nodeManager.AddNode(typeof(BranchNode)));
        genericMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue"), false, () => m_nodeManager.AddNode(typeof(DialogueNode)));
		genericMenu.AddSeparator("New Node/Dialogue/");
		genericMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue Box"), false, () => m_nodeManager.AddNode(typeof(DialogueBoxNode)));

        // utility nodes
        genericMenu.AddItem(new GUIContent("New Node/Utility/Delay"), false, () => m_nodeManager.AddNode(typeof(DelayNode)));
        genericMenu.AddItem(new GUIContent("New Node/Utility/Page"), false, () => m_nodeManager.AddNode(typeof(PageNode)));

        // variable nodes
        genericMenu.AddItem(new GUIContent("New Node/Variable/Condition"), false, () => m_nodeManager.AddNode(typeof(ConditionNode)));
        genericMenu.AddItem(new GUIContent("New Node/Variable/Modify"), false, () => m_nodeManager.AddNode(typeof(ModifyNode)));

        // end node
        genericMenu.AddItem(new GUIContent("New Node/End"), false, () => m_nodeManager.AddNode(typeof(EndNode)));

		#endregion

		#region page creation/deletion

		// new page
		genericMenu.AddSeparator("");
        genericMenu.AddItem(new GUIContent("New Page"), false, m_scene.NewPage);

		// remove page, disallow removal if only one page exists
        if (m_scene.GetPages().Count > 1)
            genericMenu.AddItem(new GUIContent("Delete Page"), false, m_scene.DeletePage);
        else
            genericMenu.AddDisabledItem(new GUIContent("Delete Page"));

		#endregion

		#region node copy/paste/deletion

		genericMenu.AddSeparator("");

        // house-keeping (copy, paste, delete, etc.)
        genericMenu.AddDisabledItem(new GUIContent("Copy"));
        if (m_nodeManager.GetClipboard() != null)
            genericMenu.AddItem(new GUIContent("Paste"), false, m_nodeManager.PasteNode);
        else
            genericMenu.AddDisabledItem(new GUIContent("Paste"));
        genericMenu.AddDisabledItem(new GUIContent("Delete"));

		#endregion

		genericMenu.ShowAsContext(); // print context menu
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    void ProcessEvents(Event e)
    {
        m_drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:

                // clear connection selection on left click
                if (e.button == (int)MouseButton.LeftClick)
                    m_connectionManager.ClearConnectionSelection();

                // process right-click context menu
                if (e.button == (int)MouseButton.RightClick)
                    ProcessContextMenu();

                break;

            case EventType.MouseDrag:

                // drag all nodes and grid elements upon scroll wheel click
                if (e.button == (int)MouseButton.ScrollWheel)
                    DragAll(e.delta);

                break;
        }
    }

	/// <summary>
	/// 
	/// </summary>
	private void DrawContent()
	{
		Rect scrollViewRect = GUILayoutUtility.GetLastRect();
		scrollViewRect.y += scrollViewRect.height;
		scrollViewRect.height = Screen.height - 56;
		m_scrollPosition = GUI.BeginScrollView(scrollViewRect, m_scrollPosition, scrollViewRect);

		// grid
		DrawGrid(20, 0.2f, Color.grey);
		DrawGrid(100, 0.4f, Color.grey);

		// nodes
		BeginWindows();
		m_nodeManager.DrawNodes();
		EndWindows();

		GUI.EndScrollView();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridSpacing"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(Screen.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(Screen.height / gridSpacing);

        // begin drawing
        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        m_offset += m_drag * 0.5f;
        Vector3 newOffset = new Vector3(m_offset.x % gridSpacing, m_offset.y % gridSpacing, 0);

        // draw vertical lines
        for (int i = 0; i <= widthDivs; i++)
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                             new Vector3(gridSpacing * i, Screen.height + gridSpacing, 0f) + newOffset);

        // draw horizontal
        for (int j = 0; j <= heightDivs; j++)
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                             new Vector3(Screen.width + gridSpacing, gridSpacing * j, 0f) + newOffset);

        // end drawing
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawToolbar()
    {
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(48));

        string[] pages = new string[m_scene.GetPages().Count];
        for (int i = 0; i < pages.Length; i++)
            pages[i] = "Page " + (i + 1);

		int currentPage = m_scene.GetCurrentPageID();
		GUILayoutOption maxWidth = GUILayout.MaxWidth(Screen.width - 272);
		m_scene.SetCurrentPage(GUILayout.Toolbar(currentPage, pages, maxWidth));

		EditorGUILayout.EndHorizontal();
	}

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <param name="delta"></param>
    void DragAll(Vector2 delta)
    {
        List<BaseNode> nodes = m_nodeManager.GetNodes();
        m_drag = delta;

        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Drag(delta);

        GUI.changed = true;
    }
}

#endif