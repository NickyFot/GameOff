using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TypeDictionary<TValue> // where TValue : new()
{
    private Dictionary<Type, TValue> m_TypeDict;

    public TypeDictionary()
    {
        m_TypeDict = new Dictionary<Type, TValue>();
    }

    public void Add<TKey>()
        where TKey : TValue, new()
    {
        Add(new TKey());
    }

    public void Add(TValue t)
    {
        Type type = t.GetType();
        if(!m_TypeDict.ContainsKey(type))
        {
            m_TypeDict.Add(type, t);
        }
        else
        {
            throw new ArgumentException("An element with the same type already exists in the dictionary.");
        }
    }

    public void Remove(TValue t)
    {
        Type type = t.GetType();
        if(m_TypeDict.ContainsKey(type))
        {
            m_TypeDict.Remove(type);
        }
        else
        {
            throw new ArgumentException("Element \"" + type.ToString() + "\" was not found in the dictionary.");
        }
    }

    public TValue this[Type type]
    {
        get
        {
            return Get(type);
        }
    }

    public TValue Get<TKey>()
        where TKey : TValue
    {
        Type type = typeof(TKey);
        return Get(type);
    }

    public TValue Get(Type type)
    {
        TValue t;
        if(m_TypeDict.TryGetValue(type, out t))
        {
            return t;
        }
        else
        {
            throw new KeyNotFoundException("Type \"" + type.Name + "\" was not found in the dictionary.");
        }
    }

    public bool Contains<TKey>()
        where TKey : TValue
    {
        Type type = typeof(TKey);
        return Contains(type);
    }

    public bool Contains(Type type)
    {
        return m_TypeDict.ContainsKey(type);
    }
}