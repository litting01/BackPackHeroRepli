using System;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;
using DG.Tweening;
using static UnityEditor.Progress;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private Transform m_SlotColTran;
    public Transform m_EffectTran;

    public bool m_IsInstall = false;

    public ItemStatus m_InitStatus;
    public ItemStatus m_CurrentStatus;
    public Sprite m_Sprite;
    protected Entity m_Owner;

    public Vector2Int m_SlotIndex;

    private ItemUI m_UI = null;
    protected ItemUI GetUI
    {
        get {
            if(null == m_UI)
                m_UI = GetComponentInChildren<ItemUI>();
            return m_UI;
        }

    }

    public string[] GetItemSizeStr
    {
        get
        {
            if (null == m_CurrentStatus.ItemSize)
                return null;
            return m_CurrentStatus.ItemSize.Split(" ");
        }
    }
    Vector2Int m_ItemCenterIndex = Vector2Int.zero;
    public Vector2Int ItemCenterIndex { get { return m_ItemCenterIndex; } }

    private Vector2Int m_ItemSize = new Vector2Int(-1, -1);
    public Vector2Int ItemSize
    {
        get
        {
            if(m_ItemSize.x == -1 || m_ItemSize.y == -1)
                m_ItemSize = GetItemSizeStr.GetStrSizeToVec2Int();
            return m_ItemSize;
        }
    }

    private LogicEventHandler<string> m_EventHandler = new LogicEventHandler<string>();
    public  LogicEventHandler<string> EventHandler {  get { return m_EventHandler; } }

    private void Update()
    {
        if (!m_IsInstall)
            return;
        EventHandler.Invoke(ItemEventType.OnInstallUpdate.ToString());
    }

    /// <summary>
    /// Player 전용
    /// </summary>
    /// <param name="p_Data"></param>
    /// <param name="p_Owner"></param>
    public virtual void InitItem(EntityEditorInvenItem p_Data, Entity p_Owner) {
        m_InitStatus = p_Data.Data.m_Data;
        m_CurrentStatus = m_InitStatus;
        m_Sprite = p_Data.Data.m_Image;
        m_SlotIndex = p_Data.SlotIndex;
        name = p_Data.Data.name;
        m_Owner = p_Owner;

        Vector2Int size = GetItemSizeStr.GetStrSizeToVec2Int();
        m_ItemCenterIndex.x = (int)MathF.Max((size.x - 1) / 2.0f, 0);
        m_ItemCenterIndex.y = (int)MathF.Max((size.y - 1) / 2.0f, 0);

        m_SlotColTran = transform.Find("SlotCols");
        m_EffectTran = transform.Find("EffectTran");
        CreateSlotCols();

        GetUI.Init(this);
    }

    /// <summary>
    /// Enemy 전용 임시
    /// </summary>
    /// <param name="p_Data"></param>
    /// <param name="p_Owner"></param>
    public virtual void InitItem(ItemScriptObj p_Data,Entity p_Owner)
    {
        m_InitStatus = p_Data.m_Data;
        m_CurrentStatus = m_InitStatus;
        m_Sprite = p_Data.m_Image;
        name = p_Data.name;
        m_Owner = p_Owner;
        m_SlotColTran = transform.Find("SlotCols");
        m_EffectTran = transform.Find("EffectTran");
    }

    private void CreateSlotCols()
    {
        string[] str = GetItemSizeStr;
        Vector2Int size = str.GetStrSizeToVec2Int();
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                char now = str[i][j];
                if (now == '0')
                    continue;
                GameObject clone = new GameObject();
                clone.layer = LayerMask.NameToLayer("UI");
                clone.transform.SetParent(m_SlotColTran, true);
                clone.transform.localPosition = new Vector3(j * 100, -i * 100);
                clone.name = $"Col[ {j}_{i} ]";

                ItemCol item = clone.AddComponent<ItemCol>();
                item.m_ParrentItem = this;

                BoxCollider2D col = clone.AddComponent<BoxCollider2D>();
                col.size = new Vector3(99, 99);

            }
        }
        Vector2Int vec = GetItemSizeStr.GetStrSizeToVec2Int();
        m_SlotColTran.localPosition = new Vector3((vec.x - 1) * -50, (vec.y - 1) * 50);
        //짝수 오프셋
        if (size.x % 2 == 0)
        {
            m_SlotColTran.localPosition += new Vector3(50, 0);
        }
        if (size.y % 2 == 0)
        {
            m_SlotColTran.localPosition += new Vector3(0, -50);
        }

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
        if (GetColSlotIndex().Count != m_SlotColTran.childCount)
            result = false;
        //아이템 설치 조건변경 이벤트
        return result;
    }

    /// <summary>
    /// 풀에 넣기전 기본값으로 변경
    /// </summary>
    public virtual void ReleaseItem()
    {
        ResetInstall(false);
        int n = m_SlotColTran.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject.Destroy(m_SlotColTran.GetChild(i).gameObject);
        }
        m_SlotColTran.localPosition = Vector2.zero;
        GetUI.Release();
    }
    /// <summary>
    /// 인벤에 배치 해제
    /// </summary>
    /// <param name="p_RanPos">랜덤 위치로 이동 true</param>
    public void ResetInstall(bool p_RanPos = true)
    {
        transform.SetParent(InventoryUI.Current.transform, false);
        if (p_RanPos)
        {
            transform.DOMove(InventoryUI.RanItemPos, 0.5f).SetEase(Ease.OutBack);
        }
        GetUI.ResetUI();
        if (m_IsInstall)
        {
            List<Vector2Int> alpha = GetColSlots();
            foreach (var element in alpha)
            {
                ItemSlot slot = InventoryUI.GetSlot(element);
                slot.Item = null;
            }
            m_IsInstall = false;
        }
    }
    //------------------------------------------------------------------------
    /// <summary>
    /// 아이템 인벤내에서 이동시
    /// </summary>
    public void BeginMoveEvent()
    {
        GetUI.BeginMoveUI();
        transform.SetParent(InventoryUI.Current.m_ItemTran);
        if (m_IsInstall)
        {
            m_IsInstall = false;
            List<Vector2Int> list = GetColSlots();
            foreach (var element in list)
            {
                ItemSlot slot = InventoryUI.GetSlot(element);
                slot.Item = null;
            }
        }
    }
    /// <summary>
    /// 아이템이 인벤내에서 이동이 끝날때
    /// </summary>
    public void EndMoveEvent()
    {
        GetUI.EndMoveUI();
    }
    /// <summary>
    /// 중심으로부터 떨어져있는 슬롯의 위치들(빈공간은 반환하지않음)
    /// </summary>
    public List<Vector2Int> GetColIndexeAboutCenter()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        string[] str = GetItemSizeStr;
        for (int i = 0; i < str.Length; i++)
        {
            for (int j = 0; j < str[i].Length; j++)
            {
                if (str[i][j] == '0')
                    continue;
                result.Add(new Vector2Int(j - ItemCenterIndex.x, i - ItemCenterIndex.y));
            }
        }
        return result;
    }

    /// <summary>
    /// 아이템 사이즈에 닿은 슬롯들
    /// </summary>
    public List<Vector2Int> GetColSlots()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int itemPos = m_SlotIndex;
        ItemSlot temp = null;
        if (!m_IsInstall)
            temp = InventoryUI.GetSlot();
        if (null != temp)
            itemPos = temp.m_SlotIndex;

        foreach (Vector2Int element in GetColIndexeAboutCenter())
        {
            Vector2Int now = element + itemPos;
            if (!InventoryUI.IsInInven(now))
                continue;
            if (list.Contains(now))
                continue;
            list.Add(now);
        }
        return list;
    }
    /// <summary>
    /// 아이템 사이즈에 닿은 슬롯들
    /// 설치된 상태가 아니면 마우스 기준슬롯으로 정함
    /// </summary>
    public List<Vector2Int> GetColSlotIndex()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int itemPos = m_SlotIndex;
        ItemSlot temp = null;
        if (!m_IsInstall)
            temp = InventoryUI.GetSlot();
        if (null != temp)
            itemPos = temp.m_SlotIndex;

        foreach (Vector2Int element in GetColIndexeAboutCenter())
        {
            Vector2Int now = element + itemPos;
            if (!InventoryUI.IsInInven(now))
                continue;
            if (list.Contains(now))
                continue;
            list.Add(now);
        }
        return list;
    }
    public List<Vector2Int> GetColSlots<T>(params string[] p_Dir) where T : Item
    {
        List<Vector2Int> vec = GetNearSlot(p_Dir);
        if (vec == null)
            return null;

        List<Vector2Int> templist = new List<Vector2Int>();
        foreach (var element in vec)
        {
            ItemSlot slot = InventoryUI.GetSlot(element);
            if (null == slot)
                continue;
            if (!slot.IsInItem)
                continue;
            if (slot.Item is T)
                templist.Add(element);
        }
        return templist;
    }
    /// <summary>
    /// 아이템의 각 슬롯의 1칸 방향에 있는 슬롯 인덱스 반환 
    /// </summary>
    public List<Vector2Int> GetNearSlot(params string[] p_Dir)
    {
        List<SlotDirction> list = new List<SlotDirction>();
        foreach (string element in p_Dir)
        {
            SlotDirction now = SlotDirction.None;
            switch (element)
            {
                case "N":
                    now = SlotDirction.N;
                    break;
                case "NE":
                    now = SlotDirction.NE;
                    break;
                case "NW":
                    now = SlotDirction.NW;
                    break;
                case "S":
                    now = SlotDirction.S;
                    break;
                case "SE":
                    now = SlotDirction.SE;
                    break;
                case "SW":
                    now = SlotDirction.SW;
                    break;
                case "E":
                    now = SlotDirction.E;
                    break;
                case "W":
                    now = SlotDirction.W;
                    break;
                default:
                    continue;
            }
            if (list.Contains(now))
                continue;
            list.Add(now);
        }
        return GetNearSlot(list);
    }
    /// <summary>
    /// 아이템의 각 슬롯의 1칸 방향에 있는 슬롯 인덱스 반환
    /// </summary>
    public List<Vector2Int> GetNearSlot(List<SlotDirction> p_Dir)
    {
        List<Vector2Int> dataList = new List<Vector2Int>();
        foreach (var element in GetColIndexeAboutCenter())
        {
            Vector2Int vec = m_SlotIndex + element;
            foreach (SlotDirction dir in p_Dir)
            {
                int temp = (int)dir - 900;
                Vector2Int offset = vec + new Vector2Int(temp / 10 - 1, temp % 10 - 1);
                if (!InventoryUI.IsInInven(offset))
                    continue;
                if (dataList.Contains(offset))
                    continue;
                if (offset == vec)
                    continue;
                dataList.Add(offset);

            }
        }
        return dataList;
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
    OnInstallUpdate,
}