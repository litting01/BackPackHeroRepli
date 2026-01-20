using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUICol : MonoBehaviour, ICanvasRaycastFilter,
    IPointerClickHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemUI m_ParrentUI;
    public ItemSlot m_ColSlot = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleManager.IsMyTurn(BattleManager.Current.Player))
            return;
        if (!m_ParrentUI.m_IsInstall)
            return;
        if (!m_ParrentUI.Data.IsCanUse())
            return;
        m_ParrentUI.Data.ActiveItem();

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (null != InventoryUI.Current.m_HoldItem)
            return;
        InventoryUI.Current.m_HoldItem = m_ParrentUI;
        if (m_ParrentUI.m_IsInstall)
        {
            InventoryUI.GetSlot(m_ParrentUI.Data.m_SlotIndex).OutPutItem(false);
        }


        m_ParrentUI.BeginMoveUI();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (null == InventoryUI.Current.m_HoldItem)
            return;
        Vector2 vec = Input.mousePosition;
        m_ParrentUI.transform.position = vec;
        
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (null == InventoryUI.Current.m_HoldItem)
            return;
        m_ColSlot = InventoryUI.GetSlot();
        if (null != m_ColSlot)
        {
            m_ColSlot.InputItem(m_ParrentUI);
            m_ParrentUI.Data.EventHandler.Invoke(ItemEventType.OnInstallTrigger.ToString());
        }
        else
        {
            m_ParrentUI.ResetInstall(false);
        }
        m_ParrentUI.EndMoveUI();
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
