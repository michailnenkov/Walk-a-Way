﻿using Assets.Scripts.InteractableBehaviour;
using UnityEngine;
using System.Collections;
using System.Linq;

public enum AnimalBehaviour{Ignore, Observe, Curious, Move, Flee}

public class RabbitGroupBehavior : ReactableBehaviour
{
	//public AnimalBehaviour Behaviour; //{ get; private set; }
	//private Vector3 initialFace;
	public float playerProgress;
	private Vector3 playerPos;

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

		if (distanceToPlayer < 10) {
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

					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(playerPos)); // Look at player
					
					// if (distanceToPlayer > 2) {
					// 	CurrentSpeed = Speed * -1;
					// } else {
					// 	// if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					// 	CurrentSpeed = 0;
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
					Debug.Log("Curious out of range");
					GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Bliss());

					// GetComponentsInChildren<RabbitMovement>().ToList().ForEach(e => e.Look(PlayerPos + transform.position)); // Look at player

					// if (distanceToPlayer > 5) {
					// 	CurrentSpeed = Speed * -1;
					// } else {
					// 	if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					// }					

					break;
				case AnimalBehaviour.Move:
					// Debug.Log("Move out of range");
					if (CurrentSpeed > 0) { CurrentSpeed += Decel; }
					break;
				case AnimalBehaviour.Flee:
					// Debug.Log("Flee out of range");
					break;
			}
		}

		Debug.Log(playerPos);
		Vector3 move = playerPos*-1;
		//move.y *= 0;

		move.Normalize();

		transform.position += (move * CurrentSpeed * Time.deltaTime);
	}
}
