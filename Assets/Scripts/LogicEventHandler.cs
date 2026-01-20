using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LogicEvent
{
    public enum LogicEventType
    {
        None = 0,
        OnBattleStartEvent,
        OnBattleEndEvent,
        OnTurnStartEvent,
        OnTurnEndEvent,
        OnUIUpdate,

        OnE_TurnStartEvent = 1001,
        OnE_TurnEndEvent,
        OnE_AttackEvent,
        OnE_HitEvent,
        OnE_ArmorHitEvent,
    }
    public delegate void LogicEventDel(params object[] p_Args);
    public delegate bool ConditionDel();
    public class LogicEventBase : IComparer<LogicEventBase>
    {
        public int m_SortIndex = 0;
        public bool isRefeat = true;
        public object[] m_Args = null;

        public LogicEventDel OnEvent;
        public ConditionDel OnCondiFunc = null;

        public LogicEventBase(LogicEventDel p_Event,int p_sortIndex)
        {
            OnEvent = p_Event;
            m_SortIndex = p_sortIndex;
        }

        public int Compare(LogicEventBase x, LogicEventBase y)
        {
            return x.m_SortIndex.CompareTo(y.m_SortIndex);
        }
        public bool IsEventEquals(LogicEventDel obj)
        {
            if (obj == null || OnEvent == null)
                return false;
            if (obj.Method == OnEvent.Method)
                return true;
            return false;
        }
        public virtual void Invoke()
        {
            if(null == OnCondiFunc)
                OnEvent.Invoke(m_Args);
            else
            {
                if(OnCondiFunc.Invoke())
                    OnEvent.Invoke(m_Args);
            }
        }
    }
    public static class LogicEventSettingExtensions
    {
        /// <summary>
        /// 실행 조건 함수 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_Obj"></param>
        /// <param name="p_SatisfyFunc">조건</param>
        /// <returns></returns>
        public static T AddCondiFunc<T>(this T p_Obj,ConditionDel p_SatisfyFunc) where T : LogicEventBase
        {
            if (!IsCanModify(p_Obj))
                return p_Obj;
            p_Obj.OnCondiFunc = p_SatisfyFunc;
            return p_Obj;
        }
        public static T SetSortIndex<T>(this T p_Obj,int index) where T : LogicEventBase
        {
            if (!IsCanModify(p_Obj))
                return p_Obj;
            p_Obj.m_SortIndex = index;
            return p_Obj;
        }
        public static T SetArgs<T>(this T p_Obj,params object[] p_Args) where T : LogicEventBase
        {
            if(!IsCanModify(p_Obj))
                return p_Obj;
            p_Obj.m_Args = p_Args;
            return p_Obj;
        }
        
        static bool IsCanModify<T>(this T p_Obj) where T : LogicEventBase
        {
            if (p_Obj == null)
                return false;

            return true;
        }
    }


    public class LogicEventHandler<T>
    {
        private Dictionary<string, List<LogicEventBase>> m_Dict = new Dictionary<string, List<LogicEventBase>>();

        public void AddEvent(T p_Type, LogicEventBase p_Event)
        {
            if (null == p_Event)
                return;
            string str = p_Type.ToString();
            if (!m_Dict.ContainsKey(str))
                m_Dict.Add(str, new List<LogicEventBase>());
            m_Dict[str].Add(p_Event);
            m_Dict[str].Sort(p_Event);
        }
        /// <summary>
        /// 함수 이름으로 비교하는거라 람다식으로 넣는건 불가능
        /// </summary>\
        /// <param name="str">해당 이벤트 클래스 이름</param>
        /// <param name="p_Event"></param>
        public void RemoveEvent(T p_Type, LogicEventDel p_Event)
        {
            string str = p_Type.ToString();
            if (!m_Dict.ContainsKey(str))
            {
                Debug.Log(str + " 이벤트 키가 없어 제거 하지 못함");
                return;
            }
            LogicEventBase t = m_Dict[str].Find((e) =>
            {
                if (p_Event == null)
                    return false;
                if (e.IsEventEquals(p_Event))
                    return true;
                return false;
            });
            if (null != t)
                m_Dict[str].Remove(t);
        }

        /// <summary>
        /// 타입의 이벤트만 지움
        /// 안전하게 지우는건 아님
        /// </summary>
        /// <param name="p_Type"></param>
        public void RemoveAll (T p_Type)
        {
            string str = p_Type.ToString();
            if (m_Dict.ContainsKey(str))
            {
                m_Dict.Remove(str);
            }
        }
        public void RemoveAll()
        {
            m_Dict.Clear();
        }

        /// <summary>
        /// 타입에서 이벤트 반환
        /// </summary>
        /// <param name="p_Type"></param>
        /// <param name="p_Func">람다식 안됨</param>
        /// <returns></returns>
        public LogicEventBase GetEvent(LogicEventType p_Type, string p_FuncName)
        {
            string type = p_Type.ToString();
            if (!m_Dict.ContainsKey(type))
                return null;
            LogicEventBase result = null;
            result = m_Dict[type].Find((e) =>
            {
                if(e.OnEvent.Method.Name == p_FuncName)
                    return true;
                return false;
            });
            return result;
        }

        /// <summary>
        /// string으로 타입 검색
        /// </summary>
        /// <param name="str">클래스</param>
        public LogicEventBase Invoke(string str)
        {
            LogicEventBase now = null;
            if (!m_Dict.ContainsKey(str))
                return now;
            int count = 0;
            int size = m_Dict[str].Count;
            for (int i = 0; i < size; i++)
            {
                if (i - count >= m_Dict[str].Count)
                    Debug.Log("LogicError");
                now = m_Dict[str][i - count];
                now.Invoke();
                if (!now.isRefeat)
                {
                    m_Dict[str].Remove(now);
                    now = null;
                    count++;
                }
            }
            return now;
        }

        /// <summary>
        /// 함수 이름으로 검색(람다식 불가능)
        /// </summary>
        /// <param name="p_Event"></param>
        public LogicEventBase Invoke(LogicEventBase p_Event)
        {
            if (null == p_Event)
                return null;
            string str = p_Event.GetType().Name;
            return Invoke(str);
        }
        public LogicEventBase Invoke(T p_Type)
        {
            string str = p_Type.ToString();
            return Invoke(str);
        }
    }

    /*namespace OtherLogicEvent
    {
        /// <summary>
        /// 조건충족 이벤트클래스
        /// </summary>
        public class SatisfyLogicEvent : LogicEventBase
        {
            public delegate bool CondiFunc();
            public CondiFunc m_conFunc = null;
            /// <param name="conFunc">실행조건함수(람다식 가능)</param>
            /// <param name="p_Event"></param>
            /// <param name="p_Index"></param>
            /// <param name="p_IsRefeat"></param>
            public SatisfyLogicEvent(CondiFunc conFunc, LogicEventDel p_Event,
                int p_Index = 0, bool p_IsRefeat = false) : base(p_Event, p_Index, p_IsRefeat)
            {
                m_conFunc = conFunc;
            }
            public override void Invoke()
            {
                if (null == m_conFunc)
                    base.Invoke();
                if (m_conFunc())
                    base.Invoke();

            }
        }

        public class ParamLogicEvent : LogicEventBase
        {
            public delegate bool ParamFunc(params object[] args);
            ParamFunc m_Func = null;
            public ParamLogicEvent(ParamFunc p_Func,LogicEventDel p_Event, int p_Index = 0, bool p_IsRefeat = false) : base(p_Event, p_Index, p_IsRefeat)
            {
                m_Func = p_Func;
            }

            public void ParamInvoke(params object[] args)
            {
                if (null == m_Func)
                    return;
                if (m_Func())
                    this.Invoke();
            }
        }

        /// <summary>
        /// 코루틴 이벤트클래스 미완
        /// </summary>
        public class CoroutineLogicEvent : LogicEventBase
        {
            public struct CoroutineData
            {
                public float DelayTime;
                
                /// <summary>
                /// false면 코루틴 실행되고 기다린뒤 넘어감
                /// </summary>
                public bool PassCoroutine;
            }

            MonoBehaviour m_Owner;
            //코루틴을 돌고난뒤 넘어가게 할건지
            //
            public CoroutineLogicEvent(MonoBehaviour p_Owner, LogicEventDel p_Event, int p_Index = 0, bool p_IsRefeat = false)
                : base(p_Event, p_Index, p_IsRefeat)
            {
                m_Owner = p_Owner;
            }
            public override void Invoke()
            {
                base.Invoke();
                m_Owner.StartCoroutine(GetA());
            }
            IEnumerator GetA()
            {

                yield return new WaitForSeconds(1);
            }
        }
    }
    */
}