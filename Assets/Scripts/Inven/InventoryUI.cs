using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI m_Instance;
    public static InventoryUI Current
    {
        get
        {
            if (null==m_Instance)
            {
                m_Instance = FindFirstObjectByType<InventoryUI>();
            }
            return m_Instance;
        }
    }

    [SerializeField] private GameObject m_SlotPrefaps;
    [SerializeField] private GameObject m_ItemPrefaps;

    public Transform m_SlotGridTran;

    /// <summary>
    /// 안에 String는 해당슬롯에 들어간 아이템 이름
    /// </summary>
    private string[,] m_SlotMap;
    private Vector2Int m_MaxSlotSize = Vector2Int.zero;
    public Vector2Int MaxSlotIndex
    {
        get
        {
            if (null != SlotGridStr&&m_MaxSlotSize == Vector2Int.zero)
            {
                string[] SlotGrid = SlotGridStr.Split(" ");
                m_MaxSlotSize.y = SlotGrid.Length;
                for (int i = 0; i < m_MaxSlotSize.y; i++)
                    m_MaxSlotSize.x = m_MaxSlotSize.x < SlotGrid[i].Length ? SlotGrid[i].Length : m_MaxSlotSize.x;
            }
            return m_MaxSlotSize;
        }
    }

    Player Player { get { return GameManager.Current.Player; } }
    string SlotGridStr { get { return Player.m_InvenGridStr; } }

    //현재 잡고있는 아이템
    //public ItemUI m_HoldItem = null;
    public Item m_HoldItem = null;
    public Transform m_ItemTran;
    
    //임시
    public Vector2Int SlotImageSize = Vector2Int.zero;

    private void Start()
    {
        Init();
    }
    void Init()
    {
        GeneratSlot();
        foreach (var item in Player.Inven)
        {
            Item now = ItemGenerator.Get(Player,item);
            now.transform.SetParent(m_ItemTran, true);
            now.transform.position = RanItemPos;
            ItemSlot slot = GetSlot(now.m_SlotIndex);
            slot?.InputItem(now);
        }
    }

    public void ResetInven()
    {
        //모든 내용 지우기
    }
    public void RemoveNoneInstallItem()
    {
        foreach(var element in transform.GetComponentsInChildren<Item>())
        {
            if (element.m_IsInstall)
                continue;
            ItemGenerator.Release(element);
        }
    }

    /// <summary>
    /// 슬롯 그리드 STR 규칙
    /// 띄어씌기는 다음행 ) "000 000"
    /// 슬롯 위치는 1 ) "010 010 000"
    /// 중심 위치는 2 ) "111 12111 111"
    /// </summary>
    [ContextMenu("슬롯생성")]
    private void GeneratSlot()
    {
        if (null == SlotGridStr)
            return;
        m_SlotMap = new string[MaxSlotIndex.y, MaxSlotIndex.x];
        if (m_SlotGridTran.childCount != 0)
        {
            int n = m_SlotGridTran.childCount;
            for (int i = 0; i < n; i++)
            {
                GameObject.DestroyImmediate(m_SlotGridTran.GetChild(0).gameObject);
            }

        }
        string[] SlotGrid = SlotGridStr.Split(" ");
        for(int i=0;i< MaxSlotIndex.y; i++)
        {
            for(int j=0;j< MaxSlotIndex.x; j++)
            {
                char now = SlotGrid[i][j];
                if (now == '0')
                    continue;

                GameObject clone = Instantiate(m_SlotPrefaps);
                clone.transform.SetParent(m_SlotGridTran, false);

                Vector2 pos = new Vector2();
                pos.x = SlotImageSize.x / 2 * (j - (MaxSlotIndex.x - 1) / 2.0f) * 2;
                pos.y = SlotImageSize.y / 2 * -(i - (MaxSlotIndex.y - 1) / 2.0f) * 2;

                clone.transform.localPosition = pos;
                
                ItemSlot slot = clone.GetComponent<ItemSlot>();
                slot.name = $"[ Slot {j}_{i} ]";
                slot.m_SlotIndex = new Vector2Int(j, i);

                m_SlotMap[i, j] = null;
            }
        }   
        for(int i=0;i<m_SlotGridTran.childCount;i++)
        {
            Transform tran = m_SlotGridTran.GetChild(i);
            ItemSlot slot = tran.GetComponent<ItemSlot>();
            m_SlotMap[slot.m_SlotIndex.y, slot.m_SlotIndex.x] = tran.name;
        }
    }

    /// <summary>
    /// 트랜스폼 FindChild
    /// </summary>
    public static ItemSlot GetSlot(string p_name)
    {
        Transform child = Current.m_SlotGridTran.FindDeepChild(p_name);
        if (null != child)
            return child.GetComponent<ItemSlot>();
        return null;
    }
    /// <summary>
    /// 인덱스에 대응하는 슬롯 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ItemSlot GetSlot(Vector2Int p_Index)
    {
        if (IsInInven(p_Index))
            return GetSlot(Current.m_SlotMap[p_Index.y,p_Index.x]);
        return null;
    }
    /// <summary>
    /// 마우스포인터에 닿은 슬롯
    /// </summary>
    /// <returns></returns>
    public static ItemSlot GetSlot()
    {
        ItemSlot temp = null;
        for(int i = 0; i < Current.m_SlotGridTran.childCount; i++)
        {
            Transform now = Current.m_SlotGridTran.GetChild(i);
            bool isCol = RectTransformUtility.RectangleContainsScreenPoint(now.GetComponent<RectTransform>(), Input.mousePosition);
            if (isCol)
            {
                if (now.TryGetComponent(out temp))
                    continue;
            }
        }
        return temp;
    }
    /// <summary>
    /// 박스콜리전에 닿은 슬롯
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public static ItemSlot GetSlot(BoxCollider2D col)
    {
        ItemSlot temp = null;
        ContactFilter2D filter = new ContactFilter2D();

        List<Collider2D> list = new List<Collider2D>();
        Physics2D.OverlapCollider(col, filter.NoFilter(), list);
        foreach(Collider2D element in list)
        {
            if (element.TryGetComponent(out temp))
                break;
        }
        return temp;
    }

    /// <summary>
    /// 해당 좌표에 설치가 가능한가
    /// </summary>
    public static bool IsCanInstall(Vector2Int p_Pos)
    {
        if (!IsInInven(p_Pos))
            return false;
        if (null != Current.m_SlotMap[p_Pos.y, p_Pos.x])
            return false;
        return true;
    }

    /// <summary>
    /// 해당 좌표가 인벤내에 있는가
    /// </summary>
    /// <param name="p_index"></param>
    /// <returns></returns>
    public static bool IsInInven(Vector2Int p_index)
    {
        if(p_index.x.Between(-1, Current.MaxSlotIndex.x)&& p_index.y.Between(-1, Current.MaxSlotIndex.y))
            return true;
        return false;
    }

    public static Vector3 RanItemPos
    {
        get
        {
            Rect size = Current.GetComponent<RectTransform>().rect;
            
            Vector3 vec = new Vector3();
            Vector3 pos = Current.m_SlotGridTran.transform.position;
            vec.x = UnityEngine.Random.Range(-300, 300) + pos.x;   
            vec.y = pos.y - size.height * 0.5f;

            return vec;
        }
    }

}