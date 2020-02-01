using System;
using Core.Common;
using Modules.General;
using UnityEngine;

namespace Modules.Towers
{
    public class TowerBehaviour : InitializableBehaviour
    {
        [SerializeField] private int initialHealth = 100;

        private int currentHealth;
        public int CurrentHealth => currentHealth;

        private Subject<int> onHealthChanged;
        private IObservable<int> OnHealthChanged => onHealthChanged;

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            onHealthChanged = new Subject<int>();
            currentHealth = initialHealth;
        }

        public void TakeDamage(int damage)
        {
            if (currentHealth >= 0)
            {
                currentHealth -= damage;
            }
        }

        public void Repair(int repair)
        {
            if (currentHealth < 100)
            {
                currentHealth += repair;
            }
        }
    }
}