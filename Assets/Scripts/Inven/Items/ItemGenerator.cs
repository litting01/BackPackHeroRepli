using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Pool;

public class ItemGenerator : MonoBehaviour
{
    private static ItemGenerator instance = null;
    public static ItemGenerator Current { get { return instance; } }
    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
            m_ItemPool = new Stack<ItemUI>(m_PoolMaxCount);
        }
    }

    public GameObject m_ItemUIPrefaps = null;
    public Transform m_PoolTran;
    public int m_PoolMaxCount = 10;

    private Stack<ItemUI> m_ItemPool = null;
    public List<EntityEditorInvenItem> m_DebugSpawnData;

    [ContextMenu("아이템 생성")]
    public void SpawnItem()
    {
        foreach(var element in m_DebugSpawnData)
        {
            ItemUI temp = Get(BattleManager.Current.Player, element);
            temp.transform.SetParent(InventoryUI.Current.transform);
            temp.ResetInstall();
        }
    }
    [ContextMenu("아이템 삭제")]
    public void RemoveItem()
    {
        ItemUI temp = FindAnyObjectByType<ItemUI>();
        if(null != temp)
            Release(temp); 
    }

    public static ItemUI Get(Entity p_En ,EntityEditorInvenItem p_Data)
    {
        return Current.InGetItemUI(p_En, p_Data);
    }
    private ItemUI InGetItemUI(Entity p_En, EntityEditorInvenItem p_Data)
    {
        ItemUI temp = null;

        if (m_ItemPool.Count > 0)
        {
            temp = m_ItemPool.Pop();
            temp.Data.InitItem(p_Data, p_En);
            temp.Data = temp.Data;
        }
        else
            temp = CreateItemUI(p_En, p_Data);
        if (null != temp)
            OnGetItemUI(temp, p_En, p_Data);
        return temp;
    }

    public static void Release(ItemUI p_Data)
    {
        Current.InRelease(p_Data);
    }
    private void InRelease(ItemUI p_Data)
    {
        if(m_ItemPool.Count >= m_PoolMaxCount)
        {
            GameObject.Destroy(p_Data.gameObject);
            return;
        }
        p_Data.Data = null;
        OnReleaseItemUI(p_Data);
    }

    private ItemUI CreateItemUI(Entity p_En, EntityEditorInvenItem p_Data)
    {
        Type t = Type.GetType(p_Data.Data.name);
        if (t == null)
        {
            Debug.Log(p_En.name + " 의 아이템 : " + p_Data.Data.name + "의 타입을 가져올수 없음");
            return null;
        }
        GameObject clone = GameObject.Instantiate(m_ItemUIPrefaps);
        Item item = (Item)Activator.CreateInstance(t);
        item.InitItem(p_Data, p_En);

        ItemUI itemUI = clone.GetComponent<ItemUI>();
        itemUI.Data = item;
        return itemUI;
    }

    private void OnGetItemUI(ItemUI p_Item, Entity p_En, EntityEditorInvenItem p_Data)
    {
        p_Item.gameObject.SetActive(true);
        p_Item.transform.SetParent(null);
    }
    private void OnReleaseItemUI(ItemUI p_Item)
    {
        p_Item.gameObject.SetActive(false);
        p_Item.transform.SetParent(m_PoolTran,false);
        p_Item.transform.localPosition = Vector3.zero;
        m_ItemPool.Push(p_Item);
    }
}
public static class ItemGeneratorExtensions
{
    public static void ReleaseItem(this ItemUI p_Data)
    {
        ItemGenerator.Release(p_Data);
    }
}