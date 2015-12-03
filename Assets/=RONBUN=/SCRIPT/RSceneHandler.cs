using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;

public class RSceneHandler : MonoBehaviour {
	public GameObject ninshikiPanel;
	public GameObject tutePanel;
	public GameObject workSpaceGobj;
	public GameObject[] listOfPrefabs;

	private bool workSpaceGobjIsSelected = false;

	public static List<GameObject> currentGameObjectsInScene = new List<GameObject>();    

	public static int currentChosenIndex = 0;

	public Material draggedObjectMaterial;

	private Material defaultObjectMaterial;

	public static RGestureListener.GESTURE_IDENTITY currentGesture = RGestureListener.GESTURE_IDENTITY.MOVE;

	// interaction manager reference
	private InteractionManager manager;

	private UDP_RecoServer udpReco;
	// Use this for initialization
	void Start () {

		this.udpReco = this.gameObject.GetComponent<UDP_RecoServer> ();

		//this.gameObject.GetComponent<RGestureListener> ().ChangeGesture (RGestureListener.GESTURE_IDENTITY.MOVE);
//		GameObject temp = (GameObject)Instantiate (listOfPrefabs [ConstantScript.GOBJ_CUBE], new Vector3 (0, 0, 0), Quaternion.identity);
//		GameObject temp1 = (GameObject)Instantiate (listOfPrefabs [ConstantScript.GOBJ_CYLINDER], new Vector3 (1, 0, 0), Quaternion.identity);
//		GameObject temp2 = (GameObject)Instantiate (listOfPrefabs [ConstantScript.GOBJ_SPHERE], new Vector3 (2, 0, 0), Quaternion.identity);
//		currentGameObjectsInScene.Add(temp);
//
//		HighlightSelectedGameObject ();
//		currentGameObjectsInScene.Add(temp1);
//		currentGameObjectsInScene.Add(temp2);
	}

	private bool IsListening = false;
	
