using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public enum AnimalBehaviour{Ignore, Observe, Curious, Move, Flee}

public class RabbitGroupBehavior : ReactableBehaviour
{
	//public AnimalBehaviour Behaviour; //{ get; private set; }
	//private Vector3 initialFace;
	public float playerProgress;

	bool movingCloser = false;
	bool waiting = false;
	bool backingUp = false;

	private Vector3 playerPos;

	private Vector3 animalDirection;

    public RabbitGroupBehavior()
	{
	    //base();
	}


	// Use this for initialization
	void Start (){
	}
	
	// Update is called once per frame
	void Update () {

		playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

		//0 -> 1z, 0x
		//90 -> 0z, 1x
		//180 -> -1z, 0x
		//270 -> 0z, -1x
		//cos -> x
		//sin -> z

		playerProgress = GameObject.Find("Progression").GetComponent<ProgressManager>().progress;

		float distanceToPlayer = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

		if (distanceToPlayer < 6) {
			PlayerInRange = true;
		} else {
			PlayerInRange = false;
		}



        if (playerProgress < 0.1f)
        {
            Behaviour = AnimalBehaviour.Ignore;
        }
        else if (playerProgress < 0.25f)
        {
            Behaviour = AnimalBehaviour.Observe;
        }
        else if (playerProgress < 0.75f)
        {
            Behaviour = AnimalBehaviour.Curious;
        }
        else if (playerProgress < 0.9f)
        {
            Behaviour = AnimalBehaviour.Move;
        }
        else
        {
            Behaviour = AnimalBehaviour.Flee;
        }



		if (PlayerInRange)
		{
			switch (Behaviour)
			{
				case AnimalBehaviour.Ignore:
					// Debug.Log("Ignore in range");
					break;
				case AnimalBehaviour.Observe:
					// Debug.Log("observe in range");
					break;
                case AnimalBehaviour.Curious:
                    // Debug.Log("curious in range");
                    // Debug.Log(distanceToPlayer);

                    if (distanceToPlayer > 3 && !waiting && !backingUp)
                    {
						// Debug.Log("moving closer");
						FacePlayer();
						//move closer
                        CurrentSpeed = Speed;
                        movingCloser = true;
                    }
                    else
                    {
                        CurrentSpeed = 0;
                        if (movingCloser)
                        { // if just finished coming closer, wait a little bit before following again
                            movingCloser = false;
                            StartCoroutine("WaitInPlace");
                        }
                    }

                    if (distanceToPlayer < 1.5f && !backingUp)
                    {
						// Debug.Log("too close!");
						
						StartCoroutine("BackUp");
                    }

					if (backingUp) {
						FaceAway();
						CurrentSpeed = Speed;
					}



                    // if (distanceToPlayer < 1.5f) {
                    // 	GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.transform.LookAt(playerPos*-1));
                    // 	CurrentSpeed = Speed;
                    // }
                    break;
                case AnimalBehaviour.Move:
					// Debug.Log("Move in range");

					break;
				case AnimalBehaviour.Flee:
					// Debug.Log("Flee in range");
                    //GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(playerPos * -1 + transform.position)); // Look away from player and run
					//CurrentSpeed = Speed;
					break;
				default:
					// Debug.Log("Default? in range");
					break;
			}
		}
		else
		{
			switch (Behaviour)
			{
				case AnimalBehaviour.Ignore:
					// Debug.Log("Ignore out of range");
					break;
				case AnimalBehaviour.Observe:
					// Debug.Log("Observe out of range");
					break;
                case AnimalBehaviour.Curious:
					// Debug.Log("Curious out of range");


					//GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Bliss());
					gameObject.GetComponent<RabbitMovement>().Bliss();

					// GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos + transform.position)); // Look at player

					// if (distanceToPlayer > 5) {
					// 	CurrentSpeed = Speed * -1;
					// } else {
					// 	if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					// }					

					break;
				case AnimalBehaviour.Move:
					// Debug.Log("Move out of range");
					//if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					break;
				case AnimalBehaviour.Flee:
					// Debug.Log("Flee out of range");
					break;
			}
		}

		// Debug.Log(playerPos);
		// Vector3 move = playerPos*-1;
		//move.y *= 0;

		//move.Normalize();

		// transform.position += (move * CurrentSpeed * Time.deltaTime);
		// transform.localPosition += (animalDirection * CurrentSpeed * Time.deltaTime);
		transform.localPosition += (animalDirection * CurrentSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
	}

    private void FacePlayer()
    {
        GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(playerPos)); // Look at player		
        // Vector3 horizontalPlayerPos = playerPos;
        // horizontalPlayerPos.y = transform.position.y;
        // transform.LookAt(horizontalPlayerPos);
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
		yield return new WaitForSeconds(Random.Range(2.0f, 6.0f));
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
