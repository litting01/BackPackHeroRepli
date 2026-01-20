using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;
using Unity.VisualScripting;
using System;


namespace Buff
{
    public class BuffManager : MonoBehaviour
    {
        static BuffManager instance = null;
        public static BuffManager Current
        {
            get
            {
                if (null == instance)
                    instance = FindFirstObjectByType<BuffManager>();
                return instance;
            }
        }
        private void Awake() => instance = this;

        GameObject m_UIPrefa = null;
        public GameObject UIPrefap
        {
            get
            {
                if (null == m_UIPrefa)
                    m_UIPrefa = (GameObject)ResourceManager.ResourceLoad(ResourceType.Prefaps, "I_BuffIcon");
                return m_UIPrefa;
            }
        }
        /// <summary>
        /// 확장메서드로 할거면 이거 왜필요한거지
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static BuffBase CreateBuf(BuffInfo buf)
        {
            Type type = null;
            type = Type.GetType(buf.BuffName);
            if (null == type)
            {
                Debug.Log($"버프 타입 :{buf.BuffName}이 없음");
                return null;
            }
            GameObject clone = GameObject.Instantiate(Current.UIPrefap);
            BuffBase temp = (BuffBase)clone.AddComponent(type);
            return temp;
        }
    }
    public static class BuffManagerExpansion
    {
        /// <summary>
        /// 해당 엔티티에 버프 추가
        /// </summary>
        public static Entity AddBuff(this Entity en,BuffInfo buf)
        {
            if (null == en)
                return en;
            BuffBase temp = en.FindBuff(buf.BuffName);
            if(null == temp)
            {
                temp = BuffManager.CreateBuf(buf);
                temp.Init(en, buf);
                temp.transform.SetParent(en.m_BuffTran);
                return en;
            }
            //만약 해당 버프가 이미 있을경우 스택이 늘어나야함
            temp.CurrentStack += buf.StackCount;
            return en;
        }
        /// <summary>
        /// 해당 버프를 이름으로 찾음
        /// </summary>
        public static BuffBase FindBuff(this Entity en, string p_BufName)
        {
            if (null == en)
                return null;
            for(int i=0;i<en.m_BuffTran.childCount;i++)
            {
                Transform now = en.m_BuffTran.GetChild(i);
                if (now.name == p_BufName)
                    return now.GetComponent<BuffBase>();
            }
            return null;
        }
        public static void ResetBuff(this Entity en)
        {
            if (null == en)
                return;
            for(int i = en.m_BuffTran.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(en.m_BuffTran.GetChild(i).gameObject);
            }

        }
    }

}
