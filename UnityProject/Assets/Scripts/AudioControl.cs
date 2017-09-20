using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool toggleMusic = Input.GetButtonDown("ToggleMusic");
		if( toggleMusic )
		{
			AudioSource test = GetComponent<AudioSource>();
			if (test.GetComponent<AudioSource>().isPlaying) 
			{
				test.GetComponent<AudioSource>().Stop();
				Debug.Log("Stop Music");
			}
			else
			{
				test.GetComponent<AudioSource>().Play();
				Debug.Log("Play Music");
			}
		}
	}
}
