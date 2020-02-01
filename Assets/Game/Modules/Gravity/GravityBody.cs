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

        private void Start()
        {
            RigidBody.useGravity = false;
            myTransform = transform;
            
            RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public override void Initialize(object data = null)
        {
            base.Initialize(data);
            
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

        public void SetAttractor(GravityAttractor gravityAttractor)
        {
            attractor = gravityAttractor;
        }
    }
}