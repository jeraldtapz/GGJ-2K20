using UnityEngine;

namespace Modules.Tools
{
    public class DestroyOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}