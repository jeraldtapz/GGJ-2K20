using System;
using Core.Common;
using Gravity;
using Modules.General;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Units
{
    public class BaseUnit : InitializableBehaviour
    {
        [SerializeField] protected string unitId = string.Empty;
        [SerializeField] protected UnitGravityBody unitGravityBody = null;
        [SerializeField] private Vector3 leftScale = default;
        [SerializeField] private Vector3 rightScale = default;
        [SerializeField] private Image healthFill = null;

        public string UnitId => unitId;

        protected UnitData UnitData;
        private int currentHealth;
        public int CurrentHealth => currentHealth;
        private Subject<int> onHealthChanged;

        private Subject<BaseUnit> onDeath;
        public IObservable<BaseUnit> OnDeath => onDeath;
        public IObservable<int> OnHealthChanged => onHealthChanged;
        
        public Direction Direction { get; protected set; }
        public UnitRole Role => UnitData.UnitRole;

        public override void Initialize(object data = null)
        {
            currentHealth = UnitData.MaxHealth;
            OnHealthChange();
        }

        public virtual void SetData<T>(T data) where T: UnitData
        {
            UnitData = data;
            unitGravityBody.SetSpeed(UnitData.MovementSpeed);
            currentHealth = UnitData.MaxHealth;
            onHealthChanged = new Subject<int>();
            onDeath = new Subject<BaseUnit>();
            OnHealthChange();
        }

        public virtual void SetDirection(Direction direction)
        {
            unitGravityBody.SetDirection(direction);
            unitGravityBody.SetToMove();
            Direction = direction;

            transform.localScale = direction == Direction.Left ? leftScale : rightScale;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            OnHealthChange();
        }

        protected virtual void OnHealthChange()
        {
            healthFill.fillAmount = CurrentHealth / (float) UnitData.MaxHealth;
            onHealthChanged.OnNext(CurrentHealth);
        }

        [Button]
        private void GetId()
        {
            unitId = gameObject.name;
        }
        
    }
}
