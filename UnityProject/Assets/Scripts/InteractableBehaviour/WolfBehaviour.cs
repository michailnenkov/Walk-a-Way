using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public enum WolfBehaviours{Ignore, Stalk, Kill, StayAway}

public class WolfBehaviour : ReactableBehaviour
{
	//public WolfBehaviours Behaviour; //{ get; private set; }
	//private Vector3 initialFace;
	public float playerProgress;

	bool movingCloser = false;
	bool waiting = false;
	bool backingUp = false;

	public float stalkingSpeed = 2;
	public float stalkingDistance = 10;

	private Vector3 playerPos;

	new private WolfBehaviours Behaviour;

	private GameObject ground;

	private Vector3 animalDirection;

	// Use this for initialization
	void Start (){
		ground = GameObject.Find("GroundTile");
	}
	
	// Update is called once per frame
	void Update () {

		playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

		playerProgress = GameObject.Find("Progression").GetComponent<ProgressManager>().progress;

		float distanceToPlayer = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

		var y = ground.GetComponent<GroundGen>().returnGroundY(transform.position.x, transform.position.z);

		transform.position = new Vector3(transform.position.x, y, transform.position.z);

		// if (distanceToPlayer < 6) {
		// 	PlayerInRange = true;
		// } else {
		// 	PlayerInRange = false;
		// }



        if (playerProgress < 0.5f)
        {
            Behaviour = WolfBehaviours.Ignore;
        }
        else if (playerProgress < 0.75f)
        {
            Behaviour = WolfBehaviours.Stalk;
        }
        else
        {
            Behaviour = WolfBehaviours.Kill;
        }
        
		// if (false) {
		//	//if too close to fire 
		// 	Behaviour = WolfBehaviours.StayAway;
		// }


		switch (Behaviour)
		{
			case WolfBehaviours.Ignore:
				// Debug.Log("Ignore in range");
				break;
			case WolfBehaviours.Stalk:

				if (distanceToPlayer > stalkingDistance && !waiting && !backingUp)
				{
					FacePlayer();
					//move closer
					CurrentSpeed = stalkingSpeed;
					movingCloser = true;
				}
				else
				{
					//if too close, or waiting, or backing up, stop
					CurrentSpeed = 0;
					FacePlayer();
					if (movingCloser)
					{ // if just finished coming closer, wait a little bit before following again
						movingCloser = false;
						StartCoroutine("WaitInPlace");
					}
				}

				if (distanceToPlayer < 3f && !backingUp)
				{
					// Debug.Log("too close!");
					
					StartCoroutine("BackUp");
				}

				if (backingUp) {
					FaceAway();
					CurrentSpeed = Speed;
				}

				break;
			case WolfBehaviours.Kill:
				// Debug.Log("Move in range");

				break;
			case WolfBehaviours.StayAway:
				
			break;
			default:
				// Debug.Log("Default? in range");
				break;
		}
		

		transform.localPosition += (animalDirection * CurrentSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
	}

    private void FacePlayer()
    {
        GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(playerPos)); // Look at player		
		animalDirection = transform.forward;
    }

	private void FaceAway()
    {
        GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(playerPos)); // Look at player
		transform.Rotate(0,180,0);
        // Vector3 horizontalPlayerPos = playerPos;
        // horizontalPlayerPos.y = transform.position.y;
        // transform.LookAt(horizontalPlayerPos);
		animalDirection = transform.forward;

    }

    IEnumerator WaitInPlace() {
		waiting = true;
		yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
		waiting = false;
		yield return null;
	}

    IEnumerator BackUp() {
		backingUp = true;
		yield return new WaitForSeconds(1);
		backingUp = false;
		yield return null;
	}	
}
