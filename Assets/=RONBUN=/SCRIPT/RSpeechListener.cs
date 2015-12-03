using UnityEngine;
using System;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class RSpeechListener : MonoBehaviour 
{
	//public Text debugText;
	// Grammar XML file name
	private string grammarFileName = "japan-speech2.grxml";
	
	// Grammar language (English by default)
	//	public int languageCode = 1033;
	private int languageCode = 1041;
	
	// Required confidence
	public float requiredConfidence = 0.0f;

	// Is currently listening
	private bool isListening;
	
	// Current phrase recognized
	private bool isPhraseRecognized;
	private string phraseTagRecognized;
	
	// primary sensor data structure
	private KinectInterop.SensorData sensorData = null;
	
	// Bool to keep track of whether Kinect and SAPI have been initialized
	private bool sapiInitialized = false;
	
	// The single instance of SpeechManager
	private static RSpeechListener instance;
	
	
	// returns the single SpeechManager instance
	public static RSpeechListener Instance
	{
		get
		{
			return instance;
		}
	}
	
	// returns true if SAPI is successfully initialized, false otherwise
	public bool IsSapiInitialized()
	{
		return sapiInitialized;
	}
	
	// returns true if the speech recognizer is listening at the moment
	public bool IsListening()
	{
		return isListening;
	}
	
	// returns true if the speech recognizer has recognized a phrase
	public bool IsPhraseRecognized()
	{
		return isPhraseRecognized;
	}
	
	// returns the tag of the recognized phrase
	public string GetPhraseTagRecognized()
	{
		return phraseTagRecognized;
	}
	
	// clears the recognized phrase
	public void ClearPhraseRecognized()
	{
		isPhraseRecognized = false;
		phraseTagRecognized = String.Empty;
	}
	
	//----------------------------------- end of public functions --------------------------------------//
	
	void Start() 
	{
		try 
		{
			// get sensor data
			KinectManager kinectManager = KinectManager.Instance;
			if(kinectManager && kinectManager.IsInitialized())
			{
				sensorData = kinectManager.GetSensorData();
			}
			
			if(sensorData == null || sensorData.sensorInterface == null)
			{
				throw new Exception("Speech recognition cannot be started, because KinectManager is missing or not initialized.");
			}
			

			
			// ensure the needed dlls are in place and speech recognition is available for this interface
			bool bNeedRestart = false;
			if(sensorData.sensorInterface.IsSpeechRecognitionAvailable(ref bNeedRestart))
			{
				if(bNeedRestart)
				{
					KinectInterop.RestartLevel(gameObject, "SM");
					return;
				}
			}
			else
			{
				string sInterfaceName = sensorData.sensorInterface.GetType().Name;
				throw new Exception(sInterfaceName + ": Speech recognition is not supported!");
			}
			
			// Initialize the speech recognizer
			string sCriteria = String.Format("Language={0:X};Kinect=True", languageCode);
			int rc = sensorData.sensorInterface.InitSpeechRecognition(sCriteria, true, false);
			if (rc < 0)
			{
				string sErrorMessage = (new SpeechErrorHandler()).GetSapiErrorMessage(rc);
				throw new Exception(String.Format("Error initializing Kinect/SAPI: " + sErrorMessage));
			}
			
			if(requiredConfidence > 0)
			{
				sensorData.sensorInterface.SetSpeechConfidence(requiredConfidence);
			}
			
			if(grammarFileName != string.Empty)
			{
				// copy the grammar file from Resources, if available
				if(!File.Exists(grammarFileName))
				{
					TextAsset textRes = Resources.Load(grammarFileName, typeof(TextAsset)) as TextAsset;
					
					if(textRes != null)
					{
						string sResText = textRes.text;
						File.WriteAllText(grammarFileName, sResText);
					}
				}
				
				// load the grammar file
				rc = sensorData.sensorInterface.LoadSpeechGrammar(grammarFileName, (short)languageCode);
				if (rc < 0)
				{
					string sErrorMessage = (new SpeechErrorHandler()).GetSapiErrorMessage(rc);
					throw new Exception("Error loading grammar file " + grammarFileName + ": " + sErrorMessage);
				}
			}
			
			instance = this;
			sapiInitialized = true;
			
			//DontDestroyOnLoad(gameObject);
			

		} 
		catch(DllNotFoundException ex)
		{
			Debug.LogError(ex.ToString());

		}
		catch (Exception ex) 
		{
			Debug.LogError(ex.ToString());

		}
	}
	
	void OnDestroy()
	{
		if(sapiInitialized && sensorData != null && sensorData.sensorInterface != null)
		{
			// finish speech recognition
			sensorData.sensorInterface.FinishSpeechRecognition();
		}
		
		sapiInitialized = false;
		instance = null;
	}
	
	void Update () 
	{
		// start Kinect speech recognizer as needed
		//		if(!sapiInitialized)
		//		{
		//			StartRecognizer();
		//			
		//			if(!sapiInitialized)
		//			{
		//				Application.Quit();
		//				return;
		//			}
		//		}
		
		if(sapiInitialized)
		{
			// update the speech recognizer
			int rc = sensorData.sensorInterface.UpdateSpeechRecognition();
			
			if(rc >= 0)
			{
				// estimate the listening state
				if(sensorData.sensorInterface.IsSpeechStarted())
				{
					isListening = true;
				}
				else if(sensorData.sensorInterface.IsSpeechEnded())
				{
					isListening = false;
				}
				
				// check if a grammar phrase has been recognized
				if(sensorData.sensorInterface.IsPhraseRecognized())
				{
					isPhraseRecognized = true;
					
					phraseTagRecognized = sensorData.sensorInterface.GetRecognizedPhraseTag();
					sensorData.sensorInterface.ClearRecognizedPhrase();

					string debug = phraseTagRecognized+"\n";
					//Debug.Log("saying = "+phraseTagRecognized);

//					if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_MOVE)){
//						debug += "now move-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("移動モード");
//						this.gameObject.GetComponent<RGestureListener>().SendMessage("ChangeGesture",RGestureListener.GESTURE_IDENTITY.MOVE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_ROTATE)){
//						debug +="now rotate-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("回転モード");
//						this.gameObject.GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.ROTATE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_SCALE)){
//						debug +="now scale-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("拡大モード");
//						this.gameObject.GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.SCALE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_CUBE)){
//						debug +="now cube-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("はこ,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CUBE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_SPHERE)){
//						debug +="now sphere-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("スフィア,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_SPHERE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_CYLINDER)){
//						debug +="now cylinder-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("まるい,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CYLINDER);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_PLANE)){
//						debug +="now plane-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("プレン,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_ALL)){
//						debug +="now all-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("ぜんぶ,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().SelectAllGameObject();
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_DELETE)){
//						debug +="now delete-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("さくじょ,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().DeleteGameObject();
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_VIEW)){
//						debug +="now view-ing\n";
//						this.gameObject.GetComponent<RSceneHandler>().ViewGameObject();
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_YES)){
//						debug +="now yes-ing\n";
//						//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_NEXT)){
//						debug +="now next-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("つぎ,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().ChangeGobjTo(true);
//						//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
//					}
//					else if(phraseTagRecognized.Equals(ConstantScript.SPEECH_COMMAND_PREV)){
//						debug +="now prev-ing\n";
//						this.gameObject.GetComponent<DebugScript>().SetMessage("まえ,聞きました");
//						this.gameObject.GetComponent<RSceneHandler>().ChangeGobjTo(false);
//						//this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
//					}
					//debugText.text = debug;
				}
			}
		}
	}
	

	
	
}
