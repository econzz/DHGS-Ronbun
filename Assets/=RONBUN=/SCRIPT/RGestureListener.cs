using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class RGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface {
	public enum GESTURE_IDENTITY
	{
		MOVE,
		SCALE,
		ROTATE
	};


	// Use this for initialization
	void Start () {
		ChangeGesture (RGestureListener.GESTURE_IDENTITY.MOVE);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DeleteAllGesture(){
		KinectManager manager = KinectManager.Instance;
		long userId = manager.GetPrimaryUserID ();
		manager.DeleteGesture(userId, KinectGestures.Gestures.SwipeLeft);
		manager.DeleteGesture(userId, KinectGestures.Gestures.SwipeRight);
		manager.DeleteGesture(userId, KinectGestures.Gestures.ZoomOut);
		manager.DeleteGesture(userId, KinectGestures.Gestures.Wheel);
	}

	public void ChangeGesture(GESTURE_IDENTITY tempGestureIdentity){

		RSceneHandler.currentGesture = tempGestureIdentity;

		KinectManager manager = KinectManager.Instance;
		long userId = manager.GetPrimaryUserID ();

		this.DeleteAllGesture ();

		if (tempGestureIdentity == GESTURE_IDENTITY.MOVE) {
			manager.DetectGesture(userId, KinectGestures.Gestures.SwipeLeft);
			manager.DetectGesture(userId, KinectGestures.Gestures.SwipeRight);
		} else if (tempGestureIdentity == GESTURE_IDENTITY.SCALE) {
			manager.DetectGesture(userId, KinectGestures.Gestures.ZoomOut);
		} else if (tempGestureIdentity == GESTURE_IDENTITY.ROTATE) {
			manager.DetectGesture(userId, KinectGestures.Gestures.Wheel);
		}
	}

	#region GestureListenerInterface implementation

	public void UserDetected (long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;

		this.ChangeGesture (GESTURE_IDENTITY.MOVE);
	}

	public void UserLost (long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;
	}

	public void GestureInProgress (long userId, int userIndex, KinectGestures.Gestures gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return;
		
		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} {1:F0}%", gesture, screenPos.z * 100f);
			//GestureInfo.GetComponent<GUIText>().text = sGestureText;
			Debug.Log(" zoom ="+(screenPos.z));
			this.gameObject.GetComponent<RSceneHandler>().ScaleSelectedGameObject(screenPos.z);
		}
		else if(gesture == KinectGestures.Gestures.Wheel && progress > 0.5f)
		{
			//string sGestureText = string.Format ("{0} {1:F0} degrees", gesture, screenPos.z);
			//GestureInfo.GetComponent<GUIText>().text = sGestureText;
			Debug.Log(" wheel = x = "+screenPos.x+" y = "+screenPos.y+" z = "+screenPos.z);
			//Debug.Log(sGestureText);
			this.gameObject.GetComponent<RSceneHandler>().RotateSelectedGameObject(screenPos,2);
		}
	}

	public bool GestureCompleted (long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint, Vector3 screenPos)
	{
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return false;

		if (gesture == KinectGestures.Gestures.SwipeLeft) {
			Debug.Log("SWIPE LEFT DETECTED");
			this.gameObject.GetComponent<RSceneHandler> ().ChangeGobjTo (false);
		} 
		else if (gesture == KinectGestures.Gestures.SwipeRight) {
			Debug.Log("SWIPE RIGHT DETECTED");
			this.gameObject.GetComponent<RSceneHandler> ().ChangeGobjTo (true);
		}

		return true;
	}

	public bool GestureCancelled (long userId, int userIndex, KinectGestures.Gestures gesture, KinectInterop.JointType joint)
	{
		// the gestures are allowed for the primary user only
		KinectManager manager = KinectManager.Instance;
		if(!manager || (userId != manager.GetPrimaryUserID()))
			return false;
		

		
		return true;
	}

	#endregion
}
