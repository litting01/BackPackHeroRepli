using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 정보 표시하는 것만 넣기
/// </summary>
public class ItemUI : MonoBehaviour
{

    private Image m_image;
    private Item m_Item;

    private void Update()
    {
        if (m_Item.IsCanUse())
        {
            m_image.color = Color.white;
        }
        else
        {
            m_image.color = Color.gray;
        }
    }
    public void Init(Item p_Item)
    {
        m_Item = p_Item;
        m_image = GetComponent<Image>();
        m_image.sprite = p_Item.m_Sprite;
        m_image.transform.localScale = new Vector3(p_Item.ItemSize.x, p_Item.ItemSize.y);
        //짝수 오프셋
        Vector2Int size = p_Item.ItemSize;
        if (size.x % 2 == 0)
        {
            m_image.transform.localPosition += new Vector3(50, 0);
        }
        if (size.y % 2 == 0)
        {
            m_image.transform.localPosition += new Vector3(0, -50);
        }
    }

    /// <summary>
    /// 풀에 넣기전 기본값으로 변경
    /// </summary>
    public void Release()
    {
        ResetUI();
        m_image.transform.localScale = Vector3.one;
        m_image.transform.localPosition = Vector3.zero;
        m_image.sprite = null;
        m_Item = null;
    }

    /// <summary>
    /// Item정보는 바꾸지않고 리셋
    /// </summary>
    /// <param name="p_RanPos"></param>
    public void ResetUI(bool p_RanPos = true)
    {
        m_image.color = Color.white;
    }

    /// <summary>
    /// 아이템 인벤내에서 이동시
    /// </summary>
    public void BeginMoveUI()
    {
        m_image.GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.HoldItemUI;
        m_image.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.HoldItemUI - 1;
    }
    /// <summary>
    /// 아이템이 인벤내에서 이동이 끝날때
    /// </summary>
    public void EndMoveUI()
    {
        m_image.GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.ItemUI;
        m_image.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = (int)UISortingIndex.ItemUIBG;
    }

    /*public List<Vector2Int> GetColSlots<T>(params string[] p_Dir) where T : Item
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
    }*/
}