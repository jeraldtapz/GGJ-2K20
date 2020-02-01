using System;
using Modules.General;
using UnityEngine;

namespace Gravity
{
    public class UnitGravityBody : GravityBody
    {
        [SerializeField] private float speed = 4f;

        private Direction direction = Direction.Right;

        protected Vector3 Vector3Cache = default;

        protected override void Update()
        {
            base.Update();

            Vector3Cache.x = speed * (direction == Direction.Right ? 1 : -1);
        }

        protected virtual void FixedUpdate()
        {
            Vector3 worldMove = transform.TransformDirection(Vector3Cache) * Time.fixedDeltaTime;
            RigidBody.MovePosition(RigidBody.position + worldMove);
        }

        public void SetDirection(Direction dir)
        {
            direction = dir;
        }
    }
}