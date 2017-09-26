using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour {
	
	public enum Mechanic {Sitting, Travel, Interaction}
	public enum Values {Alpha, Speed, InertiaDuration, InertiaDistance, GreyPlayerColor, BackgroundColorFactor, CollisionSizePercent}
	public PlayerController player;
	public GameObject sun;
	public GameObject light;
	public bool StopTime = false;
	public GameObject ground;
	public GameObject wolf;
	private bool wolfSpawned = false;
	public float timer = 0;
	public float defaultTimerRate;
	public float timerRate;
	float sunRot = 0;
	public float progress= 0.0f;
	public float baseRateOfProgress = 0.1f;
	public float rateOfProgress = 0.1f;
	private float totalSittingTime = 0.0f; //100.0f for testing
	public float totalTilesTraveled = 1; // 45 for testing
	public float totalTilesVisited = 1; //number of unique tiles visited
	Dictionary<string, float> timeAtTile = new Dictionary<string, float>();

	
	// Use this for initialization
	void Start () {
	
		sun = GameObject.Find("Sun");
		light = GameObject.Find("DirectionalLight");

		InvokeRepeating("AddValue", 1, 0.01f); // function string, start after float, repeat rate float

		timerRate = defaultTimerRate;
	}
	
	void AddValue() {
		timer += timerRate;
		if (timer > 1.0f) {
			timer = 0;
		};
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateSun();
        TrackTimeOnTile();

		float timeOnTileMultipier = Mathf.InverseLerp(60,0,TimeOnTile());
		float timeOnTilePenalty = Mathf.Lerp(-1,1, timeOnTileMultipier);	

		float explorationMultiplier = totalTilesVisited/totalTilesTraveled;

		rateOfProgress = baseRateOfProgress * explorationMultiplier * timeOnTilePenalty * (totalSittingTime/240) + (totalTilesVisited/100000);
		// Debug.Log("baseRateOfProgress: " + baseRateOfProgress + " * explorationMultiplier: " + explorationMultiplier + "* timeOnTilePenalty: " + timeOnTilePenalty);


		if (player.isSitting && progress < 0.5f) {
			progress += rateOfProgress;

			timerRate = defaultTimerRate * rateOfProgress * 100000;
		} else if (player.isSitting && progress >= 0.5f) {

		} else {
			timerRate = defaultTimerRate;
		}

		if (StopTime) {
			timerRate = 0;
		}

		if (progress > 0.5 && !wolfSpawned) {
			if (timer < 0.12f || timer > 0.9f) {
				StartCoroutine(startSpawningWolf());
			}
		}

		//for testing only
		if (Input.GetKeyDown ("space")) {
			spawnWolf();
			wolfSpawned = true;
		}
    }


	IEnumerator startSpawningWolf() {
		wolfSpawned = true;
		Debug.Log("wolf started spawning");
		yield return new WaitForSeconds(Random.Range(10, 60));
		if (timer < 0.12f || timer > 0.9f) {
			Debug.Log("wolf spawned!");
			spawnWolf();
		} else {
			wolfSpawned = false;
			Debug.Log("wolf not spawned");
		}
		yield return null;
	}



    private void TrackTimeOnTile()
    {
        float temp = 0;
        string currentTile = ground.GetComponent<GroundGen>().CurrentTile();
        if (timeAtTile.TryGetValue(currentTile, out temp))
        {
            timeAtTile[currentTile] += Time.deltaTime;
        }
        else
        {
            timeAtTile.Add(currentTile, 0);
        }
    }
	private float TimeOnTile() {
		//Always current tile
		float temp = 0;
		string currentTile = ground.GetComponent<GroundGen>().CurrentTile();
		if (timeAtTile.TryGetValue(currentTile, out temp))
        {
            return temp;
        } else {
			return 0;
		}
	}

    private void UpdateSun()
    {
        sunRot = Mathf.Lerp(220.0f, 365.0f, timer);
        sun.transform.eulerAngles = new Vector3(0, 0, sunRot);
        //start dimming the light after 0.75 on the timer
        if (timer > 0.75)
        {
            light.GetComponent<Light>().intensity = Mathf.Lerp(0.0f, 2.0f, Mathf.InverseLerp(1.0f, 0.75f, timer));
        }
        //start increasing intensity of the light after between 0 and 0.25 on the timer
        if (timer < 0.25)
        {
            light.GetComponent<Light>().intensity = Mathf.Lerp(0.0f, 2.0f, Mathf.InverseLerp(0.0f, 0.25f, timer));
        }
    }

    public float getProgress()
	{
		return progress;
	}
	public void usedMechanic(Mechanic inMech, float inVal=0.0f)
	{
		switch(inMech)
		{
		    case Mechanic.Interaction:
		        //nearInteractionCounter++;
		        break;
		    case Mechanic.Sitting:		        
				totalSittingTime  += inVal;
		        break;
			case Mechanic.Travel:
		        totalTilesTraveled++;
		        break;		    
		    default:
		        Debug.Log("unknown Mechanic");
		        break;
		}		
	}
	public float getValue(Values inVal)
	{
		switch(inVal)
		{
			case Values.Alpha:
		        return Mathf.Min(1.0f, 0.3f+progress);
		    case Values.Speed:		
			
				Vector2[] speedValues = {
					new Vector2(0.0f, 45.0f), //fast in the beginning
					new Vector2(0.25f, 30.0f), //constant in the middle
					new Vector2(0.75f, 30.0f), //constant in the middle
					new Vector2(1.0f, 20.0f) //slow in the end
				};
				return multipointInterpolation(speedValues,progress);
			case Values.InertiaDuration:
		        return Mathf.Max(0.0f, 1.0f - progress*10.0f);
			case Values.InertiaDistance:
		        return Mathf.Max(0.0f, 0.1f - progress);
			case Values.GreyPlayerColor:			
		        return Mathf.Min(1.0f, -0.4f*progress+0.5f);
			case Values.BackgroundColorFactor:
				return linearInterpolationBetween(1.08f,0.7f,progress);
			case Values.CollisionSizePercent: // 0.25 = zero, 0.5 = one
				return linearInterpolationBetween(0.20f,0.5f,progress);
		    default:
		        Debug.Log("unknown Value");
		        break;
		}		
		
		return progress;
	}

	public float multipointInterpolation(Vector2[] inVal, float inProgress) 
	{
		// find 2 closest points in the array
		Vector2 lower = inVal[0];
		Vector2 higher = inVal[inVal.Length-1];
		foreach( Vector2 p in inVal)
		{	
			if( p.x == inProgress)
			{
				return p.y;
			}		
			if( p.x > lower.x && p.x < inProgress)
			{
				lower = p;
			}
			if( p.x < higher.x && p.x > inProgress)
			{
				higher = p;
			}
		}
		float tmp = linearInterpolationBetween(lower.x, higher.x, inProgress);

		return lower.y*(1-tmp) + higher.y*(tmp);
	}
	public float linearInterpolationBetween( float zeroTill, float oneAt, float t)
	{
		return Mathf.Max(0.0f, Mathf.Min(1.0f,(t-zeroTill) / (oneAt-zeroTill)));
	}

	public void spawnWolf() {
		float direction = Random.Range(0, 4);
		float x;
		float y;
		float z;
		GameObject thisWolf;
		if(direction<1) {
		//north
		x = 18;
		z = Random.Range(1, 18);
	}
	else if(direction<2) {
		//south
		x = 1;
		z = Random.Range(1, 18);
	}
	else if(direction<3) {
		//east
		x = Random.Range(1, 18);
		z = 1;
	}
	else {
		//west
		x = Random.Range(1, 18);
		z = 18;
	}
		y = ground.GetComponent<GroundGen>().returnGroundY(x, z);
		Vector3 wolfPosition = new Vector3(x, y, z);
		thisWolf = Instantiate(wolf, wolfPosition, Quaternion.identity);
		Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
		thisWolf.transform.LookAt(playerPos);
	}
}
