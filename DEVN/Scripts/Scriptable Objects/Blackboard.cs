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

    public KeyValue(KeyValue keyValue)
    {
        m_key = keyValue.m_key;
        m_valueType = keyValue.m_valueType;
        m_value = keyValue.m_value;
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
[CreateAssetMenu(fileName = "New Blackboard", menuName = "DEVN/Blackboard")]
[System.Serializable]
public class Blackboard : ScriptableObject
{
    [HideInInspector]
    [SerializeField] private int m_blackboardID;

    [HideInInspector]
	[SerializeField] private List<KeyValue> m_booleans = new List<KeyValue>();
    [HideInInspector]
	[SerializeField] private List<KeyValue> m_floats = new List<KeyValue>();
    [HideInInspector]
	[SerializeField] private List<KeyValue> m_strings = new List<KeyValue>();

    [HideInInspector]
    [SerializeField] private int m_booleanID = 0;
    [HideInInspector]
    [SerializeField] private int m_floatID = 0;
    [HideInInspector]
    [SerializeField] private int m_stringID = 0;

    #region getters

    public int GetBlackboardID() { return m_blackboardID; }
    public List<KeyValue> GetBooleans() { return m_booleans; }
    public List<KeyValue> GetFloats() { return m_floats; }
    public List<KeyValue> GetStrings() { return m_strings; }
    public int NewBooleanID() { return m_booleanID++; }
    public int NewFloatID() { return m_floatID++; }
    public int NewStringID() { return m_stringID++; }

    #endregion
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="blackboard"></param>
    public void Copy(Blackboard blackboard)
    {
        m_blackboardID = blackboard.GetInstanceID();

        for (int i = 0; i < blackboard.m_booleans.Count; i++)
            m_booleans.Add(new KeyValue(blackboard.m_booleans[i]));

        for (int i = 0; i < blackboard.m_floats.Count; i++)
            m_floats.Add(new KeyValue(blackboard.m_floats[i]));

        for (int i = 0; i < blackboard.m_strings.Count; i++)
            m_strings.Add(new KeyValue(blackboard.m_strings[i]));

        m_booleanID = blackboard.m_booleanID;
        m_floatID = blackboard.m_floatID;
        m_stringID = blackboard.m_stringID;
    }

    /// <summary>
    /// adds a new key-value variable to the blackboard
    /// </summary>
    /// <param name="key">variable name</param>
    /// <param name="valueType">variable type, e.g. Boolean, Float</param>
    public void AddKey(string key, ValueType valueType)
    {
        KeyValue keyValue = new KeyValue(key, valueType);

		switch (valueType)
		{
			case ValueType.Boolean:
				m_booleans.Add(keyValue);
				break;

			case ValueType.Float:
				m_floats.Add(keyValue);
				break;

			case ValueType.String:
				m_strings.Add(keyValue);
				break;
		}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="newKey"></param>
    /// <param name="valueType"></param>
	public void SetKey(string target, string newKey, ValueType valueType)
	{
		if (target != newKey && IsKeyTaken(newKey))
		{
			Debug.LogWarning("DEVN: Attempting to create two variables of the same name!");
			return;
		}

		switch (valueType)
		{
			case ValueType.Boolean:
				for (int i = 0; i < m_booleans.Count; i++)
				{
					if (m_booleans[i].m_key == target)
						m_booleans[i].m_key = newKey;
				}

				break;

			case ValueType.Float:
				for (int i = 0; i < m_floats.Count; i++)
				{
					if (m_floats[i].m_key == target)
						m_floats[i].m_key = newKey;
				}

				break;

			case ValueType.String:
				for (int i = 0; i < m_strings.Count; i++)
				{
					if (m_strings[i].m_key == target)
						m_strings[i].m_key = newKey;
				}

				break;
		}
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
	public bool IsKeyTaken(string key)
	{
		for (int i = 0; i < m_booleans.Count; i++)
		{
			if (m_booleans[i].m_key == key)
				return true;
		}

		for (int i = 0; i < m_floats.Count; i++)
		{
			if (m_floats[i].m_key == key)
				return true;
		}

		for (int i = 0; i < m_strings.Count; i++)
		{
			if (m_strings[i].m_key == key)
				return true;
		}

		return false;
	}

    /// <summary>
    /// sets the value of the variable with the name of the given key
    /// </summary>
    /// <param name="key">the variable to set</param>
    /// <param name="value">the boolean value to set it as</param>
    public void SetValue(string key, bool value)
    {
		for (int i = 0; i < m_booleans.Count; i++)
        {
            if (m_booleans[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.Boolean);
                keyValue.m_value.m_boolean = value;

				m_booleans[i] = keyValue; // set
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
        for (int i = 0; i < m_floats.Count; i++)
        {
            if (m_floats[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.Float);
                keyValue.m_value.m_float = value;

				m_floats[i] = keyValue; // set
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
        for (int i = 0; i < m_strings.Count; i++)
        {
            if (m_strings[i].m_key == key)
            {
                // create new key-value
                KeyValue keyValue = new KeyValue(key, ValueType.String);
                keyValue.m_value.m_string = value;

                m_strings[i] = keyValue; // set
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

        for (int i = 0; i < m_booleans.Count; i++)
        {
			if (m_booleans[i].m_key == key)
				value = m_booleans[i].m_value; // get value data
        }

		for (int i = 0; i < m_floats.Count; i++)
		{
			if (m_floats[i].m_key == key)
				value = m_floats[i].m_value; // get value data
		}

		for (int i = 0; i < m_strings.Count; i++)
		{
			if (m_strings[i].m_key == key)
				value = m_strings[i].m_value; // get value data
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
        ValueType valueType = ValueType.String;

        // is it a boolean?
		for (int i = 0; i < m_booleans.Count; i++)
		{
			if (m_booleans[i].m_key == key)
				valueType = m_booleans[i].m_valueType; // get value data
		}

        // not a boolean, so might be a float
		for (int i = 0; i < m_floats.Count; i++)
		{
			if (m_floats[i].m_key == key)
				valueType = m_floats[i].m_valueType; // get value data
		}

        // not a float, so must be a string
		return valueType;
    }

    /// <summary>
    /// deletes the key-value variable with the name of the given key
    /// </summary>
    /// <param name="key">the name of the variable to remove</param>
    public void RemoveKey(string key, ValueType valueType)
	{
        List<KeyValue> listToSearch = null;

        switch (valueType)
        {
            case ValueType.Boolean:
                listToSearch = m_booleans;
                break;

            case ValueType.Float:
                listToSearch = m_floats;
                break;

            case ValueType.String:
                listToSearch = m_strings;
                break;
        }

        if (listToSearch == null)
            return;

        for (int i = 0; i < listToSearch.Count; i++)
        {
            if (listToSearch[i].m_key == key)
            {
                listToSearch.Remove(listToSearch[i]);
                return;
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

		for (int i = 0; i < m_booleans.Count; i++)
			outputKeys.Add(m_booleans[i].m_key);

		for (int i = 0; i < m_floats.Count; i++)
			outputKeys.Add(m_floats[i].m_key);

		for (int i = 0; i < m_strings.Count; i++)
			outputKeys.Add(m_strings[i].m_key);

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
        List<KeyValue> listToGet = null;

		switch (valueType)
		{
			case ValueType.Boolean:
                listToGet = m_booleans;
				break;

			case ValueType.Float:
                listToGet = m_floats;
				break;

			case ValueType.String:
                listToGet = m_strings;
				break;
		}

        if (listToGet != null)
        {
            for (int i = 0; i < listToGet.Count; i++)
                outputKeys.Add(listToGet[i].m_key);
        }

        return outputKeys;
    }
}

}