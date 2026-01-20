using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using LogicEvent;
using UnityEngine.UI;


namespace Buff
{
    /// <summary>
    /// 버프 생성할때 사용될 정보
    /// </summary>
    public struct BuffInfo
    {
        public string BuffName;
        public int StackCount;
    }

    public abstract class BuffBase : MonoBehaviour
    {
        protected BuffInfo m_Data;
        public string BufName => nameof(GetType);

        int m_CurrentStack = 0;
        public int CurrentStack
        {
            get
            {
                return m_CurrentStack;
            }
            set
            {
                m_CurrentStack = value;
                UIUpdate();
            }
        }
        protected int m_MaxStack = 99;
        public int MaxStack { get { return m_MaxStack; } }

        Entity m_LinkedEntity = null;
        protected Entity En { get { return m_LinkedEntity; } }

        Image I_UIIcon;
        Text T_BuffCount;

        public virtual void Init(Entity p_LinkEntity,BuffInfo p_Data)
        {
            m_LinkedEntity = p_LinkEntity;
            BattleManager.Current.BattleEventHandler
                .AddEvent(LogicEventType.OnTurnEndEvent, new LogicEventBase(BuffUpdate, 10));
            m_Data = p_Data;
            name = m_Data.BuffName;
            UIInit();
            CurrentStack = m_Data.StackCount;
        }
        public abstract void BuffReset();
        public virtual void BuffUpdate(params object[] args)
        {
            if (null == m_LinkedEntity)
            {
                Release(); 
                return;
            }
            BuffActive();
            
            CurrentStack--;
            if (CurrentStack <= 0)
                Release();
            UIUpdate();
        }
        public abstract void BuffActive();
        public virtual void Release()
        {
            BattleManager.Current.BattleEventHandler
                .RemoveEvent(LogicEventType.OnTurnEndEvent, BuffUpdate);
            GameObject.Destroy(this);
        }

        public virtual void UIInit()
        {
            I_UIIcon = GetComponent<Image>();
            T_BuffCount = GetComponentInChildren<Text>();
            //임시
            //I_UIIcon.sprite = ResourceManager.Load<Sprite>(m_Data.BuffName)
            //T_BuffCount.text = CurrentStack.ToString();
        }
        public virtual void UIUpdate()
        {
            T_BuffCount.text = CurrentStack.ToString();
        }
    }
}