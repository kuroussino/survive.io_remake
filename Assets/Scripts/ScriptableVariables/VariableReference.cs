using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariableReference<T>
{
    [SerializeField] private bool useConst;
    [SerializeField] private T constValue;
    [SerializeField] ScriptableVariable<T> variable;

    public T Value
    {
        get
        {
            return useConst? constValue : variable.Value;
        }
    }
    public static implicit operator T(VariableReference<T> d) => d.Value;
}
