using Sirenix.OdinInspector;
using UnityEngine;
// ReSharper disable NotAccessedField.Global

namespace Modules.General
{
    public class Entity : SerializedMonoBehaviour
    {
        [TabGroup("Data", "Entity")]
        public string Name;

        [TabGroup("Data", "Entity")]
        public string Description;
    }
}