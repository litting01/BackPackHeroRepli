using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;
using Buff;
using System;
using DG.Tweening;

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
        ItemUI ui = GetUI();
        if (null == ui)
            return;
        Vector2Int delta = DownMoveItem(m_SlotIndex);
        if (ui.Data.m_SlotIndex == delta)
            return;
        ItemSlot slot = InventoryUI.GetSlot(delta);
        ui.BeginMoveUI();
        ui.transform.DOMoveY(slot.transform.position.y, 1)
            .SetEase(Ease.InOutElastic).onComplete = () => slot.InputItem(ui);
    }
    private Vector2Int DownMoveItem(Vector2Int p_Vec)
    {
        Vector2Int offset = p_Vec + new Vector2Int(0, 1);
        if (!InventoryUI.IsInInven(offset))
            return p_Vec;
        ItemSlot slot = InventoryUI.GetSlot(offset);
        if (null != slot.Item)
        {
            Type t = slot.Item.Data.GetType();
            if (t == typeof(Item_Bricks))
                return p_Vec;
        }
        return DownMoveItem(offset);
    }
}
