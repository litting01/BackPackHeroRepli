using LogicEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item_Shield : Item
{
    //public int ArmorCount = 7;

    public override void InitItem(EntityEditorInvenItem p_Data, Entity p_Owner)
    {
        base.InitItem(p_Data, p_Owner);
        //EventHandler.AddEvent(ItemEventType.OnInstallUpdate.ToString(), new LogicEvent.LogicEventBase(TempFunc, 0));
    }
    public void TempFunc(params object[] args)
    {
        //Debug.Log(m_CurrentStatus.Damage);
    }

    public override bool ActiveItem()
    {
        m_Owner.m_CurrentStatus.Armor += m_CurrentStatus.Damage;
        m_Owner.EntityEventHandler.GetEvent(LogicEventType.OnE_ArmorHitEvent, "ArmorChangeUIEvent")
                    .SetArgs(m_CurrentStatus.Damage);
        m_Owner.EntityEventHandler.Invoke(LogicEventType.OnE_ArmorHitEvent);
        return true;
    }
    
}