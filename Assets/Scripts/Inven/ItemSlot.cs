using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Vector2Int m_SlotIndex;
    [SerializeField] private Item m_Item = null;
    public Item Item
    {
        get { return m_Item; }
        set { m_Item = value; }
    }
    public bool IsInItem
    {
        get
        {
            if (null == m_Item)
                return false;
            return true;
        }
    }

    public void ResetSlot()
    {
        OutPutItem();
    }
    /// <summary>
    /// 해당 아이템을 슬롯에 넣음
    /// </summary>
    /// <param name="p_Item"></param>
    public void InputItem(Item p_Item)
    {
        if (!p_Item.IsCanInstall())
        {
            p_Item.ResetInstall();
            return;
        }
        if (IsInItem)
            OutPutItem();
        p_Item.BeginMoveEvent();
        Item = p_Item;
        Item.transform.position = transform.position;
        Item.transform.SetParent(transform);

        Item.m_IsInstall = true;
        Item.m_SlotIndex = m_SlotIndex;
        ChildInputItem();
        p_Item.EndMoveEvent();
    }
    
    /// <summary>
    /// 현재 슬롯안에 있는 아이템 밖으로 빼내기
    /// </summary>
    /// <param name="p_RanPos"></param>
    public void OutPutItem(bool p_RanPos = true)
    {
        if (null == m_Item)
            return;
        if (m_Item.m_IsInstall)
        {
            Item.EventHandler.Invoke(ItemEventType.OnUnInstallTrigger.ToString());
            Item.ResetInstall(p_RanPos);
        }
        Item = null;
    }
    /// <summary>
    /// 아이템의 사이즈에 닿은 슬롯에 이벤트
    /// </summary>
    void ChildInputItem()
    {
        List<Vector2Int> list = Item.GetColSlots();
        list = list.Where((e) => e != m_SlotIndex).ToList();
        foreach (var element in list)
        {
            ItemSlot slot = InventoryUI.GetSlot(element);
            if (slot.IsInItem)
                slot.Item.ResetInstall();
            slot.m_Item = Item;
        }
    }

    void SlotHighLight()
    {
        //이것처럼 선 그려서 하이라이트 표시
        Debug.DrawLine(transform.position,Vector3.right,Color.white);
    }
}
/// <summary>
/// 앞에 9는 의미 없음
/// 뒤에 두자리 x , y ( 0이 -1)
/// </summary>
public enum SlotDirction
{
    None = 911,
    N = 910,
    NE = 920,
    NW = 900,
    S = 912,
    SE = 922,
    SW = 902,
    E = 921,
    W = 901,
}