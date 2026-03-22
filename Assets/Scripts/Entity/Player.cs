using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections.Generic;
using LogicEvent;
using Buff;

public enum PlayerState
{
    None=0,
    Battle,
}
public class Player : Entity
{
    public string m_InvenGridStr = "111 121 111";

    bool m_IsTurnEnd = false;
    bool m_IsDied = false;

    public PlayerState m_Stat;

    private void Awake()
    {
        Init();
    }

    public override void EntityReset()
    {
        base.EntityReset();
        m_IsDied = false;
    }
    protected override void SelectTarget()
    {
        if (!Input.GetMouseButtonUp(0))
            return;
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.one);
        Enemy en = null;
        foreach (var index in hits)
        {
            en = index.collider.transform.parent.GetComponent<Enemy>();
            if (null != en)
                break;
        }
        if (null == en)
            return;
        m_Target = en;
    }

    public override void EntityReadyTurn()
    {
        m_IsTurnEnd = false;
        m_CurrentStatus.Cost = m_InitStatus.Cost;
    }
    public override bool EntityEndTurn()
    { 
        SelectTarget();
        //타겟이 없어도 아이템을 사용해서 스테미나 소모됨
        UseItem();
        if (m_IsTurnEnd)
            return true;
        return false;
    }
    protected override void Die()
    {
        m_IsDied = true;
    }

    void UseItem()
    {
        if (!Input.GetMouseButtonUp(0))
            return;
        ItemSlot slot = InventoryUI.GetSlot();

        if(null == Target)
        {
            m_Target = BattleManager.GetEnemy(1)[0];
        }
        if(null != slot && slot.IsInItem)
        {
            Item now = slot.Item;
            if (now.IsCanUse())
            {
                int val = now.m_CurrentStatus.Cost;
                EntityEventHandler.GetEvent(LogicEventType.OnE_AttackEvent, "StaminaChangeUIEvent")
                    .SetArgs(val);
                m_CurrentStatus.Cost -= val;
                Attack();
                UseItem(now);
            }

        }
        
    }
   
    public void ClickButtonTurnEnd()
    {
        m_IsTurnEnd = true;
    }

    public bool IsBattleEnd()
    {
        if (m_IsDied)
            return true;

        return false;
    }
}