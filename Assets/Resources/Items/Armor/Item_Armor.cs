using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;
using System;
using System.Linq;
using Unity.VisualScripting;

public class Item_Armor : Item, I_ItemEffect
{
    public int ArmorCount = 10;
    private List<Vector2Int> m_NearSlot = new List<Vector2Int>();
    
    public override void InitItem(EntityEditorInvenItem p_Data, Entity p_Owner)
    {
        base.InitItem(p_Data, p_Owner);
        EventHandler.AddEvent(ItemEventType.OnInstallUpdate.ToString(), new LogicEventBase(OnInstallUpdateEvent, 0));
    }
    public override bool ActiveItem()
    {
        return true;
    }
    public override void ReleaseItem()
    {
        base.ReleaseItem();

        Debug.Log("아이템 제거");

        m_NearSlot.Clear();
    }

    private void OnInstallUpdateEvent(params object[] args)
    {
        m_NearSlot = GetColSlots<Item_Shield>("N", "S", "W", "E");
            
        foreach(Vector2Int element in m_NearSlot)
        {
            ItemSlot slot = InventoryUI.GetSlot(element);
            ItemEffect[] com = slot.Item.m_EffectTran.GetComponents<ItemEffect>();
            ItemEffect temp = Array.Find(com, (e) =>
            {
                if(e.m_EffectName == nameof(Item_Armor))
                    return true;
                return false;
            });
            if (null == temp)
            {
                temp = slot.Item.AddItemEffect(InEffect, OutEffect);
                temp.m_EffectName = nameof(Item_Armor);
            }
        }
    }
    public void InEffect(params object[] args)
    {
        if (args.Length == 0)
            return;
        Item item = (Item)args[0];
        if (null == item)
            return;
        item.m_CurrentStatus.Damage += ArmorCount;
    }
    
    public void OutEffect(params object[] args)
    {
        if (args.Length == 0)
            return;
        Item item = (Item)args[0]; //
        if (null == item)
            return;

        //상대 아이템의 주변에 현재 아이템을 찾고

        Debug.Log("Out");
        //없으면 이펙트 삭제
        item.m_CurrentStatus.Damage -= ArmorCount;
        if (true) 
        {
            ItemEffect effect = (ItemEffect)args[1];
            effect.Release();
        }
    }
    /// <summary>
    /// 아머 주변에 있는 아이템인지
    /// </summary>
    /// <param name="p_Item">주변 아이템</param>
    /// <returns></returns>
    private void IsNearShield(Item p_Item)
    {
        
    }
}