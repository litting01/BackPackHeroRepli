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
    public void AddItem(Item item)
    {
        m_UseItemList.Enqueue(item);
        GameObject clone = GameObject.Instantiate(UIPrefap);
        clone.transform.SetParent(transform);
        clone.name = item.m_Name;

        Image image = clone.GetComponent<Image>();
        image.sprite = item.m_Sprite;

    }
    public void RemoveItem()
    {
        
        m_UseItemList.Dequeue();
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }
}