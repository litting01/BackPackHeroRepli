using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    private static ItemGenerator instance = null;
    public static ItemGenerator Current { get { return instance; } }
    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
            m_ItemPool = new Stack<GameObject>(m_PoolMaxCount);
        }
    }

    public GameObject m_ItemUIPrefaps = null;
    public Transform m_PoolTran;
    public int m_PoolMaxCount = 10;

    private Stack<GameObject> m_ItemPool = null;
    public List<EntityEditorInvenItem> m_DebugSpawnData;

    [ContextMenu("아이템 생성")]
    public void SpawnItem()
    {
        foreach(var element in m_DebugSpawnData)
        {
            Item temp = Get(BattleManager.Current.Player, element);
            temp.transform.SetParent(InventoryUI.Current.transform);
            temp.ResetInstall();
        }
    }
    [ContextMenu("아이템 삭제")]
    public void RemoveItem()
    {
        Item temp = FindAnyObjectByType<Item>();
        if(null != temp)
            Release(temp); 
    }

    public static Item Get(Entity p_En ,EntityEditorInvenItem p_Data)
    {
        return Current.InGetItem(p_En, p_Data);
    }
    private Item InGetItem(Entity p_En, EntityEditorInvenItem p_Data)
    {
        GameObject obj = null;
        Item temp = null;

        if (m_ItemPool.Count > 0)
        {
            obj = m_ItemPool.Pop();
            Type t = Type.GetType(p_Data.Data.name);
            temp = (Item)obj.AddComponent(t);
            temp?.InitItem(p_Data, p_En);
        }
        else
            temp = CreateItem(p_En, p_Data);
        if (null != temp)
            OnGetItem(temp, p_En, p_Data);
        return temp;
    }

    public static void Release(Item p_Item)
    {
        Current.InRelease(p_Item);
    }
    private void InRelease(Item p_Item)
    {
        p_Item.ReleaseItem();
        GameObject.Destroy(p_Item);
        if(m_ItemPool.Count >= m_PoolMaxCount)
        {
            GameObject.Destroy(p_Item.gameObject);
            return;
        }
        OnReleaseItem(p_Item);
        //p_Data.Data = null;
    }

    private Item CreateItem(Entity p_En, EntityEditorInvenItem p_Data)
    {
        Type t = Type.GetType(p_Data.Data.name);
        if (t == null)
        {
            Debug.Log(p_En.name + " 의 아이템 : " + p_Data.Data.name + "의 타입을 가져올수 없음");
            return null;
        }
        GameObject clone = GameObject.Instantiate(m_ItemUIPrefaps);
        Item item = (Item)clone.AddComponent(t);
        item?.InitItem(p_Data, p_En);

        return item;
    }

    private void OnGetItem(Item p_Item, Entity p_En, EntityEditorInvenItem p_Data)
    {
        p_Item.gameObject.SetActive(true);
        p_Item.transform.SetParent(null);
    }
    private void OnReleaseItem(Item p_Item)
    {
        p_Item.gameObject.SetActive(false);
        p_Item.transform.SetParent(m_PoolTran,false);
        p_Item.transform.localPosition = Vector3.zero;
        p_Item.name = m_PoolTran.childCount.ToString();

        m_ItemPool.Push(p_Item.gameObject);
    }
}
public static class ItemGeneratorExtensions
{
    public static void ReleaseItem(this Item p_Data)
    {
        ItemGenerator.Release(p_Data);
    }
}