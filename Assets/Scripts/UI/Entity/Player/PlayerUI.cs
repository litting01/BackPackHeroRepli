using DG.Tweening;
using LogicEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : EntityUI
{
    Text T_StaminaCost;

    protected override void UIInit()
    {
        base.UIInit();

        T_StaminaCost = GetChildUI<Text>("T_StaminaCost");
        T_StaminaCost.text = $"{m_ParrentEntity.m_CurrentStatus.Cost}";

        m_ParrentEntity?.EntityEventHandler
           .AddEvent(LogicEventType.OnE_AttackEvent, new LogicEventBase(StaminaChangeUIEvent, 1));
    }
    public override void UIUpdate(params object[] p_Args)
    {
        base.UIUpdate(p_Args);
        T_StaminaCost.text = $"{m_ParrentEntity.m_CurrentStatus.Cost}";
    }
    protected void StaminaChangeUIEvent(params object[] p_Args)
    {
        GameObject obj = GameObject.Instantiate(UIPrefap);
        obj.transform.SetParent(T_StaminaCost.transform);
        obj.transform.position = T_StaminaCost.transform.position;
        obj.transform.DOMoveY(T_StaminaCost.transform.position.y + 40, 0.5f).OnComplete(() =>
        {
            GameObject.Destroy(obj);
        });

        Text t = obj.GetComponent<Text>();
        if (null != p_Args[0])
        {
            t.text = $"-{(int)p_Args[0]}";
        }

        //t.color = new Color(0, 255 * 0.6f, 0);
    }
}
