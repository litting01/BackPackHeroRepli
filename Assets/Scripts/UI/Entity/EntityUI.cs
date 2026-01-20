using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using LogicEvent;
using UnityEngine.UI;
using System;

public class EntityUI : MonoBehaviour
{
    protected Entity m_ParrentEntity = null;
    protected RectTransform RT_HPCurrentBG;
    protected Text T_HPVal;
    protected Image I_HPState;
    protected Text T_HPStateVal;

    GameObject m_UIPrefap = null;
    protected GameObject UIPrefap
    {
        get
        {
            if (null == m_UIPrefap)
                m_UIPrefap = (GameObject)ResourceManager.ResourceLoad(ResourceType.Prefaps, "T_DamageUI");
            return m_UIPrefap;
        }
    }

    private void OnEnable()
    {
        UIInit();
    }
    protected virtual void UIInit()
    {
        m_ParrentEntity = GetComponentInParent<Entity>();
         
        RT_HPCurrentBG = GetChildUI<RectTransform>("I_HPCurrentBG");
        T_HPVal = GetChildUI<Text>("T_HPVal");
        T_HPVal.text = $"{m_ParrentEntity.m_CurrentStatus.HP} / {m_ParrentEntity.m_CurrentStatus.HP}";

        
        I_HPState = GetChildUI<Image>("I_HPState");
        T_HPStateVal = GetChildUI<Text>("T_HPStateVal");

        m_ParrentEntity?.EntityEventHandler
            .AddEvent(LogicEventType.OnUIUpdate, new LogicEventBase(UIUpdate, 1));
        m_ParrentEntity?.EntityEventHandler
            .AddEvent(LogicEventType.OnE_HitEvent, new LogicEventBase(HpChangeUIEvent, 1));
        m_ParrentEntity?.EntityEventHandler
           .AddEvent(LogicEventType.OnE_ArmorHitEvent, new LogicEventBase(ArmorChangeUIEvent,1));
    }
    protected virtual void UIReset()
    {
        m_ParrentEntity?.EntityEventHandler
                .RemoveEvent(LogicEventType.OnUIUpdate, UIUpdate);
        m_ParrentEntity?.EntityEventHandler
            .RemoveEvent(LogicEventType.OnE_HitEvent, HpChangeUIEvent);
        m_ParrentEntity?.EntityEventHandler
           .RemoveEvent(LogicEventType.OnE_ArmorHitEvent, ArmorChangeUIEvent);
    }

    public virtual void UIUpdate(params object[] p_Args)
    {
        if(null == m_ParrentEntity)
        {
            UIReset();
            return;
        }
        //HP
        int curhp = m_ParrentEntity.m_CurrentStatus.HP;
        int maxhp = m_ParrentEntity.m_InitStatus.HP;
        float val = curhp / (float)maxhp;
        RT_HPCurrentBG.localScale = new Vector3(Mathf.Max(0, val), 1, 1);
        T_HPVal.text = $"{curhp}/{maxhp}";

        if(m_ParrentEntity.m_CurrentStatus.Armor> 0)
        {
            I_HPState.color = Color.blue;
            T_HPStateVal.text = $"{m_ParrentEntity.m_CurrentStatus.Armor}";
        }
        else
        {
            I_HPState.color = Color.white;
            T_HPStateVal.text = $"";
        }
    }

    /// <summary>
    /// 공격 받을시 HP 감소량 표시하는거
    /// </summary>
    /// <param name="p_Args">감소량</param>
    protected virtual void HpChangeUIEvent(params object[] p_Args)
    {
        if (null == m_ParrentEntity)
            return;
        GameObject obj = GameObject.Instantiate(UIPrefap);
        obj.transform.SetParent(T_HPVal.transform);
        obj.transform.position = T_HPVal.transform.position;
        obj.transform.DOMoveY(T_HPVal.transform.position.y + 40, 0.5f).OnComplete(() =>
        {
            GameObject.Destroy(obj);
        });

        Text t = obj.GetComponent<Text>();
        if (null != p_Args[0])
        {
            t.text = $"{(int)p_Args[0]}";
        }
        t.color = new Color(255 * 0.6f, 0, 0);
    }
    /// <summary>
    /// 공격 받을시 HP 감소량 표시하는거
    /// </summary>
    /// <param name="p_Args">감소량</param>
    protected virtual void ArmorChangeUIEvent(params object[] p_Args)
    {
        if (null == m_ParrentEntity)
            return;
        GameObject obj = GameObject.Instantiate(UIPrefap);
        obj.transform.SetParent(T_HPStateVal.transform);
        obj.transform.position = T_HPStateVal.transform.position;
        obj.transform.DOMoveY(T_HPStateVal.transform.position.y + 40, 0.5f).OnComplete(() =>
        {
            GameObject.Destroy(obj);
        });

        Text t = obj.GetComponent<Text>();
        if (null != p_Args[0])
        {
            t.text = $"{(int)p_Args[0]}";
        }
        t.color = new Color(0, 0, 255 * 0.6f);
    }
    protected T GetChildUI<T>(string name)
    {
        string err = m_ParrentEntity.name + " 에서  UI "+name+" 을 불러오지 못함";
        Transform tran = transform.FindDeepChild(name);
        T data = tran.GetComponent<T>();
        if (null == data)
            Debug.LogWarning(err);
        return data;
        
    }
}
