﻿using UnityEngine;

namespace Modules.Units
{
    [CreateAssetMenu(menuName = "GGJ/Unit Data")]
    public class UnitData : ScriptableObject
    {
        public UnitRole UnitRole;
        public string Name;
        public int Level;
        public int Price;
        public int MaxHealth;
        public int MovementSpeed;
        public Sprite Icon;
    }
}