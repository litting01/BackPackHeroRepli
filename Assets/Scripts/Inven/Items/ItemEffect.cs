using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using LogicEvent;

using static ItemEffect;

public class ItemEffect : MonoBehaviour
{
    private Item m_Item = null;
    public Item Item { get { return m_Item; } }
    public string m_EffectName;

    LogicEventDel OnReleaseEvent;

    public void Init(Item p_item, LogicEventDel p_OnInitEvent, LogicEventDel p_OnReleaseEvent)
    {
        if (null == p_item)
            return;
        m_Item = p_item;

        p_OnInitEvent(m_Item);
        OnReleaseEvent = p_OnReleaseEvent;
    }
    public void Release()
    {
        OnReleaseEvent.Invoke(m_Item);
        GameObject.Destroy(this);
    }
}
public interface I_ItemEffect
{
    public void InEffect(params object[] args);
    public void OutEffect(params object[] args);
}

public static class ItemEffectExtensions
{
    public static ItemEffect AddItemEffect(this Item p_item, LogicEventDel OnInitEvent, LogicEventDel p_OnReleaseEvent)
    {
        if (null == p_item)
            return null;
        ItemEffect effect = null;
        effect = p_item.GetUI().m_EffectTran.AddComponent<ItemEffect>();
        effect.Init(p_item,OnInitEvent, p_OnReleaseEvent);
        return effect;
    }
} 