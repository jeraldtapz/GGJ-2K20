using System;
using UnityEngine;
using Zenject;

namespace Modules.CameraSystem
{
    public class CameraRotator : MonoBehaviour
    {

        [SerializeField]
        [Range(10, 180)]
        private float rotationSpeed = 45f;

        [SerializeField] private Vector3 cameraInitialOffset = default;

        [Inject] private readonly GameCamera gameCamera = null;

        private void Awake()
        {
            gameCamera.transform.parent = transform;
            gameCamera.transform.position = cameraInitialOffset;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, Vector3.forward);
            }
            
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, Vector3.back);
            }
        }
    }
}
