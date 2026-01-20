using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemScriptObj : ScriptableObject
{
    [SerializeField] public ItemStatus m_Data;
    public Sprite m_Image;
}