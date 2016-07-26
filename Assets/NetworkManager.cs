using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
	public string mainTextInput;

	// read Thread
	Thread readThread;

	// udpclient object
	UdpClient client;

	// port number
	public int port = 8001;

	// UDP packet store
	public string lastReceivedPacket = "";
	public string allReceivedPackets = ""; // this one has to be cleaned up from time to time

	// Functionality for setting Text
	GameObject mainText, b1, b2, b3, b4, network;
	Text text, b1text, b2text, b3text, b4text;
	String newText, b1newText, b2newText, b3newText, b4newText;

	// Use this for initialization
	void Awake () {
		// Find the GameObject
		mainText = GameObject.Find("MainText");
		b1 = GameObject.Find ("Button1");
		b2 = GameObject.Find ("Button2");
		b3 = GameObject.Find ("Button3");
		b4 = GameObject.Find ("Button4");

		// Get each Text Component
		text = mainText.GetComponent <Text> ();
		b1text = b1.GetComponent <Text> ();
		b2text = b2.GetComponent <Text> ();
		b3text = b3.GetComponent <Text> ();
		b4text = b4.GetComponent <Text> ();

		// Reassign the Text Component (Testing)
		text.text = "Working"; // for debugging...
		b1text.text = "Working as well";
		b2text.text = "Working as well";
		b3text.text = "Working as well";
		b4text.text = "Working as well";

		// Set newTexts
		newText = "";
		b1newText = "";
		b2newText = "";
		b3newText = "";
		b4newText = "";
	}

	// start from unity3d
	void Start()
	{
		// create thread for reading UDP messages
		readThread = new Thread(new ThreadStart(ReceiveData));
		readThread.IsBackground = true;
		readThread.Start();
	}

	// Unity Update Function
	void Update()
	{
		// check button "s" to abort the read-thread
		if (Input.GetKeyDown ("q")) {
			stopThread ();
		}

		text.text = newText.ToString();
		b1text.text = b1newText.ToString();
		b2text.text = b2newText.ToString();
		b3text.text = b3newText.ToString();
		b4text.text = b4newText.ToString();

	}

	// Unity Application Quit Function
	void OnApplicationQuit()
	{
		stopThread();
	}

	// Stop reading UDP messages
	private void stopThread()
	{
		if (readThread.IsAlive)
		{
			readThread.Abort();
		}
		client.Close();
	}

	// receive thread function
	public void ReceiveData()
	{
		client = new UdpClient(port);
		while (true)
		{
			try
			{
				// receive bytes
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);

				// encode UTF8-coded bytes to text format
				string inText = Encoding.UTF8.GetString(data);

				// show received message
				print(">> " + inText);

				// Route data to the right place
				// if... substring(0) == q {}
				switch(inText.Substring(0,1)) 
				{
				case "q" :
					newText = inText.Substring(2);
					break;
				case "1" :
					b1newText = inText.Substring(2);
					break;
				case "2" :
					b2newText = inText.Substring(2);
					break;
				case "3" :
					b3newText = inText.Substring(2);
					break;
				case "4" :
					b4newText = inText.Substring(2);
					break;
				}

				// store new massage as latest message
				lastReceivedPacket = inText;

				// update received messages
				allReceivedPackets = allReceivedPackets + inText;

			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	// return the latest message
	public string getLatestPacket()
	{
		allReceivedPackets = "";
		return lastReceivedPacket;
	}


}