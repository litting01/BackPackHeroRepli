using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCol : MonoBehaviour,
    IPointerClickHandler,IBeginDragHandler, IDragHandler, IEndDragHandler, ICanvasRaycastFilter
{
    public Item m_ParrentItem;
    public ItemSlot m_ColSlot = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleManager.IsMyTurn(BattleManager.Current.Player))
            return;
        if (!m_ParrentItem.m_IsInstall)
            return;
        if (!m_ParrentItem.IsCanUse())
            return;
        m_ParrentItem.ActiveItem();

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (null != InventoryUI.Current.m_HoldItem)
            return;
        InventoryUI.Current.m_HoldItem = m_ParrentItem;
        if (m_ParrentItem.m_IsInstall)
        {
            InventoryUI.GetSlot(m_ParrentItem.m_SlotIndex).OutPutItem(false);
        }
        m_ParrentItem.BeginMoveEvent();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (null == InventoryUI.Current.m_HoldItem)
            return;
        Vector2 vec = Input.mousePosition;
        m_ParrentItem.transform.position = vec;
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (null == InventoryUI.Current.m_HoldItem)
            return;
        m_ColSlot = InventoryUI.GetSlot();
        if (null != m_ColSlot)
        {
            m_ColSlot.InputItem(m_ParrentItem);
            m_ParrentItem.EventHandler.Invoke(ItemEventType.OnInstallTrigger.ToString());
        }
        else
        {
            m_ParrentItem.ResetInstall(false);
        }
        m_ParrentItem.EndMoveEvent();
        InventoryUI.Current.m_HoldItem = null;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle
            (GetComponent<RectTransform>(), sp, eventCamera, out local))
            return false;
        return true;
    }
}
