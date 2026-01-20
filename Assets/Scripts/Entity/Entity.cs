using System;
using System.Collections;
using System.Collections.Generic;
using Buff;
using LogicEvent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] public EntityStatus m_CurrentStatus;
    public EntityStatus m_InitStatus;

    public List<EntityEditorInvenItem> m_EditorInven;

    //private List<Item> m_Inven = new List<Item>();
    public List<EntityEditorInvenItem> Inven { get { return m_EditorInven; } }

    protected Entity m_Target = null;
    public Entity Target { get { return m_Target; } } 
    public Transform m_BuffTran;
    
    private LogicEventHandler<LogicEventType> m_EventHandler = null;
    public LogicEventHandler<LogicEventType> EntityEventHandler { get
        {
            if(null == m_EventHandler)
                m_EventHandler = new LogicEventHandler<LogicEventType>();
            return m_EventHandler;
        } 
    }

    private void Update()
    {
        EntityEventHandler.Invoke(LogicEventType.OnUIUpdate);
    }

    protected virtual void Init()
    {
        //임시
        m_InitStatus.HP = UnityEngine.Random.Range(10, 100);
        m_InitStatus.Cost = UnityEngine.Random.Range(10, 99);
        m_InitStatus.Armor = UnityEngine.Random.Range(0, 10);
        m_CurrentStatus = m_InitStatus;
    }
    public virtual void EntityReset()
    {
        m_CurrentStatus = m_InitStatus;
        m_Target = null;
        this.ResetBuff();
    }

    /// <summary>
    /// 타겟 선택
    /// </summary>
    protected abstract void SelectTarget();

    protected virtual void Die()
    {
        Debug.Log($"[{name}] : 이 죽음");
        GameObject.Destroy(gameObject);
    }
    protected virtual bool Attack()
    {
        if (null == m_Target)
        {
            Debug.Log($"{name}의 목표가 없음");
            return false;
        }
        Debug.Log($"{name} 이 {m_Target.name} 을 공격");
        return true;
    }

    /// <summary>
    /// 아이템으로 공격시 성공하면 True
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Attack(Item item)
    {
        if (null == m_Target)
        {
            Debug.Log($"{name}의 목표가 없음");
            return false;
        }
        if (m_CurrentStatus.Cost < item.m_CurrentStatus.Cost)
        {
            Debug.Log($"{name}의 코스트가 없음");
            return false;
        }
        EntityEventHandler.Invoke(LogicEventType.OnE_AttackEvent);
        //m_Target?.HitDamage(this, item);
        
        return true;
    }
    /// <summary>
    /// 자신이 데미지를 어택커로부터 받음
    /// </summary>
    public void HitDamage(Entity p_Attacker,Item p_Item)
    {
        HitDamage(p_Attacker, p_Item.m_CurrentStatus.Damage);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_IgoneArmor">방어무시 여부</param>
    public void HitDamage(Entity p_Attacker, int p_Damage,bool p_IgoneArmor =false)
    {
        if (null != p_Attacker)
            Debug.Log($"{p_Attacker.name} 이 {name} 을 {p_Damage} 데미지");

        int damage = p_Damage;
        if (!p_IgoneArmor&&m_CurrentStatus.Armor > 0)
        {
            int temp = m_CurrentStatus.Armor - damage;
            EntityEventHandler.GetEvent(LogicEventType.OnE_ArmorHitEvent, "ArmorChangeUIEvent")
                .SetArgs((int)MathF.Min(0, -damage));
            EntityEventHandler.Invoke(LogicEventType.OnE_ArmorHitEvent);
            if (temp > 0)
            {
                m_CurrentStatus.Armor = temp;
                return;
            }
            m_CurrentStatus.Armor = 0;
            damage = -temp;
        }
        if (damage > 0)
        {
            EntityEventHandler.GetEvent(LogicEventType.OnE_HitEvent, "HpChangeUIEvent").SetArgs(-damage);
            EntityEventHandler.Invoke(LogicEventType.OnE_HitEvent);
            if (m_CurrentStatus.HP > damage)
                m_CurrentStatus.HP -= damage;
            else
            {
                m_CurrentStatus.HP = 0;
                Die();
            }
        }
    }
    /// <summary>
    /// 전투시 번 시작전 준비
    /// </summary>
    public abstract void EntityReadyTurn();

    /// <summary>
    /// True면 다음 차례로 턴을 넘김
    /// </summary>
    /// <returns></returns>
    public virtual bool EntityEndTurn()
    {
        return false;
    }

}

[Serializable]
public struct EntityStatus
{
    public int HP, MP, Armor;
    public int Cost;
}

[Serializable]
public struct EntityEditorInvenItem
{
    public Vector2Int SlotIndex;
    public ItemScriptObj Data;
}