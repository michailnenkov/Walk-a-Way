using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProgressBar : MonoBehaviour {

	Image progressBar;
	public ProgressManager manager;
	public float progress;

	// Use this for initialization
	void Start () {
		progressBar = GetComponentInParent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		progressBar.fillAmount = manager.progress;
	}
}
