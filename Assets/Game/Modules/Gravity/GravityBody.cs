using System;
using System.Collections;
using System.Collections.Generic;
using Modules.General;
using UnityEngine;
using Zenject;

namespace Gravity
{
    public class GravityBody : InitializableBehaviour
    {
        private GravityAttractor attractor = null;
        private Transform myTransform;
        
        [SerializeField]
        protected Rigidbody RigidBody = null;

        public override void Initialize(object data = null)
        {
            RigidBody.useGravity = false;
            myTransform = transform;
            
            RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        protected virtual void Update()
        {
            if (attractor == null)
                return;
            
            if (attractor)
                attractor.Attract(myTransform, RigidBody);
        }

        protected void FixedUpdate()
        {
            
        }

        public void SetAttractor(GravityAttractor gravityAttractor)
        {
            attractor = gravityAttractor;
        }
    }
}