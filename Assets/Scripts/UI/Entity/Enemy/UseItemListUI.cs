using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//이번턴에 사용할 아이템
public class UseItemListUI : MonoBehaviour
{

    public Queue<Item> m_UseItemList = new Queue<Item>();
    GameObject m_UIPrefap = null;
    GameObject UIPrefap
    {
        get
        {
            if (null == m_UIPrefap)
                m_UIPrefap = (GameObject)ResourceManager.ResourceLoad(ResourceType.Prefaps, "EnemyItemUI");
            return m_UIPrefap;
        }
    }

    public void Init()
    {
        m_UseItemList.Clear();
        for(int i=0;i<transform.childCount;i++)
        {
            GameObject.Destroy(transform.GetChild(0).gameObject);
        }
    }
    /// <summary>
    /// 적들이 쓰는 아이템 생성및 추가
    /// </summary>
    /// <param name="p_ItemData"></param>
    public void AddItem(EntityEditorInvenItem p_ItemData)
    {
        Type t = Type.GetType(p_ItemData.Data.name);
        if (t == null)
        {
            Debug.Log(name + " 의 아이템 : " + p_ItemData.Data.name + "의 타입을 가져올수 없음");
            return;
        }
        GameObject clone = GameObject.Instantiate(UIPrefap);
        Item item = (Item)clone.AddComponent(t);

        item.InitItem(p_ItemData.Data, transform.parent.GetComponent<Entity>());
        clone.transform.SetParent(transform);
        clone.name = item.name;

        Image image = clone.GetComponent<Image>();
        image.sprite = item.m_Sprite;
        m_UseItemList.Enqueue(item);
    }
    public void RemoveItem()
    {
        
        m_UseItemList.Dequeue();
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }
}