using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableVariable<T> : ScriptableObject
{
    [SerializeField] private T value;
    public T Value { get => value; }
    public static implicit operator T(ScriptableVariable<T> d) => d.Value;
}