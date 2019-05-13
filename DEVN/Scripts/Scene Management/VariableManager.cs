using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DEVN;


public class VariableManager
{
    // singleton
    private static VariableManager m_instance;

    private ConditionNode m_currentConditionNode;

    #region getters

    public static VariableManager GetInstance()
    {
        if (m_instance == null)
            m_instance = new VariableManager();
        return m_instance;
    }

    #endregion
	
    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    public void PerformModify(BaseNode node)
    {
        ModifyNode modifyNode = node as ModifyNode;

        string key = modifyNode.GetKey();
        Blackboard blackboard = FindBlackboard(modifyNode.GetBlackboard());
        Value value = blackboard.GetValue(key);

        switch (modifyNode.GetValueType())
        {
            case ValueType.Boolean:
                if (modifyNode.GetBooleanSelection() == 0) // set
                    blackboard.SetValue(key, modifyNode.GetBooleanValue());
                else // toggle
                    blackboard.SetValue(key, !value.m_boolean);

                break;

            case ValueType.Float:
                if (modifyNode.GetFloatSelection() == 0) // set
                    blackboard.SetValue(key, modifyNode.GetFloatValue());
                else // increment
                    blackboard.SetValue(key, value.m_float + modifyNode.GetFloatValue());

                break;

            case ValueType.String:
                blackboard.SetValue(key, modifyNode.GetStringValue());

                break;
        }

        SceneManager.GetInstance().NextNode();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public int EvaluateCondition(BaseNode node)
    {
        m_currentConditionNode = node as ConditionNode;

        string keyA = m_currentConditionNode.GetKeyA();
        Blackboard blackboardA = m_currentConditionNode.GetBlackboardA();
        Value valueA = FindBlackboard(blackboardA).GetValue(keyA);

        if (m_currentConditionNode.GetSourceSelection() == 0)
            return BlackboardAndValue(valueA);
        else
            return BlackboardAndBlackboard(valueA);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <returns></returns>
    private int BlackboardAndValue(Value valueA)
    {
        switch (m_currentConditionNode.GetValueType())
        {
            case ValueType.Boolean:
                bool booleanValue = m_currentConditionNode.GetBooleanValue();
                if (valueA.m_boolean == booleanValue)
                    return 1; // bool is equal to value
                break;

            case ValueType.Float:
                float floatValue = m_currentConditionNode.GetFloatValue();
                switch (m_currentConditionNode.GetFloatSelection())
                {
                    case 0:
                        if (valueA.m_float < floatValue)
                            return 1; // float is less than value
                        break;
                    case 1:
                        if (valueA.m_float == floatValue)
                            return 1; // float is equal to value
                        break;
                    case 2:
                        if (valueA.m_float > floatValue)
                            return 1; // float is greater than value
                        break;
                }
                        
                break;

            case ValueType.String:
                string stringValue = m_currentConditionNode.GetStringValue();
                if (valueA.m_string == stringValue)
                    return 1; // string is equal to value
                break;
        }

        return 0; // condition is false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueA"></param>
    /// <returns></returns>
    private int BlackboardAndBlackboard(Value valueA)
    {
        string keyB = m_currentConditionNode.GetKeyB();
        Blackboard blackboardB = m_currentConditionNode.GetBlackboardB();
        Value valueB = FindBlackboard(blackboardB).GetValue(keyB);

        switch (m_currentConditionNode.GetValueType())
        {
            case ValueType.Boolean:
                if (valueA.m_boolean == valueB.m_boolean)
                    return 1; // bool is equal to value

                break;

            case ValueType.Float:
                switch (m_currentConditionNode.GetFloatSelection())
                {
                    case 0:
                        if (valueA.m_float < valueB.m_float)
                            return 1; // float is less than value
                        break;
                    case 1:
                        if (valueA.m_float == valueB.m_float)
                            return 1; // float is equal to value
                        break;
                    case 2:
                        if (valueA.m_float > valueB.m_float)
                            return 1; // float is greater than value
                        break;
                }

                break;

            case ValueType.String:
                if (valueA.m_string == valueB.m_string)
                    return 1; // string is equal to value

                break;
        }

        return 0; // condition is false
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    /// <returns></returns>
    private Blackboard FindBlackboard(Blackboard blackboard)
    {
        SceneManager sceneManager = SceneManager.GetInstance();
        List<Blackboard> blackboards = sceneManager.GetBlackboards();

        for (int i = 0; i < blackboards.Count; i++)
        {
            if (blackboard.GetInstanceID() == blackboards[i].GetBlackboardID())
                return blackboards[i]; // blackboard found
        }

        return null; // blackboard not found
    }
}
