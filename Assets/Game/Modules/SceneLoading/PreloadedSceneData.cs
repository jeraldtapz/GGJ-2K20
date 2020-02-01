using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.SceneSystem
{
    [CreateAssetMenu(menuName = "FALLEN TEAR/Scene Data/Preloaded SceneData")]
    public class PreloadedSceneData : SerializedScriptableObject
    {
        public Dictionary<string, Scene> SceneMap;
    }
}