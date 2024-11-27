using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_Blackboard
{

    Dictionary<string, object> _blackboardData = new();
    public event Action<string, object> OnBlackboardValueChange;


    public void SetData(string key, object value)
    {
        if (_blackboardData.ContainsKey(key))
            _blackboardData[key] = value;
        else
            _blackboardData.Add(key, value);

        OnBlackboardValueChange?.Invoke(key, value);
    }

    public bool GetData<T>(string key, out T value)
    {
        if (_blackboardData.ContainsKey(key))
        {
            value = (T)_blackboardData[key];
            return true;
        }

        value = default;
        return false;
    }
}
