using Modules.CameraSystem;
using UnityEngine;
using Zenject;


namespace Gravity
{
    public class GravityAttractor : MonoBehaviour
    {
        [Inject] private GameCamera gameCamera = null;
        public float gravity = -10;
        
        public void Attract(Transform body, Rigidbody rBody)
        {
            Vector3 gravityUp = (body.position - transform.position);
            gravityUp = gravityUp.normalized;

            Vector3 bodyUp = body.up;
            rBody.AddForce(gravityUp * gravity);

            Quaternion rotation = body.rotation;
            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * rotation;
            rotation = Quaternion.Slerp(rotation, targetRotation, 50 * Time.deltaTime);
            body.rotation = rotation;
        }
    }
}