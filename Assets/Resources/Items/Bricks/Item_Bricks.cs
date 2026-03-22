using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;
using Buff;
using System;
using DG.Tweening;
using Unity.VisualScripting;

public class Item_Bricks : Item
{
    public override void InitItem(EntityEditorInvenItem p_Data, Entity p_Owner)
    {
        m_InitStatus.ItemSize = "1";
        base.InitItem(p_Data, p_Owner);

        EventHandler.AddEvent(ItemEventType.OnInstallTrigger.ToString(), new LogicEventBase(OnInstallEvent, 0));
    }
    public override bool ActiveItem()
    {
        base.ActiveItem();
        /*m_Owner.Target.AddBuff(new BuffInfo()
        {
            BuffName = "Buff_Fire",
            StackCount = 5
        });*/
        return true;
    }
    
    private void OnInstallEvent(params object[] args)
    {
        Vector2Int delta = DownMoveItem(m_SlotIndex);
        if (m_SlotIndex == delta)
        {
            if (!m_IsInstall)
            {

            }
            return;
        }
        ItemSlot slot = InventoryUI.GetSlot(delta);
        BeginMoveEvent();
        transform.DOMoveY(slot.transform.position.y, 1)
            .SetEase(Ease.InOutElastic).onComplete = () => slot.InputItem(this);
        EndMoveEvent();
    }
    private Vector2Int DownMoveItem(Vector2Int p_Vec)
    {
        Vector2Int offset = p_Vec + new Vector2Int(0, 1);
        if (!InventoryUI.IsInInven(offset))
            return p_Vec;
        ItemSlot slot = InventoryUI.GetSlot(offset);
        if (slot.IsInItem)
        {
            Type t = slot.Item.GetType();
            if (slot.Item != this && t == typeof(Item_Bricks))
                return p_Vec;
        }
        return DownMoveItem(offset);
    }
}
