using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DebugScript : MonoBehaviour {

	public GameObject panelDebug;
	public Text debugText;

	private float time;

	private bool debugIsShown = true;
	// Use this for initialization
	void Start () {
		time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (debugIsShown) {
			time += Time.deltaTime;
			if(time > 1.5f){
				time = 0.0f;
				debugIsShown = false;
				debugText.gameObject.SetActive(false);
			}
		}
	}

	public void SetUserDetected(bool isDetected){
		if (isDetected) {
			this.panelDebug.SetActive(true);
		} else {
			this.panelDebug.SetActive(false);
		}
	}

	public void SetMessage(string message){
		debugText.gameObject.SetActive (true);
		this.debugText.text = message;
		debugIsShown = true;
	}
}
