using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ResourceType
{
    None=0,
    Items,
    Prefaps,
}
public class ResourceManager 
{
    protected static ResourceManager m_Instance = null;
    static ResourceManager Instance
    {
        get
        {
            if( m_Instance == null)
            {
                m_Instance = new ResourceManager();
            }
            return m_Instance;
        }
    }

    /// <summary>
    /// 타입이름 폴더 안에 오브젝트이름
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="p_type"></param>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static T ResourceLoad<T>(T p_type ,string resourceName)
    {
        return Instance.InResourceFind(p_type, resourceName);
    }
    public static object ResourceLoad(ResourceType p_type ,string resourceName)
    {
        return Instance.InResourceFind(p_type, resourceName);
    }

    protected Dictionary<string, Dictionary<string, object>> m_ResourceDict
        = new Dictionary<string, Dictionary<string, object>>();

    T InResourceFind<T>(T p_type, string p_objName)
    {
        string typeName = p_type.GetType().Name.ToUpper();
        if (!m_ResourceDict.ContainsKey(typeName))
            m_ResourceDict.Add(typeName, new Dictionary<string, object>());

        if (!m_ResourceDict[typeName].ContainsKey(p_objName))
            InResourceLoad(typeName, p_objName);

        T result = (T)m_ResourceDict[typeName][p_objName];
        return result;
    }
    object InResourceFind(ResourceType p_type, string p_objName)
    {
        string typeName = p_type.ToString();
        if (!m_ResourceDict.ContainsKey(typeName))
            m_ResourceDict.Add(typeName, new Dictionary<string, object>());

        if (!m_ResourceDict[typeName].ContainsKey(p_objName))
            InResourceLoad(typeName, p_objName);

        object result = m_ResourceDict[typeName][p_objName];
        return result;
    }
    void InResourceLoad(string p_TypeName, string p_objName)
    {
        string basicPath = p_TypeName + "/" + p_objName;
        var data = Resources.Load(basicPath);
        if (null==data)
        {
            Debug.LogAssertion("리소스 유형이 존재 하지 않음");
            return;
        }
        m_ResourceDict[p_TypeName].Add(p_objName, data);
    }
}