using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Buff;

public class Buff_Fire : BuffBase
{
    public override void BuffActive()
    {
        Debug.Log("Fire");
        En.HitDamage(null, 10);
    }

    public override void BuffReset()
    {
        
    }
}