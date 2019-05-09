using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

public class SceneManager : MonoBehaviour
{
	// singleton
	private static SceneManager m_instance;

	[SerializeField] private Scene m_startScene;
	[HideInInspector]
    [SerializeField] private Scene m_currentScene;
    private List<BaseNode> m_sceneNodes;
    private BaseNode m_currentNode;
	
	private bool m_isInputAllowed = false;

	#region getters

	public static SceneManager GetInstance() { return m_instance; }
	public BaseNode GetCurrentNode() { return m_currentNode; }

	#endregion

	#region setters

	public void SetIsInputAllowed(bool isInputAllowed) { m_isInputAllowed = isInputAllowed; }

	#endregion

	/// <summary>
	/// 
	/// </summary>
	void Start ()
    {
		m_instance = this; // initialise singleton

		NewScene(m_startScene);
	}
	
	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		DialogueManager dialogueManager = DialogueManager.GetInstance();

		// dialogue box toggle
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			SetIsInputAllowed(!m_isInputAllowed);
			dialogueManager.ToggleDialogueBox();
		}

		if (m_isInputAllowed && Input.GetKeyDown(KeyCode.Space))
		{
			if (dialogueManager.GetIsTyping())
				dialogueManager.SkipTypewrite();
			else
				NextNode();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="scene"></param>
	public void NewScene(Scene scene)
	{
		m_currentScene = scene;

		m_sceneNodes = m_currentScene.GetNodes(0);
		m_currentNode = m_sceneNodes[0]; // start node

		NextNode();
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageNumber"></param>
    public void NewPage(int pageNumber)
    {
        m_sceneNodes = m_currentScene.GetNodes(pageNumber);
        m_currentNode = m_sceneNodes[0]; // start node

        NextNode();
    }

	/// <summary>
	/// 
	/// </summary>
	public BaseNode GetNode(int nodeID)
	{
		for (int i = 0; i < m_sceneNodes.Count; i++)
		{
			if (m_sceneNodes[i].GetNodeID() == nodeID)
				return m_sceneNodes[i];
		}

		return null;
	}

    /// <summary>
    /// 
    /// </summary>
    public void NextNode()
    {
		// only one node to proceed to
		if (m_currentNode.m_outputs.Count == 1)
			m_currentNode = GetNode(m_currentNode.m_outputs[0]);

        // other node types here

        UpdateScene();
    }

	/// <summary>
	/// 
	/// </summary>
	private void Transition()
	{
		if (m_currentNode is EndNode)
		{
			EndNode endNode = m_currentNode as EndNode;

			if (endNode.GetNextScene() == null)
			{
				// -- place custom scene transition code here -- 
			}
			else
				NewScene(endNode.GetNextScene());
		}
	}

	/// <summary>
	/// 
	/// </summary>
    private void UpdateScene()
	{
		SetIsInputAllowed(false);

        if (m_currentNode is BackgroundNode)
            BackgroundManager.GetInstance().SetBackground();

        else if (m_currentNode is BGMNode)
            AudioManager.GetInstance().SetBGM();

        else if (m_currentNode is CharacterNode || m_currentNode is CharacterInvertNode ||
				 m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
            CharacterManager.GetInstance().EvaluateCharacterNode(m_currentNode);

		

        else if (m_currentNode is DialogueNode)
            DialogueManager.GetInstance().SetDialogue();

        else if (m_currentNode is DialogueBoxNode)
        {
            if ((m_currentNode as DialogueBoxNode).GetToggleSelection() == 0)
                DialogueManager.GetInstance().ToggleDialogueBox(true);
            else
                DialogueManager.GetInstance().ToggleDialogueBox(false);

            NextNode();
        }

        else if (m_currentNode is SFXNode)
            StartCoroutine(AudioManager.GetInstance().PlaySFX());

        else if (m_currentNode is PageNode)
            NewPage((m_currentNode as PageNode).GetPageNumber());

        else if (m_currentNode is EndNode)
            Transition();
    }
}

}
