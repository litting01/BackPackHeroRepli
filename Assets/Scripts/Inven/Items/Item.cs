using System;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;

public abstract class Item
{
    public string m_Name;

    protected string[] m_EffectStr;
    public string[] EffectStr { get { return m_EffectStr; } }

    public ItemStatus m_InitStatus;
    public ItemStatus m_CurrentStatus;
    public Sprite m_Sprite;
    protected Entity m_Owner;

    public Vector2Int m_SlotIndex;

    private LogicEventHandler<string> m_EventHandler = new LogicEventHandler<string>();
    public  LogicEventHandler<string> EventHandler {  get { return m_EventHandler; } }

    public virtual void InitItem(EntityEditorInvenItem p_Data, Entity p_Owner) {
        m_InitStatus = p_Data.Data.m_Data;
        m_CurrentStatus = m_InitStatus;
        m_Sprite = p_Data.Data.m_Image;
        m_SlotIndex = p_Data.SlotIndex;
        m_Name = p_Data.Data.name;
        m_Owner = p_Owner;

        Vector2Int size = GetItemSizeStr.GetStrSizeToVec2Int();
        m_ItemSizeCenter.x = (int)MathF.Max((size.x - 1) / 2.0f, 0);
        m_ItemSizeCenter.y = (int)MathF.Max((size.y - 1) / 2.0f, 0);
    }
    public virtual bool ActiveItem()
    {
        //CurrentStatus에 버프 바르고 넘기기
        bool result = true;
        

        return result;
    }
    public virtual bool IsCanUse()
    {
        if(m_Owner.m_CurrentStatus.Cost < m_CurrentStatus.Cost) 
            return false;
        if (null == BattleManager.Current && BattleManager.IsMyTurn(m_Owner))
            return false;
        return true;
    }
    public virtual bool IsCanInstall()
    {
        bool result = true;
        //아이템 설치 조건변경 이벤트
        
        return result;
    }
    public virtual void ReleaseItem() { }
    
    public string[] GetItemSizeStr
    {
        get
        {
            if (null == m_CurrentStatus.ItemSize)
                return null;
            return m_CurrentStatus.ItemSize.Split(" ");
        }
    }
    Vector2Int m_ItemSizeCenter = Vector2Int.zero;
    public Vector2Int ItemSizeCenter { get {  return m_ItemSizeCenter; } }

    /// <summary>
    /// 중심으로부터 떨어져있는 슬롯의 위치들(빈공간은 반환하지않음)
    /// </summary>
    public List<Vector2Int> GetItemSizeAboutCenter()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        string[] str = GetItemSizeStr;
        for (int i = 0; i < str.Length; i++)
        {
            for (int j = 0; j < str[i].Length; j++)
            {
                if (str[i][j] == '0')
                    continue;
                result.Add(new Vector2Int(j - ItemSizeCenter.x, i - ItemSizeCenter.y));
            }
        }
        return result;
    }
    public ItemUI GetUI()
    {
        //아이템이 설치된 상태가 아니면 슬롯이 null일수도 있음
        ItemSlot now = InventoryUI.GetSlot(m_SlotIndex);
        if(null == now)
            return null;
        return now.Item;
    }
}

[Serializable]
public struct ItemStatus
{
    public int Damage;
    public int Cost;
    public int Price;
    public string ItemSize;
}

public enum ItemEventType
{
    None,
    OnInstallTrigger,
    OnUnInstallTrigger,
    OnInstallUpdate
}