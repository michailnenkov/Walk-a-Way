﻿using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections;

//public enum for use in ActiveTile and WorldGeneration as well
public enum Direction {North,South,East,West,None};

public class PlayerController : MonoBehaviour {
	
	//publics
	public float speed;
	public ActiveTile actTile;
	//privates
	private bool isSitting=false;
	private bool isInteracting=false;
	private int movementMode = 0;
	
	//get the collider component once, because the GetComponent-call is expansive
	void Awake()
	{
		//rigidbody.freezeRotation = true;//would push the sphere over the landscape
	}	
	
	
	void Update () 
	{
		// Cache the inputs.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
		// changed the Axis gravity&sensitivity to 1000, for more direct input.
		// for joystick usage however Vince told me to:
		/* just duplicate Horizontal and use gravity 1, dead 0.2 and sensitivity 1 that it works*/		
        bool sit = Input.GetButtonDown("Sit");
		bool interact = Input.GetButtonDown("Interact");
		
		if( sit )
		{
			if(!isSitting) 
			{
				//stop the motion
				//rigidbody.velocity = Vector3.zero;
				//rigidbody.angularVelocity = Vector3.zero;		
				//sit down
				gameObject.transform.localScale = new Vector3(1.0f,0.5f,1.0f);				
				gameObject.transform.Translate(new Vector3(0.0f,-0.25f,0.0f));
				isSitting = true;
			}
			else
			{
				//stand up again
				gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
				gameObject.transform.Translate(new Vector3(0.0f,0.25f,0.0f));
				isSitting = false;
			}
		}	
		
        if( Input.GetButtonDown("ToggleMovementMode"))
		{	movementMode++;
			if( movementMode > 2)
				movementMode = 0;
		}
		
		if ( !isSitting)
		{
			if( movementMode == 0) // DPAD mode
			{
				if( v > 0.05f )
					gameObject.transform.Translate(new Vector3(0.1f,0.0f,0.0f)*Time.deltaTime*speed);
				if( v < -0.05f )
					gameObject.transform.Translate(new Vector3(-0.1f,0.0f,0.0f)*Time.deltaTime*speed);
				if( h < -0.05f )
					gameObject.transform.Translate(new Vector3(0.0f,0.0f,0.1f)*Time.deltaTime*speed);
				if( h > 0.05f )
					gameObject.transform.Translate(new Vector3(0.0f,0.0f,-0.1f)*Time.deltaTime*speed);
			}
			else if( movementMode == 1) // diagonal mode version one
			{
				Vector3 move = new Vector3(0.0f,0.0f,0.0f);
				if( v > 0 )
				{
					move.x += 0.1f;
					move.z += 0.1f;
				}
				if( v < 0 )
				{
					move.x -= 0.1f;
					move.z -= 0.1f;
				}
				if( h < 0 )
				{
					move.x -= 0.1f;
					move.z += 0.1f;
				}
				if( h > 0 )
				{
					move.x += 0.1f;
					move.z -= 0.1f;
				}
				gameObject.transform.Translate(move*Time.deltaTime*speed);
			}
			else if( movementMode == 2) // diagonal mode 
			{
				Vector3 move = new Vector3(0.0f,0.0f,0.0f);
				if( v > 0 )
				{
					move.x = Mathf.Min(0.1f, move.x+0.1f);
					move.z = Mathf.Min(0.1f, move.z+0.1f);
				}
				if( v < 0 )
				{
					move.x =  Mathf.Max(-0.1f, move.x-0.1f);
					move.z =  Mathf.Max(-0.1f, move.z-0.1f);					
				}
				if( h < 0 )
				{
					move.x =  Mathf.Max(-0.1f, move.x-0.1f);
					move.z = Mathf.Min(0.1f, move.z+0.1f);
				}
				if( h > 0 )
				{
					move.x = Mathf.Min(0.1f, move.x+0.1f);
					move.z =  Mathf.Max(-0.1f, move.z-0.1f);					
				}
				// when moving diagnoal reduce walk speed by sqrt(2)
				if( move.x != 0.0f && move.z != 0.0f)
				{
					move.x /= Mathf.Sqrt( 2.0f );
					move.z /= Mathf.Sqrt( 2.0f );
				}
				gameObject.transform.Translate(move*Time.deltaTime*speed);
			}
		}	
	}
	
	void OnTriggerEnter (Collider other)
	{
		if( other.gameObject.tag == "NextTileTriggers")
		{
			// stop motion
			//rigidbody.velocity = Vector3.zero;
			//rigidbody.angularVelocity = Vector3.zero;
			
			//teleport to new position
			Direction dir=Direction.None;
			if( other.name == "WestTrigger")
			{
				dir = Direction.West;
				gameObject.transform.Translate(new Vector3((-other.gameObject.transform.position.x*2)-2.0f,0.0f,0.0f),Space.World); // move to east 
			}
			else if( other.name == "EastTrigger")
			{
				dir = Direction.East;
				gameObject.transform.Translate(new Vector3(-(other.gameObject.transform.position.x*2)+2.0f,0.0f,0.0f),Space.World); // move to west 
			}
			else if( other.name == "NorthTrigger")
			{
				dir = Direction.North;
				gameObject.transform.Translate(new Vector3(0.0f, 0.0f, -(other.gameObject.transform.position.z*2)+2.0f),Space.World); // move to south 
			}
			else if( other.name == "SouthTrigger")
			{
				dir = Direction.South;
				gameObject.transform.Translate(new Vector3(0.0f, 0.0f, (-other.gameObject.transform.position.z*2)-2.0f),Space.World); // move to south 
			}
			
			// update tile, pass the direction along
			actTile.showNextTile(dir);			
		}
	}
    void OnGUI()
    {
        speed = GUI.HorizontalSlider(new Rect(25, 35, 300, 10), speed, 0f, 500.0f);
        GUI.Label(new Rect(25,15,60,20), speed.ToString(CultureInfo.InvariantCulture));
		float test = GUI.HorizontalSlider(new Rect(25, 65, 50, 10), movementMode, 0.0f, 2.0f);
		GUI.Label(new Rect(25,45,150,20), "AlternateMoveStyle:");
		if( test > 1.5f) 
			movementMode = 2;
		else if( test > 0.5f)
			movementMode = 1;
		else
			movementMode = 0;
			
      //  GUI.Label(new Rect(25,15,60,20), movementMode.ToString(CultureInfo.InvariantCulture));
    }
}