	private void SpeechDetect(){
		string speechDetected = this.udpReco.UDPGetPacket ();

		this.udpReco.ResetPacket ();
		if (speechDetected.Length > 0) {
			Debug.Log ("speech = "+speechDetected);
		
			if(IsListening == false){
				if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_START)){
					this.gameObject.GetComponent<DebugScript>().SetMessage("音声認識起動");
					IsListening = true;
					this.ninshikiPanel.SetActive(true);
				}
			}
			else if(IsListening == true){
				if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_MOVE)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("移動、聞きました");
					this.gameObject.GetComponent<RGestureListener>().SendMessage("ChangeGesture",RGestureListener.GESTURE_IDENTITY.MOVE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_ROTATE)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("回転、聞きました");
					this.gameObject.GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.ROTATE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_SCALE)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("拡大、聞きました");
					this.gameObject.GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.SCALE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_CUBE1) || speechDetected.Equals(ConstantScript.SPEECH_COMMAND_CUBE2)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("はこ、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CUBE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_SPHERE1) || speechDetected.Equals(ConstantScript.SPEECH_COMMAND_SPHERE2)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("スフィア、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_SPHERE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_CYLINDER1) || speechDetected.Equals(ConstantScript.SPEECH_COMMAND_CYLINDER2)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("シリンダー、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CYLINDER);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_PLANE1) || speechDetected.Equals(ConstantScript.SPEECH_COMMAND_PLANE2)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("プレーン、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_ALL)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("全部、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().SelectAllGameObject();
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_DELETE)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("削除、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().DeleteGameObject();
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_VIEW)){
					
					this.gameObject.GetComponent<RSceneHandler>().ViewGameObject();
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_YES)){
					
					this.CloseTute();
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_NEXT)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("つぎ、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().ChangeGobjTo(true);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_PREV)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("まえ、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().ChangeGobjTo(false);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_HEAD1) || speechDetected.Equals(ConstantScript.SPEECH_COMMAND_HEAD2)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("ヘッド、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_HEAD);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_BODY)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("体、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_BODY);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_LARM)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("左腕、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_LARM);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_RARM)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("右腕、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_RARM);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_LLEG)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("左足、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_LLEG);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_RLEG)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("右足、聞きました");
					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_RLEG);
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
				else if(speechDetected.Equals(ConstantScript.SPEECH_COMMAND_TUTE)){
					
					this.gameObject.GetComponent<DebugScript>().SetMessage("説明、聞きました");
					OpenTute();
					IsListening = false;
					this.ninshikiPanel.SetActive(false);
					//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
				}
			}


		}

	}

	bool isTuteOpen = false;
	void OpenTute(){
		if (isTuteOpen == false) {
			isTuteOpen = true;

			this.tutePanel.SetActive(true);
		}
	}

	void CloseTute(){
		if (isTuteOpen) {

			isTuteOpen = false;
			this.tutePanel.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.R)) {
			InputTracking.Recenter();
		}

		SpeechDetect ();
		//Debug.Log ("from update = "+this.udpReco.UDPGetPacket());

		// get the interaction manager instance
		if(manager == null)
		{
			manager = InteractionManager.Instance;
		}

		if (currentGesture == RGestureListener.GESTURE_IDENTITY.MOVE && currentGameObjectsInScene.Count > 0) {
			if(manager != null && manager.IsInteractionInited())
			{
				if(manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Grip){
					Vector3 screenNormalPos = Vector3.zero;
//					Vector3 screenPixelPos = Vector3.zero;
					
					if(manager.IsLeftHandPrimary())
					{
						Debug.Log("start, lefthand primary");
						screenNormalPos = manager.GetLeftHandScreenPos();
					}
//					if(manager.IsRightHandPrimary())
//					{
//						Debug.Log("start, righthand primary");
//						screenNormalPos = manager.GetRightHandScreenPos();
//					}

					Debug.Log("normal pos x = "+screenNormalPos.x+" y = "+screenNormalPos.y+" z = "+screenNormalPos.z);
					// convert the normalized screen pos to 3D-world pos
//					screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
//					screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
//					screenPixelPos.z = (currentGameObjectsInScene[currentChosenIndex].transform.position.z - Camera.main.transform.position.z);
//					Vector3 newObjectPos = Camera.main.ScreenToWorldPoint(screenPixelPos);
					//Debug.Log("newobjectpos");
					//currentGameObjectsInScene[currentChosenIndex].transform.position = Vector3.Lerp(currentGameObjectsInScene[currentChosenIndex].transform.position, newObjectPos, 3.0f * Time.deltaTime);
					
//					this.MoveSelectedGameObject(Vector3.Lerp(currentGameObjectsInScene[currentChosenIndex].transform.position, newObjectPos, 3.0f * Time.deltaTime));
					this.MoveSelectedGameObject(screenNormalPos,Time.deltaTime);
				}

			}
		}
		else if (currentGesture == RGestureListener.GESTURE_IDENTITY.ROTATE && currentGameObjectsInScene.Count > 0) {
			if(manager != null && manager.IsInteractionInited())
			{
				if(manager.GetLastLeftHandEvent() == InteractionManager.HandEventType.Grip){
					Vector3 screenNormalPos = Vector3.zero;
//					Vector3 screenPixelPos = Vector3.zero;
					
					if(manager.IsLeftHandPrimary())
					{
						Debug.Log("start, lefthand primary");
						screenNormalPos = manager.GetLeftHandScreenPos();
					}
					if(manager.IsRightHandPrimary())
					{
						Debug.Log("start, righthand primary");
						screenNormalPos = manager.GetRightHandScreenPos();
					}

//					screenPixelPos.x = (int)(screenNormalPos.x * Camera.main.pixelWidth);
//					screenPixelPos.y = (int)(screenNormalPos.y * Camera.main.pixelHeight);
//					screenPixelPos.z = currentGameObjectsInScene[currentChosenIndex].transform.rotation.eulerAngles.z;
					//Debug.Log("hand pos x = "+screenPixelPos.x+" y = "+screenPixelPos.y+" z = "+screenPixelPos.z);
					RotateSelectedGameObject(screenNormalPos,1);
				}
			}
		}
	}

	public void RotateSelectedGameObject(Vector3 handPos,int flag){
		GameObject currentGobj;
		if (workSpaceGobjIsSelected) {
			currentGobj = workSpaceGobj;
		} else {
			currentGobj = currentGameObjectsInScene[currentChosenIndex];
		}
		Vector3 screenPixelPos = Vector3.zero;
		screenPixelPos.x = (int)(handPos.x * Camera.main.pixelWidth);
		screenPixelPos.y = (int)(handPos.y * Camera.main.pixelHeight);
		screenPixelPos.z = handPos.z;

		Vector3 currentRotate = currentGobj.transform.rotation.eulerAngles;
		if (flag == 1) {
			currentRotate.x = screenPixelPos.x;
			currentRotate.y = screenPixelPos.y;
		} 
		else if (flag == 2) {

			currentRotate.z = screenPixelPos.z;
		}

		currentGobj.transform.eulerAngles = currentRotate;
	}

	public void MoveSelectedGameObject(Vector3 handPos,float deltatime){
		Vector3 screenPixelPos = Vector3.zero;
		GameObject tempCurrent;
		if (workSpaceGobjIsSelected) {
			tempCurrent = workSpaceGobj;
		} else {
			tempCurrent = currentGameObjectsInScene[currentChosenIndex];
		}

		screenPixelPos.x = (int)(handPos.x * Camera.main.pixelWidth);
		screenPixelPos.y = (int)(handPos.y * Camera.main.pixelHeight);
		screenPixelPos.z = 10.0f;
		Vector3 newObjectPos = Camera.main.ScreenToWorldPoint(screenPixelPos);

		tempCurrent.transform.position = Vector3.Lerp(tempCurrent.transform.position, newObjectPos, 3.0f * deltatime);
	}
	
	public void ScaleSelectedGameObject(float scale){
		GameObject tempCurrent;
		if (workSpaceGobjIsSelected) {
			tempCurrent = workSpaceGobj;
		} else {
			tempCurrent = currentGameObjectsInScene[currentChosenIndex];
		}
		tempCurrent.transform.localScale = new Vector3(scale,scale,scale);
	}

	public void SelectAllGameObject (){
		workSpaceGobjIsSelected = true;
		ResetHighlight ();
	}

	public void DeleteGameObject (){
		if (currentGameObjectsInScene.Count > 0) {

			ResetHighlight ();
			if(workSpaceGobjIsSelected){
				foreach(GameObject temp in currentGameObjectsInScene){
					Destroy(temp);
				}
				workSpaceGobjIsSelected = false;
				currentGameObjectsInScene.RemoveRange(0,currentGameObjectsInScene.Count);
				currentChosenIndex = 0;
			}
			else{
				GameObject temp = currentGameObjectsInScene[currentChosenIndex];
				Destroy(temp);
				currentGameObjectsInScene.RemoveAt(currentChosenIndex);
				currentChosenIndex = 0;
				if(currentGameObjectsInScene.Count > 0){
					HighlightSelectedGameObject ();
				}
			}

		}
	}

	public void ViewGameObject(){
		
	}

	public void CreateGameObject (int gobjIndex){
		GameObject temp = (GameObject)Instantiate (this.listOfPrefabs [gobjIndex], new Vector3 (0,0,0), Quaternion.identity);

//		if (gobjIndex == ConstantScript.GOBJ_BODY || 
//			gobjIndex == ConstantScript.GOBJ_HEAD || 
//			gobjIndex == ConstantScript.GOBJ_LARM ||
//			gobjIndex == ConstantScript.GOBJ_RARM ||
//			gobjIndex == ConstantScript.GOBJ_LLEG ||
//			gobjIndex == ConstantScript.GOBJ_RLEG) {
//			Vector3 scaleTemp = temp.transform.localScale;
//			scaleTemp.x = 50.0f;
//			scaleTemp.y = 50.0f;
//			scaleTemp.z = 50.0f;
//			temp.transform.localScale = scaleTemp;
//
//			temp.transform.Rotate(new Vector3(0,-180.0f,0));
//	
//		}

		temp.transform.parent = this.workSpaceGobj.transform;
		currentGameObjectsInScene.Add (temp);

		if (currentGameObjectsInScene.Count == 1) {
			HighlightSelectedGameObject ();
		}
	}

	public void ChangeGobjTo(bool isRight){

		if (currentGameObjectsInScene.Count > 0) {
			workSpaceGobjIsSelected = false;
			//to left
			if (isRight == false) {
				if(currentChosenIndex - 1 >= 0){
					currentChosenIndex--;
				}
				else if(currentChosenIndex -1 < 0){
					currentChosenIndex = currentGameObjectsInScene.Count-1;
				}
				
			} 
			//to right
			else if (isRight == true) {
				if(currentChosenIndex + 1 <= currentGameObjectsInScene.Count-1){
					currentChosenIndex++;
				}
				else if(currentChosenIndex + 1 > currentGameObjectsInScene.Count-1){
					currentChosenIndex = 0;
				}
			}
			Debug.Log("current index = "+currentChosenIndex);
			ResetHighlight ();
			HighlightSelectedGameObject ();
		}

	}

	public void ResetHighlight(){
		foreach (GameObject temp in currentGameObjectsInScene) {
			temp.GetComponent<Renderer> ().material = this.defaultObjectMaterial;
		}

	}

	public void HighlightSelectedGameObject(){


		this.defaultObjectMaterial = currentGameObjectsInScene [currentChosenIndex].GetComponent<Renderer> ().material;
		currentGameObjectsInScene[currentChosenIndex].GetComponent<Renderer>().material = draggedObjectMaterial;

	}
}
