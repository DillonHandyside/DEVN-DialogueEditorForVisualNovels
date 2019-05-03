using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DEVN
{

/// <summary>
/// the different variable types available using blackboards
/// </summary>
public enum ValueType
{
    Boolean,
    Float,
    String
}


/// <summary>
/// custom key-value class which allows for different value types,
/// unlike a dictionary
/// </summary>
[System.Serializable]
public class KeyValue
{
    public string m_key; // variable name
    public Value m_value; // variable value
    public ValueType m_valueType; // variable type

    public KeyValue(string key, ValueType valueType)
    {
        m_key = key;
        m_valueType = valueType;

        // intialise default values
        m_value.m_boolean = false;
        m_value.m_float = 0.0f;
        m_value.m_string = "";
    }
}
    

/// <summary>
/// a collection of the different variable types allowed in a blackboard
/// </summary>
[System.Serializable]
public struct Value
{
    public bool m_boolean;
    public float m_float;
    public string m_string;
}


/// <summary>
/// an object which houses a collection of variables of different type.
/// These blackboards acts as a global access point for variables to be used and 
/// modified within the game.
/// Users can create blackboards in the DEVN blackboard editor
/// </summary>
[System.Serializable]
public class Blackboard : ScriptableObject
{
    public string m_blackboardName;
    private List<KeyValue> m_keyValues = new List<KeyValue>();

    /// <summary>
    /// adds a new key-value variable to the blackboard
    /// </summary>
    /// <param name="key">variable name</param>
    /// <param name="valueType">variable type, e.g. Boolean, Float</param>
    public void AddKey(string key, ValueType valueType)
    {
        KeyValue keyValue = new KeyValue(key, valueType);
        m_keyValues.Add(keyValue);
    }

    /// <summary>
    /// sets the value of the variable with the name of the given key
    /// </summary>
    /// <param name="key">the variable to set</param>
    /// <param name="value">the boolean value to set it as</param>
    public void SetValue(string key, bool value)
    {
        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.Boolean);
                keyValue.m_value.m_boolean = value;

                m_keyValues[i] = keyValue; // set
            }
        }
    }

    /// <summary>
    /// sets the value of the variable with the name of the given key
    /// </summary>
    /// <param name="key">the variable to set</param>
    /// <param name="value">the float value to set it as</param>
    public void SetValue(string key, float value)
    {
        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.Float);
                keyValue.m_value.m_float = value;

                m_keyValues[i] = keyValue; // set
            }
        }
    }

    /// <summary>
    /// sets the value of the variable with the name of the given key
    /// </summary>
    /// <param name="key">the variable to set</param>
    /// <param name="value">the string value to set it as</param>
    public void SetValue(string key, string value)
    {
        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.String);
                keyValue.m_value.m_string = value;

                m_keyValues[i] = keyValue; // set
            }
        }
    }

    /// <summary>
    /// return the value struct of a specific key (variable)
    /// </summary>
    /// <param name="key">the name of the variable to find</param>
    /// <returns>the value struct containing variable data</returns>
    public Value GetValue(string key)
    {
        Value value = new Value();

        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
                value = m_keyValues[i].m_value; // get value data
        }

        return value;
    }

    /// <summary>
    /// return the variable type of a specific key (variable)
    /// </summary>
    /// <param name="key">the name of the variable to find</param>
    /// <returns>the variable type, e.g. Boolean, Float etc.</returns>
    public ValueType GetValueType(string key)
    {
        ValueType valueType = ValueType.Boolean;

        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
                valueType = m_keyValues[i].m_valueType; // get value type
        }

        return valueType;
    }

    /// <summary>
    /// deletes the key-value variable with the name of the given key
    /// </summary>
    /// <param name="key">the name of the variable to remove</param>
    public void RemoveKey(string key)
    {
        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_key == key)
            {
                m_keyValues.Remove(m_keyValues[i]); // perform removal
                return; // stop search
            }
        }
    }

    /// <summary>
    /// gets a list of all of the names of each key-value variable in
    /// the blackboard
    /// </summary>
    /// <returns>a collection of key (variable) names</returns>
    public List<string> GetKeys()
    {
        List<string> outputKeys = new List<string>();

        for (int i = 0; i < m_keyValues.Count; i++)
            outputKeys.Add(m_keyValues[i].m_key);

        return outputKeys;
    }

    /// <summary>
    /// gets a list of all of the names of specific key-value variables in 
    /// the blackboard, depending on the given ValueType
    /// </summary>
    /// <param name="valueType">the variable type to search for, e.g. Boolean</param>
    /// <returns>a collection of keys (variables) of specific type</returns>
    public List<string> GetAllOfValueType(ValueType valueType)
    {
        List<string> outputKeys = new List<string>();

        for (int i = 0; i < m_keyValues.Count; i++)
        {
            if (m_keyValues[i].m_valueType == valueType)
                outputKeys.Add(m_keyValues[i].m_key);
        }

        return outputKeys;
    }
}

}