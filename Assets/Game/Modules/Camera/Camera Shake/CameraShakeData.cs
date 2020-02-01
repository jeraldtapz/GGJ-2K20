using UnityEngine;

namespace Modules.CameraSystem
{
    [CreateAssetMenu(menuName = "FALLEN TEAR/Camera/Camera Shake Data")]
    public class CameraShakeData : ScriptableObject
    {
        public float Frequency;
        public float Amplitude;
        public float Duration;
    }
}