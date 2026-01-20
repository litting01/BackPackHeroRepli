using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private Item m_Data = null;
    public Item Data { 
        get { return m_Data; } 
        set {
            if(null == value)
                Release();
            else
                Init(value);
        }
    }

    public Image m_image;

    private Transform m_SlotColTran;
    public Transform m_EffectTran;

    public bool m_IsInstall = false;

    private void Update()
    {
        if (!m_IsInstall)
            return;
        //이게 여기있으면 안되는데
        Data.EventHandler.Invoke(ItemEventType.OnInstallUpdate.ToString());
        if (Data.IsCanUse())
        {
            m_image.color = Color.white;
        }
        else
        {
            m_image.color = Color.gray;
        }
    }
    private void Init(Item item)
    {
        m_Data = item;
        name = item.m_Name;
        m_image.sprite = item.m_Sprite;

        Vector2Int vec = Data.GetItemSizeStr.GetStrSizeToVec2Int();
        m_image.transform.localScale = new Vector3(vec.x, vec.y);

        m_SlotColTran = transform.Find("SlotCols");
        CreateSlotCols();
    }

    /// <summary>
    /// 풀에 넣기전 기본값으로 변경
    /// </summary>
    private void Release()
    {
        ResetInstall();
        int n = m_SlotColTran.childCount;
        for(int i=n-1; i>=0; i--)
        {
            GameObject.Destroy(m_SlotColTran.GetChild(i).gameObject);
        }
        m_SlotColTran.localPosition = Vector2.zero;

        m_image.transform.localScale = Vector3.one;
        m_image.transform.localPosition = Vector3.zero;
        m_image.sprite = null;
    }

    private void CreateSlotCols()
    {
        string[] str = Data.GetItemSizeStr;
        Vector2Int size = str.GetStrSizeToVec2Int();
        for(int i=0;i<size.y; i++)
        {
            for(int j = 0; j < size.x; j++)
            {
                char now = str[i][j];
                if (now == '0')
                    continue;
                GameObject clone = new GameObject();
                clone.layer = LayerMask.NameToLayer("UI");
                clone.transform.SetParent(m_SlotColTran, true);
                clone.transform.localPosition = new Vector3(j * 100, -i * 100);
                clone.name = $"Col[ {j}_{i} ]";

                ItemUICol item = clone.AddComponent<ItemUICol>();
                item.m_ParrentUI = this;

                BoxCollider2D col = clone.AddComponent<BoxCollider2D>();
                col.size = new Vector3(99, 99);

                Image img = clone.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0);

            }
        }
        Vector2Int vec = Data.GetItemSizeStr.GetStrSizeToVec2Int();
        m_SlotColTran.localPosition = new Vector3((vec.x - 1) * -50, (vec.y - 1) * 50);

        //짝수 오프셋
        if(size.x%2 == 0)
        {
            m_image.transform.localPosition += new Vector3(50, 0);
            m_SlotColTran.localPosition += new Vector3(50, 0);
        }
        if (size.y % 2 == 0)
        {
            m_image.transform.localPosition += new Vector3(0, -50);
            m_SlotColTran.localPosition += new Vector3(0, -50);
        }
    }
    public void ResetInstall(bool p_RanPos = true)
    {
        transform.SetParent(InventoryUI.Current.transform, false);
        if (p_RanPos)
        {
            transform.DOMove(InventoryUI.RanItemPos, 0.5f).SetEase(Ease.OutBack);
        }
            
        m_image.color = Color.white;

        if (m_IsInstall)
        {
            m_IsInstall = false;
            foreach (var element in GetColSlots())
            {
                ItemSlot slot = InventoryUI.GetSlot(element);
                slot.ResetSlot();
            }
        }
    }

    public void BeginMoveUI()
    {
        m_image.GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.HoldItemUI;
        m_image.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.HoldItemUI - 1;
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
    public void EndMoveUI()
    {
        m_image.GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.ItemUI;
        m_image.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.ItemUIBG;
    }

    public bool IsCanInstall()
    {
        bool result = true;
        if (GetColSlots().Count != m_SlotColTran.childCount)
            return false;
        result = Data.IsCanInstall();
        return result;
    }

    /// <summary>
    /// 아이템 사이즈에 닿은 슬롯들
    /// </summary>
    public List<Vector2Int> GetColSlots()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int itemPos = Data.m_SlotIndex;
        ItemSlot temp = null;
        if(!m_IsInstall)
            temp = InventoryUI.GetSlot();
        if (null != temp)
            itemPos = temp.m_SlotIndex;

        foreach (Vector2Int element in Data.GetItemSizeAboutCenter())
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
        foreach(var element in vec)
        {
            ItemSlot slot = InventoryUI.GetSlot(element);
            if(null == slot)
                continue;
            if (!slot.IsInItem)
                continue;
            if(slot.Item.Data is T)
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
        foreach(string element in p_Dir)
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
        foreach(var element in Data.GetItemSizeAboutCenter())
        {
            Vector2Int vec = Data.m_SlotIndex + element;
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