using UnityEngine;

namespace Modules.Pooling
{
    public class AttackerPoolable : Poolable
    {
        public override void Spawn()
        {
            base.Spawn();
            
            // transform.rotation = Quaternion.identity;
        }
    }
}