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

    // the current selected DialogueScene scriptable object
    private static Scene m_scene;

    // references to the node and connection managers
    private static NodeManager m_nodeManager;
    private static ConnectionManager m_connectionManager;

    // reference to the mouse position
    private static Vector2 m_mousePosition;

    // drag variables used for middle-mouse window dragging
    private Vector2 m_offset;
    private Vector2 m_drag;

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

        // load logo
        string path = "Assets/DEVN/Textures/logoDEVN.png";
        m_logoDEVN = EditorGUIUtility.Load(path) as Texture2D;

        Load(true);
    }

    /// <summary>
    /// Node editor "update" function. Handles the drawing and processing of nodes and connections
    /// </summary>
    void OnGUI()
    {
        Load();

        // don't draw the graph editor if no scriptable object is selected
        if (m_scene == null)
        {
            GUI.Label(new Rect(10, 10, 500, 20), "Please select a DialogueScene");
            return;
        }

        // update mouse position reference
        m_mousePosition = Event.current.mousePosition;

        // draw all editor elements in appropriate order
        DrawGrid(20, 0.2f, Color.grey);
        DrawGrid(100, 0.4f, Color.grey);
        BeginWindows();
        m_nodeManager.DrawNodes();
        EndWindows();
        DrawToolbar();
		DrawLogo();

        // event processing
        ProcessEvents(Event.current);

        if (GUI.changed)
        {
            Save();
            Repaint();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Save()
    {
        m_scene.SaveNodes(m_nodeManager.GetNodes());
        EditorUtility.SetDirty(m_scene);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Load(bool isReload = false)
    {
        // if the user clicks on a new dialogue scene scriptable object
        if ((Selection.activeObject != m_scene || isReload ||
            EditorApplication.isPlayingOrWillChangePlaymode) &&
            Selection.activeObject is Scene)
        {
            // assign the dialogue scene to the one the user clicked
            m_scene = Selection.activeObject as Scene;

            // load the nodes
            m_nodeManager.SetNodes(m_scene.LoadNodes());
        }
        else if (Selection.activeObject != m_scene)
            m_scene = null;
    }

    /// <summary>
    /// handles right-click events, such as the creation of new nodes, copy & paste, etc.
    /// </summary>
    void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu(); // initialise context menu

        // -- add new nodes to this function! --
        // genericMenu.AddItem(new GUIContent("New Node/Category/Node"), false, () => m_nodeManager.AddNode(typeof(YourNode)));

        // audio relevant nodes
        genericMenu.AddItem(new GUIContent("New Node/Audio/BGM"), false, () => m_nodeManager.AddNode(typeof(BGMNode)));
		genericMenu.AddItem(new GUIContent("New Node/Audio/SFX"), false, () => m_nodeManager.AddNode(typeof(SFXNode)));

		// background relevant nodes
		genericMenu.AddItem(new GUIContent("New Node/Background/Background"), false, () => m_nodeManager.AddNode(typeof(BackgroundNode)));
		genericMenu.AddItem(new GUIContent("New Node/Background/CG"), false, () => m_nodeManager.AddNode(typeof(CGNode)));

		// character relevant nodes
		genericMenu.AddItem(new GUIContent("New Node/Character/Character"), false, () => m_nodeManager.AddNode(typeof(CharacterNode)));

        // dialogue relevant nodes
        genericMenu.AddItem(new GUIContent("New Node/Dialogue/Branch"), false, () => m_nodeManager.AddNode(typeof(BranchNode)));
        genericMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue"), false, () => m_nodeManager.AddNode(typeof(DialogueNode)));
        genericMenu.AddItem(new GUIContent("New Node/Dialogue/Dialogue Box"), false, () => m_nodeManager.AddNode(typeof(DialogueBoxNode)));

        // utility relevant nodes
        genericMenu.AddItem(new GUIContent("New Node/Utility/Delay"), false, () => m_nodeManager.AddNode(typeof(DelayNode)));
        genericMenu.AddItem(new GUIContent("New Node/Utility/Page"), false, () => m_nodeManager.AddNode(typeof(PageNode)));

        // variable relevant nodes
        genericMenu.AddItem(new GUIContent("New Node/Variable/Condition"), false, () => m_nodeManager.AddNode(typeof(ConditionNode)));
        genericMenu.AddItem(new GUIContent("New Node/Variable/Modify"), false, () => m_nodeManager.AddNode(typeof(ModifyNode)));

        // end node
        genericMenu.AddItem(new GUIContent("New Node/End"), false, () => m_nodeManager.AddNode(typeof(EndNode)));
        genericMenu.AddSeparator("");
        genericMenu.AddItem(new GUIContent("New Page"), false, NewPage);

        if (m_scene.GetPages().Count > 1)
            genericMenu.AddItem(new GUIContent("Remove Page"), false, RemovePage);
        else
            genericMenu.AddDisabledItem(new GUIContent("Remove Page"));

        genericMenu.AddSeparator("");

        // house-keeping (copy, paste, delete, etc.)
        genericMenu.AddDisabledItem(new GUIContent("Copy"));
        if (m_nodeManager.GetClipboard() != null)
            genericMenu.AddItem(new GUIContent("Paste"), false, m_nodeManager.PasteNode);
        else
            genericMenu.AddDisabledItem(new GUIContent("Paste"));
        genericMenu.AddDisabledItem(new GUIContent("Delete"));

        genericMenu.ShowAsContext(); // print context menu
    }

    /// <summary>
    /// 
    /// </summary>
    private void NewPage()
    {
        m_scene.NewPage();
        Load(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void RemovePage()
    {
        if (EditorUtility.DisplayDialog("Wait!", 
            "Are you sure you want to delete this page?", "Yes", "No"))
            m_scene.GetPages().RemoveAt(m_scene.GetCurrentPage());
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

                //
                if (e.button == (int)MouseButton.LeftClick)
                    m_connectionManager.ClearConnectionSelection();

                //
                if (e.button == (int)MouseButton.RightClick)
                    ProcessContextMenu();
                break;

            case EventType.MouseDrag:

                //
                if (e.button == (int)MouseButton.ScrollWheel)
                    DragAll(e.delta);
                break;
        }
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
        string[] pages = new string[m_scene.GetPages().Count];
        for (int i = 0; i < pages.Length; i++)
            pages[i] = "Page " + (i + 1);

        int prevPage = m_scene.GetCurrentPage();
        m_scene.SetCurrentPage(GUILayout.Toolbar(prevPage, pages));

        if (m_scene.GetCurrentPage() != prevPage)
            Load(true);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawLogo()
    {
        float xPosLogo = Screen.width - 155;
        float yPosLogo = Screen.height - 100;
        float xPosText = Screen.width - 195;
        float yPosText = yPosLogo + 60;

        // adjust transparency
        GUI.color = new Color(1, 1, 1, 0.25f);

        // draw logo & text
        GUI.DrawTexture(new Rect(xPosLogo, yPosLogo, 150.0f, 75.0f), m_logoDEVN);
        GUI.Label(new Rect(xPosText, yPosText, 300.0f, 20.0f), "Dialogue Editor for Visual Novels");

        // reset transparency
        GUI.color = new Color(1, 1, 1, 1.0f);
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