using System;
using Core.Common;
using Modules.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Towers
{
    public class TowerBehaviour : InitializableBehaviour
    {
        [SerializeField] private string towerId = string.Empty;
        [SerializeField] private int initialHealth = 100;
        [SerializeField] private Direction towerDirection = Direction.Left;
        [SerializeField] private Image healthFill = null;
        
        [ShowInInspector]
        private int currentHealth;
        public int CurrentHealth => currentHealth;

        private Subject<int> onHealthChanged;
        public IObservable<int> OnHealthChanged => onHealthChanged;

        public string TowerId => towerId;

        public Direction TowerDirection => towerDirection;

        public override void Initialize(object data = null)
        {
            onHealthChanged = new Subject<int>();
            currentHealth = initialHealth;
            healthFill.fillAmount = currentHealth / 100.0f;
        }

        public void TakeDamage(int damage)
        {
            if (currentHealth >= 0)
            {
                currentHealth -= damage;
                onHealthChanged.OnNext(currentHealth);
                healthFill.fillAmount = currentHealth / 100.0f;
            }
        }

        public void Repair(int repair)
        {
            if (currentHealth < 100)
            {
                currentHealth += repair;
                onHealthChanged.OnNext(currentHealth);
                healthFill.fillAmount = currentHealth / 100.0f;
            }
        }
    }
}