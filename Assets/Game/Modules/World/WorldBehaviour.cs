using System;
using System.Collections;
using System.Collections.Generic;
using Gravity;
using UnityEngine;

namespace Modules.World
{
    public class WorldBehaviour : GravityAttractor
    {
        [SerializeField] private GravityBody debugUnit = null;

        private void Awake()
        {
            debugUnit.SetAttractor(this);
        }
    }
}
