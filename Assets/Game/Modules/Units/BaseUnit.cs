using Gravity;
using Modules.General;
using UnityEngine;

namespace Modules.Units
{
    public class BaseUnit : MonoBehaviour
    {
        [SerializeField] private UnitGravityBody unitGravityBody = null;
        [SerializeField] private Vector3 leftScale = default;
        [SerializeField] private Vector3 rightScale = default;
        public Direction Direction { get; protected set; }

        public virtual void SetDirection(Direction direction)
        {
            unitGravityBody.SetDirection(direction);
            Direction = direction;

            transform.localScale = direction == Direction.Left ? leftScale : rightScale;
        }
    }
}
