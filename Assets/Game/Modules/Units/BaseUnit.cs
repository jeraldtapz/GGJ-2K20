using System;
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
        protected int CurrentHealth;
        
        public Direction Direction { get; protected set; }
        public UnitRole Role => UnitData.UnitRole;

        public override void Initialize(object data = null)
        {
            OnHealthChange();
        }

        public virtual void SetData<T>(T data) where T: UnitData
        {
            UnitData = data;
            unitGravityBody.SetSpeed(UnitData.MovementSpeed);
            CurrentHealth = UnitData.MaxHealth;
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
            CurrentHealth -= damage;
            OnHealthChange();
        }
        

        protected virtual void OnHealthChange()
        {
            healthFill.fillAmount = CurrentHealth / (float) UnitData.MaxHealth;
        }


        [Button]
        private void GetId()
        {
            unitId = gameObject.name;
        }
        
    }
}
