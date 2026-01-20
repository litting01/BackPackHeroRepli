using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class MyClass
{
    //이거 템플릿으로 만들어서 비교 연산자 넘겨서 확장메소드 만들기
    public static bool Between(this int p_Val, int p_MinVal, int p_MaxVal)
    {
        if (p_MinVal < p_Val && p_MaxVal > p_Val)
            return true;
        return false;
    }
    public static Vector2Int GetStrSizeToVec2Int(this string[] p_str)
    {
        Vector2Int result = Vector2Int.zero;
        result.y = p_str.Length;
        for(int i=0;i< result.y; i++)
        {
            result.x = result.x > p_str[i].Length ? result.x : p_str[i].Length;
        }

        return result;
    }

    public static Transform FindDeepChild(this Transform p_Parent,string p_ChildName)
    {
        for(int i=0;i< p_Parent.childCount; i++)
        {
            Transform now = p_Parent.GetChild(i);
            if(now.name ==  p_ChildName)
                return now;
            Transform result = FindDeepChild(now, p_ChildName);
            if(null != result)
                return result;
        }
        return null;
    }
}
