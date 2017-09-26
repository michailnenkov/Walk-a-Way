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
	bool doneWaiting = false;
	bool howled = false;
	bool backingUp = false;

	bool closeToFire = false;

	GameObject fire;

	bool touchingPlayer = false;

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

		if (fire != null) {
			float wolf2fireDistance;
			float player2fireDistance;

			wolf2fireDistance = Vector3.Distance(fire.transform.position, transform.position);
			player2fireDistance = Vector3.Distance(fire.transform.position, playerPos);

			if (wolf2fireDistance < 5 && player2fireDistance < 5) {
				Behaviour = WolfBehaviours.StayAway;
			}
		}

		Debug.Log(Behaviour);
		
        
		switch (Behaviour)
		{
			case WolfBehaviours.Ignore:
				doneWaiting = true;
				// Debug.Log("Ignore in range");
				break;
			case WolfBehaviours.Stalk:
				doneWaiting = true;
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
			
				if (distanceToPlayer > 1.2 && !backingUp)
				{
					FacePlayer();
					if (!howled) {
						StartCoroutine(WaitForStrike());
					}  
					if (!waiting){
						CurrentSpeed = 5;
					}
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

				break;
			
			case WolfBehaviours.StayAway:
				CurrentSpeed = 0;
				transform.RotateAround(fire.transform.position, Vector3.up, 10 * Time.deltaTime);
				transform.LookAt(fire.transform);
				transform.Rotate(0,-90,0);

			break;
			default:
				// Debug.Log("Default? in range");
				break;
		}
		

		transform.localPosition += (animalDirection * CurrentSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.name == "Player") {
			Debug.Log("touching player");
			touchingPlayer = true;
			if (!backingUp) {
				Debug.Log("killing player");
				GameObject.Find("Player").GetComponent<PlayerController>().Die();
			}
		}

		if (collider.name == "BurningCollider") {
			fire = collider.gameObject;
			closeToFire = true;
		}
	}

	void OnTriggerExit(Collider collider) {
		if (collider.name == "ObstacleCollider") {
			touchingPlayer = false;
			Debug.Log("stopped touching player");
		}

		if (collider.name == "BurningCollider") {
			closeToFire = false;
		}
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
		doneWaiting = true;
		yield return null;
	}

    IEnumerator BackUp() {
		backingUp = true;
		yield return new WaitForSeconds(1);
		backingUp = false;
		yield return null;
	}	

	IEnumerator WaitForStrike() {
		howled = true;
		waiting = true;
		CurrentSpeed = 0;
		GameObject.Find("AudioWolf").GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(1);
		CurrentSpeed = 1;
		yield return new WaitForSeconds(2);
		waiting = false;
		yield return null;
	}
}
