
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Enemy : Entity
{
    EditorEnemyInfo m_Data;
    UseItemListUI m_UseItemUI;

    public void EnemyReset(EditorEnemyInfo p_Data)
    {
        m_UseItemUI = GetComponentInChildren<UseItemListUI>();
        m_Data = p_Data;
        m_EditorInven = m_Data.EnemyInven;
        Init();
    }
    protected override void SelectTarget()
    {
        Player player = FindFirstObjectByType<Player>();
        if (null == player)
            Debug.Log("플레이어 선택안됨");
        m_Target = player;
        //기본은 플레이어만 하는데 힐같은건 다른 적을 타겟으로 해야함
    }
    protected override bool Attack()
    {
        bool result = true;
        SelectTarget();
        int n = m_UseItemUI.m_UseItemList.Count;
        for (int i = 0; i < n; i++)
        {
            result = UseItem(m_UseItemUI.m_UseItemList.Peek());
            if (result)
                m_UseItemUI.m_UseItemList.Peek().ActiveItem();
            m_UseItemUI.RemoveItem();
        }

        return result;
    }

    public void UseItemSelect()
    {
        if (Inven.Count <= 0)
            return;
        m_UseItemUI.Init();
        int n = UnityEngine.Random.Range(1, Inven.Count);
        for (int i = 0; i < n; i++)
        {
            int v = UnityEngine.Random.Range(0, Inven.Count);
            EntityEditorInvenItem now = Inven[v];
   
            m_UseItemUI.AddItem(now);
        }
    }

    public override bool EntityEndTurn()
    {

        return Attack();
    }

    public override void EntityReadyTurn()
    {
        UseItemSelect();
    }
}

[Serializable]
public struct EditorEnemyInfo
{
    public string EnemyName;
    [SerializeField] public List<EntityEditorInvenItem> EnemyInven;
}