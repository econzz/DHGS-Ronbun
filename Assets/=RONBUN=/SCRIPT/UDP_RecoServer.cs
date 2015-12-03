// *********************************************************
// UDP SPEECH RECOGNITION
// *********************************************************
using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

public class UDP_RecoServer : MonoBehaviour
{
	Thread receiveThread;
	UdpClient client;
	public int port = 26000; // DEFAULT UDP PORT !!!!! THE QUAKE ONE ;)
	string strReceiveUDP = "";
	string LocalIP = String.Empty;
	string hostname;
	
	public void Start()
	{
		Application.runInBackground = true;
		init();  
	}
	// init
	private void init()
	{
		receiveThread = new Thread( new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		hostname = Dns.GetHostName();
		IPAddress[] ips = Dns.GetHostAddresses(hostname);
		if (ips.Length > 0)
		{
			LocalIP = ips[0].ToString();
			Debug.Log(" MY IP : "+LocalIP);
		}
	}
	
	private  void ReceiveData()
	{
		client = new UdpClient(port);
		while (true)
		{
			try
			{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Broadcast, port);
				byte[] data = client.Receive(ref anyIP);
				strReceiveUDP = Encoding.UTF8.GetString(data);
				// ***********************************************************************
				// Simple Debug. Must be replaced with SendMessage for example.
				// ***********************************************************************
//				Debug.Log("said = "+strReceiveUDP);
//				if(strReceiveUDP.Equals("ido")){
//					Debug.Log("now move-ing");
//					GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.MOVE);
//					//this.gameObject.SendMessage("ChangeGesture",RGestureListener.GESTURE_IDENTITY.MOVE);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_ROTATE)){
//					Debug.Log("now rotate-ing");
//					GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.ROTATE);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_SCALE)){
//					Debug.Log("now scale-ing");
//					this.gameObject.GetComponent<RGestureListener>().ChangeGesture(RGestureListener.GESTURE_IDENTITY.SCALE);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_CUBE)){
//					Debug.Log("now cube-ing");
//					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CUBE);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_SPHERE)){
//					Debug.Log("now sphere-ing");
//					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_SPHERE);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_CYLINDER)){
//					Debug.Log("now cylinder-ing");
//					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_CYLINDER);
//				}
//				else if(strReceiveUDP.Equals(ConstantScript.SPEECH_COMMAND_PLANE)){
//					Debug.Log("now plane-ing");
//					this.gameObject.GetComponent<RSceneHandler>().CreateGameObject(ConstantScript.GOBJ_PLANE);
//				}
				// ***********************************************************************
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public void ResetPacket(){
		strReceiveUDP = "";
	}
	
	public string UDPGetPacket()
	{
		return strReceiveUDP;
	}
	
	void OnDisable()
	{
		if ( receiveThread != null) receiveThread.Abort();
		client.Close();
	}
}
// *********************************************************