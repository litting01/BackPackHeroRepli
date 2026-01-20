using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicEvent;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Current { get; private set; }
    private void Awake()
    {
        Current = this;
    }

    public GameObject m_EnemyPrefap = null;
    public Transform spawnTrn = null;

    [SerializeField] private List<EditorEnemyInfo> m_EnemyDataList;

    [SerializeField] private Player m_Player = null;
    public Player Player
    {
        get
        {
            return m_Player;
        }
        private set
        {
            if(null != m_Player)
                m_Player = FindFirstObjectByType<Player>();
        }
    }

    [SerializeField] private int m_CurTurn = 0;
    public int CurTurn { get { return m_CurTurn; } }
    public Text T_Turn;

    [SerializeField] Entity m_ActiveEntity = null;
    List<Entity> m_TurnList = new List<Entity>();
    List<Entity> m_EntityList = new List<Entity>();

    LogicEventHandler<LogicEventType> eventHandler = null;
    public LogicEventHandler<LogicEventType> BattleEventHandler
    {
        get
        {
            if (null == eventHandler)
                eventHandler = new LogicEventHandler<LogicEventType>();
            return eventHandler;
        }
    }

    //임시
    bool m_IsTrig = false;
    bool m_IsTurnTrig = true;

    public static void BattleStart()
    {
        Current.InBattleStart();
    }
    public static void BattleEnd()
    {
        Current.InBattleEnd();
    }

    void InBattleStart()
    {
        m_IsTrig = true;
        m_CurTurn = 0;
        m_EntityList.Add(Player);
        Player.EntityReset();
        //MapGenerator에서 소환할 적 정보 받아오기
        SpawnEnemy();
        BattleEventHandler.Invoke(LogicEventType.OnBattleStartEvent);
        BattleEventHandler.AddEvent(LogicEventType.OnUIUpdate, new LogicEventBase(BattleUIUpdate, 0));
    }
    void TurnStart()
    {
        m_IsTurnTrig = false;
        m_CurTurn++;
        m_TurnList.Clear();
        m_TurnList.Add(Player);
        for (int i = 0; i < m_EntityList.Count; i++)
        {
            m_TurnList.Add(m_EntityList[i]);
        }
        //만약 속도 넣는다면 여기서 리스트 정렬
        foreach(Entity element in m_TurnList)
        {
            element.EntityReadyTurn();
        }
        BattleEventHandler.Invoke(LogicEventType.OnTurnStartEvent);
    }
    void TurnUpdate()
    {
        m_EntityList = m_EntityList.Where(e => e != null).ToList();
        m_TurnList = m_TurnList.Where(e => e != null).ToList();
        if (m_IsTurnTrig)
            TurnStart();
        if (null == m_ActiveEntity)
            SetActivityEntity();
        if (m_ActiveEntity.EntityEndTurn())
        {
            m_ActiveEntity.EntityEventHandler.Invoke(LogicEventType.OnE_TurnEndEvent);
            m_ActiveEntity = null;
            if (m_TurnList.Count <= 0)
                TurnEnd();
        }
    }
    void TurnEnd()
    {
        m_IsTurnTrig = true;
        BattleEventHandler.Invoke(LogicEventType.OnTurnEndEvent);
        if (m_EntityList.Count < 0 || Player.IsBattleEnd())
            InBattleEnd();
    }
    void InBattleEnd()
    {
        m_CurTurn = 0;
        m_IsTrig = false;
        m_IsTurnTrig = true;
        m_ActiveEntity = null;
        m_TurnList.Clear();
        m_EntityList = m_EntityList.Where(e => e != null).ToList();
        int n = m_EntityList.Count - 1;
        for (int i = n; i > 0; i--)
        {
            GameObject.Destroy(m_EntityList[i].gameObject);
            m_EntityList.RemoveAt(i);
        }
        m_EntityList.Clear();
        BattleEventHandler.Invoke(LogicEventType.OnBattleEndEvent);

        Player.EntityReset();
        BattleEventHandler.RemoveAll();
        BattleUIReset();
    }

    void SetActivityEntity()
    {
        /*if (m_TurnList.Count <= 0)
            ReadyTurn();*/
        m_ActiveEntity = m_TurnList.First();
        m_TurnList.Remove(m_ActiveEntity);
        m_ActiveEntity.EntityEventHandler.Invoke(LogicEventType.OnE_TurnStartEvent);
    }
    private void Update()
    {
        if (m_IsTrig)
        {
            TurnUpdate();
            BattleEventHandler.Invoke(LogicEventType.OnUIUpdate);
        }
    }
    void BattleUIUpdate(params object[] args)
    {
        T_Turn.text = m_CurTurn.ToString();
    }
    
    //임시
    void BattleUIReset()
    {
        T_Turn.text = " ";
    }


    /*void ReadyTurn()
    {
        m_CurTurn++;
        m_TurnList.Add(Player);

        m_EnemyList = m_EnemyList.Where(e => e != null).ToList();
        foreach (Entity e in m_EnemyList)
        {
            m_TurnList.Add(e);
        }

        //m_TurnList = m_TurnList.Where(e => e != null).ToList();
        foreach (Entity item in m_TurnList)
        {
            item.EntityReadyTurn();
            item.EntityEventHandler.Invoke(LogicEventType.OnTurnStartEvent);
        }
        BattleEventHandler.Invoke(LogicEventType.OnTurnStartEvent);
    }*/
    /*void PlayTurn()
    {
        if(null == m_ActiveEntity)
            SetActivityEntity();
        
        if (m_ActiveEntity.EntityEndTurn())
        {
            m_ActiveEntity.EntityEventHandler.Invoke(LogicEventType.OnE_TurnEndEvent);
            m_ActiveEntity = null;
            if(m_TurnList.Count <= 0)
            {
                BattleEventHandler.Invoke(LogicEventType.OnTurnEndEvent);
                //겜 끝났는지 판별
                ReadyTurn();
            }
        }
    }*/

    //임시
    void SpawnEnemy()
    {
        List<int> ranList = new List<int>();
        int n = (int)MathF.Min(spawnTrn.childCount, m_EnemyDataList.Count);
        while(ranList.Count != n)
        {
            int ran = UnityEngine.Random.Range(0, m_EnemyDataList.Count - 1);
            ranList.Add(ran);
        }
        for(int i = 0; i < ranList.Count; i++)
        {
            GameObject obj = CreateEnmey(m_EnemyDataList[i]);
            int sortIndex = spawnTrn.childCount - i - 1;
            obj.transform.position = spawnTrn.GetChild(sortIndex).position;

            m_EntityList.Add(obj.GetComponent<Entity>());
        }
    }
    private GameObject CreateEnmey(EditorEnemyInfo p_Data)
    {
        GameObject clone = Instantiate(m_EnemyPrefap);
        clone.transform.SetParent(spawnTrn.parent);
        clone.name = p_Data.EnemyName;

        Enemy enemy = clone.GetComponent<Enemy>();
        enemy.EnemyReset(p_Data);

        return clone;
    }

    public static bool IsMyTurn(Entity e)
    {
        if (Current.m_IsTrig)
            return true;
        if (Current.m_ActiveEntity == e)
            return true;
        return false;
    }
    
    //임시
    public static Entity[] GetEnemy(int p_Count)
    {
        int n = (int)MathF.Min(p_Count, Current.m_TurnList.Count);
        if(n <= 0)
            return null;
        Entity[] en = new Entity[n];
        for(int i = 0; i < n; i++)
        {
            en[i] = Current.m_TurnList[i];
        }
        return en;
    }
}
