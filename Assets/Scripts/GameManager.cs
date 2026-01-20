using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogicEvent;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance = null;
    public static GameManager Current { get
        {
            if(null == Instance)
                Instance = FindAnyObjectByType<GameManager>();
            return Instance;
        } 
    }


    [SerializeField] private Player m_Player;
    public Player Player { get { return m_Player; } }
    private void Awake()
    {
        Instance = this;
    }


    //임시
    public bool m_IsStart = false;
    public bool m_IsEnd = false;
    private void Update()
    {
        if (m_IsStart)
        {
            Player.m_Stat = PlayerState.Battle;
            //TempInit();
            InventoryUI.Current.RemoveNoneInstallItem();
            BattleManager.BattleStart();
            m_IsStart = false;
        }
        if (m_IsEnd)
        {
            BattleManager.BattleEnd();
            m_IsEnd = false;
        }
    }
    /// <summary>
    /// 디버깅용
    /// </summary>
    void TempInit()
    {
        BattleManager.Current.BattleEventHandler
            .AddEvent(LogicEventType.OnTurnStartEvent, new LogicEventBase(TurnStart, 10));
        BattleManager.Current.BattleEventHandler
            .AddEvent(LogicEventType.OnTurnEndEvent, new LogicEventBase(TurnEnd, 10));
        BattleManager.Current.BattleEventHandler
            .AddEvent(LogicEventType.OnBattleStartEvent, new LogicEventBase(BattleStart, 10));
        BattleManager.Current.BattleEventHandler
            .AddEvent(LogicEventType.OnBattleEndEvent, new LogicEventBase(BattleEnd, 10));
    }
    void TurnStart(object[] args)
    {
        Debug.Log("TurnStart");
    }
    void TurnEnd(object[] args)
    {
        Debug.Log("TurnEnd");
    }
    void BattleEnd(object[] args)
    {
        Debug.Log("BattleEnd");
    }
    void BattleStart(object[] args)
    {
        Debug.Log("BattleStart");
    }
}

public enum UISortingIndex
{
    ItemUIBG = 1000,
    ItemUI = 1010,
    HoldItemUI = 1100
}