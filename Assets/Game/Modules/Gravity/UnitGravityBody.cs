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
        protected bool ShouldMove = false;
        protected Transform ReferenceTransform = null;

        public bool IsMoving { get; protected set; } = false;


        public override void Initialize(object data = null)
        {
            base.Initialize(data);

            ShouldMove = true;
        }

        protected override void Update()
        {
            base.Update();

            Vector3Cache.x = speed * (direction == Direction.Right ? 1 : -1);
        }

        protected virtual void FixedUpdate()
        {
            if (!ShouldMove)
                return;
            // transform.rotation *= Quaternion.identity;
            Vector3 worldMove = transform.TransformDirection(Vector3Cache) * Time.fixedDeltaTime;
            RigidBody.MovePosition(RigidBody.position + worldMove);
        }

        public void SetDirection(Direction dir)
        {
            direction = dir;
        }

        public void SetSpeed(float moveSpeed)
        {
            speed = moveSpeed;
        }

        public void SetReference(Transform tForm)
        {
            ReferenceTransform = tForm;
        }

        public void SetToMove()
        {
            ShouldMove = true;
            IsMoving = true;
        }

        public void SetToStop()
        {
            ShouldMove = false;
            IsMoving = false;
        }
    }
}