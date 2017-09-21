using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InteractableBehaviour
{
    public abstract class ActableBehaviour : MonoBehaviour {
        public float triggerRadius=1.0f;
	
        //public abstract void customAwake();
        //public abstract CarryObject activate(float playerProgress);

        protected Component GetGhildComponent(string Name)
        {
            return GetComponentsInChildren<Component>().First(c => c.name == Name);
            //return head;
        }
    }

    public abstract class ReactableBehaviour : ActableBehaviour
    {
        protected AnimalBehaviour Behaviour;
        public bool PlayerInRange = false;
        public Vector3 PlayerPos; // { get; set; }
        public float Speed = 3.5f;
        public float Decel = -0.1f;
        public float CurrentSpeed = 0f;

        protected ReactableBehaviour()
        {
            Behaviour = AnimalBehaviour.Ignore; 
        }

        public virtual void React(float playerProgress, Vector3 playerPos)
        {

            PlayerInRange = true;
            PlayerPos = playerPos;
        }

        public void Deactivate()
        {
            PlayerInRange = false;
        }
    }

    public abstract class InteractableBehaviour : ActableBehaviour
    {
        public abstract CarryObject Activate(float playerProgress, Vector3 playerPos);
        public abstract string customInteractiveText();
        
    }
}
