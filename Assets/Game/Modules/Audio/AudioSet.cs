using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.Audio
{
    [CreateAssetMenu(menuName = "FALLEN TEAR/Audio/Audio Set")]
    public class AudioSet : SerializedScriptableObject
    {
        public string AudioSetName;
        
        public Dictionary<string, AudioClip> BgmMap;
        public Dictionary<string, AudioClip> SfxMap;
    }
}