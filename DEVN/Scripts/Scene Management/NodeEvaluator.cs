using UnityEngine;
using DEVN.Nodes;

namespace DEVN
{

namespace SceneManagement
{

public class NodeEvaluator
{
    private BaseNode m_currentNode;

    // scene manager ref
    private readonly SceneManager m_sceneManager;
    private readonly NodeProcessor m_nodeProcessor;

    public NodeEvaluator(SceneManager sceneManager)
    {
        m_sceneManager = sceneManager;
        m_nodeProcessor = new NodeProcessor(sceneManager);
    }

    public void Evaluate(BaseNode currentNode)
    {
        m_currentNode = currentNode;
        m_nodeProcessor.Update(currentNode);

        // evaluate all the different node types
        if (EvaluateAudioNode())
            return;
        else if (EvaluateBackgroundNode())
            return;
        else if (EvaluateBranchNode())
            return;
        else if (EvaluateCharacterNode())
            return;
        else if (EvaluateDialogueNode())
            return;
        else if (EvaluateUtilityNode())
            return;
        else if (EvaluateVariableNode())
            return;
        EvaluateTransitionNodes();
    }

    #region node evaluation functions

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateAudioNode()
    {
        AudioManager audioManager = m_sceneManager.GetAudioManager();

        if (audioManager != null)
        {
            if (m_currentNode is BGMNode)
            {
                m_nodeProcessor.ProcessBGM();
                return true;
            }
            else if (m_currentNode is SFXNode)
            {
                SFXNode sfxNode = m_currentNode as SFXNode;
                m_sceneManager.StartCoroutine(audioManager.PlaySFX(sfxNode.GetSFX(), true, sfxNode.GetWaitForFinish()));
                return true;
            }
        }
        else
        {
            if (m_currentNode is BGMNode || m_currentNode is SFXNode)
                Debug.LogError("DEVN: SceneManager needs an AudioComponent if you are using audio nodes!");
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateBackgroundNode()
    {
        BackgroundManager backgroundManager = m_sceneManager.GetBackgroundManager();

        if (backgroundManager != null)
        {
            if (m_currentNode is BackgroundNode)
            {
                m_nodeProcessor.ProcessBackground();
                return true;
            }
        }
        else
        {
            if (m_currentNode is BackgroundNode)
                Debug.LogError("DEVN: SceneManager needs a BackgroundComponent if you are using background nodes!");
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateBranchNode()
    {
        BranchManager branchManager = m_sceneManager.GetBranchManager();

        if (branchManager != null)
        {
            if (m_currentNode is BranchNode)
            {
                branchManager.DisplayChoices((m_currentNode as BranchNode).GetBranches(), true);
                return true;
            }
        }
        else
        {
            if (m_currentNode is BranchNode)
                Debug.LogError("DEVN: SceneManager needs a BranchComponent is you are using branch nodes!");
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateCharacterNode()
    {
        CharacterManager characterManager = m_sceneManager.GetCharacterManager();

        if (characterManager != null)
        {
            if (m_currentNode is CharacterNode)
            {
                m_nodeProcessor.ProcessCharacter();
                return true;
            }
            else if (m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
            {
                m_nodeProcessor.ProcessCharacterTransform();
                return true;
            }
        }
        else
        {
            if (m_currentNode is CharacterNode || m_currentNode is CharacterScaleNode || m_currentNode is CharacterTranslateNode)
                Debug.LogError("DEVN: SceneManager needs a CharacterComponent if you are using character nodes!");
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateDialogueNode()
    {
        DialogueManager dialogueManager = m_sceneManager.GetDialogueManager();

        if (dialogueManager != null)
        {
            if (m_currentNode is DialogueNode)
            {
                m_nodeProcessor.ProcessDialogue();
                return true;
            }
            else if (m_currentNode is DialogueBoxNode)
            {
                m_nodeProcessor.ProcessDialogueBox();
                return true;
            }
        }
        else
        {
            if (m_currentNode is BranchNode || m_currentNode is DialogueNode || m_currentNode is DialogueBoxNode)
                Debug.LogError("DEVN: SceneManager needs a DialogueComponent if you are using dialogue nodes!");
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateUtilityNode()
    {
        UtilityManager utilityManager = m_sceneManager.GetUtilityManager();

        if (m_currentNode is DelayNode)
        {
            m_sceneManager.StartCoroutine(utilityManager.Delay((m_currentNode as DelayNode).GetDelayTime(), true));
            return true;
        }
        else if (m_currentNode is ApplicationQuitNode)
            utilityManager.ApplicationQuit();

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool EvaluateVariableNode()
    {
        if (m_currentNode is ConditionNode)
        {
            m_nodeProcessor.ProcessCondition();
            return true;
        }
        else if (m_currentNode is ModifyNode)
        {
            m_nodeProcessor.ProcessModify();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void EvaluateTransitionNodes()
    {
        if (m_currentNode is PageNode)
        {
            PageNode pageNode = m_currentNode as PageNode;
            m_sceneManager.NewPage((pageNode).GetPageNumber());
        }
        else if (m_currentNode is EndNode)
        {
            EndNode endNode = m_currentNode as EndNode;

            if (endNode.GetNextScene() == null)
            {
                // -- place custom scene transition code here -- 
            }
            else
                m_sceneManager.NewScene(endNode.GetNextScene());
        }
    }

    #endregion
}

}

}