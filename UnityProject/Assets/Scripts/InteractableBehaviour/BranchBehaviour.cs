using System.Globalization;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.InteractableBehaviour{
	public class BranchBehaviour : InteractableBehaviour {
		PlayerController player;
        bool branchPicked;

        // Use this for initialization
        void Start()
        {
			player = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override CarryObject Activate(float playerProgress, Vector3 playerPos)
        {
            if(!branchPicked & playerProgress <= 0.5f)
            {
                return CarryObject.Nothing;
            }

            if (!branchPicked && playerProgress > 0.5f)
            {
                branchPicked = true;

                player.AddToInventory("branch");

                gameObject.SetActive(false);

                return CarryObject.Branch;

            }

            return CarryObject.Nothing;
        }

        public override string customInteractiveText()
        {
            //Press E
            return "to pick up the branch";
        }
	}
}	