using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Sword : Item
{
    public override bool ActiveItem()
    {
        Debug.Log(m_Owner.name+ " : " + nameof(Item_Sword) + " | »ç¿ë");
        m_Owner.Target.HitDamage(m_Owner, this);
        return true;
    }
}