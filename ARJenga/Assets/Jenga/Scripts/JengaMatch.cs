using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to manage Jenga tower data and connection with the other player.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

public class JengaMatch : MonoBehaviour 
{
	//== Members ===============================================================

	public JengaStateMachine stateMachine;      // State machine of the game.

	public GameObject tower;					// Jenga tower of the game.

	//== Properties ============================================================

	public int portNumber;						// Port number of the local player.

	public int remotePortNumber;				// Port number of the remote player.

	public string remoteIP = "127.0.0.1";		// IP of the remote player.

	UdpClient udp;								// To control UDP connection.

	IPEndPoint ep;								//  Info needed to make an UDP connection.
	
	public Dictionary<int, JengaBlock> dblocks; // Dictionary of Jenga blocks.

	public float lastData = 0;					// Time of last connection.

	public bool isMoving = false;				// Flag to know if player is in his turn.

	[System.Serializable]
	public class JengaData						// Class containing all info of a Jenga match.
	{
		public float timestamp;
		public JengaBlockData[] blocks;
		public bool endTurn;
		public bool lost;
		public int blockMoving;

		public JengaData()
		{
			timestamp = Time.time;
			endTurn = false;
			lost = false;
			blocks = null;
			blockMoving = -1;
		}
	}

	[System.Serializable]
	public class JengaBlockData					// Class containing all the info of a Jenga block.
	{
		public int id;
		public float[] p;
		public float[] r;
		public bool e;

		public JengaBlockData(JengaBlock b)
		{
			id = b.id;
			p = new float[]{b.p.x, b.p.y, b.p.z};
			r = new float[]{b.r.x, b.r.y, b.r.z, b.r.w};
			e = b.e;
		}
	}

	//== Methods ===============================================================

	// ---- Unity events ----

	void Start () 
	{
		// Get information of connection.

		remoteIP = PlayerPrefs.GetString("remoteIP", remoteIP);
		remotePortNumber = PlayerPrefs.GetInt("remotePort", remotePortNumber);
		portNumber = PlayerPrefs.GetInt("localPort",10101);

		// Get information of Jenga tower.

		JengaBlock[] blocks = tower.GetComponentsInChildren<JengaBlock>();
		dblocks = new Dictionary<int, JengaBlock>();

		foreach(JengaBlock b in blocks)
			dblocks[b.id] = b;

		// Create socket and determine which turn is.

		initSocket();
		
		if (portNumber > remotePortNumber)  
			stateMachine.receivedStartTurn();
		else if (portNumber == remotePortNumber) 
		{
			if (PlayerPrefs.GetInt("Invited", 0) == 0) 
				stateMachine.endTurn();
			else
				stateMachine.receivedStartTurn();
		}
		else 
		{
			stateMachine.endTurn();
		}
	}
	
	void Update () 
	{
		// Enable physix of Jenga blocks if is a turn.

		enablePhysx(isMoving);
		
		// Send info of Jenga Blocks to the remote player.

		if (isMoving) 
		{
			lastData = Time.time;

			if (!IsInvoking("sendBlocks"))
				InvokeRepeating("sendBlocks", 0.5f, 0.1f);
		}
		else if (!isMoving) 
		{
			CancelInvoke("sendBlocks");
		}

		receiveData();
	}

	// ---- Enable blocks method ----

	void enablePhysx(bool value)
	{
		Rigidbody[] bs = tower.GetComponentsInChildren<Rigidbody>();

		foreach (Rigidbody b in bs)
			b.isKinematic = !value;
	}

	// ---- Connection method ----

	void initSocket() 
	{
		ep = new IPEndPoint(IPAddress.Parse(remoteIP), remotePortNumber);
		udp = new UdpClient(portNumber);
	}

	// ---- Serialization and parsing of data for connection methods ----

	void receiveData()
	{
		if (udp == null)
			return;

		udp.Client.ReceiveTimeout = 5;
		try {
			// Try to load 20 packets to avoid acumulation of the information.

			for (int i = 0; i < 20; ++i)
			{
				IPEndPoint ep2 = new IPEndPoint(IPAddress.Any, 0);
				byte[] bytes = udp.Receive(ref ep2);

				if (!isMoving)
				{
					// Parse Jenga block data from remote player.

					JengaData data;
					BinaryFormatter bf = new BinaryFormatter();
					using (MemoryStream ms = new MemoryStream(bytes))
					{
						data = (JengaData)bf.Deserialize(ms);

						lastData = Time.time;

						if (data.blocks != null)
							foreach (JengaBlockData b in data.blocks)
							{
								dblocks[b.id].p = new Vector3(b.p[0], b.p[1], b.p[2]);
								dblocks[b.id].r = new Quaternion(b.r[0], b.r[1], b.r[2], b.r[3]);
								dblocks[b.id].e = b.e;
							}

						// And finally pass remote Jenga state of game to state Machine.

						if (data.endTurn == true)
							stateMachine.receivedStartTurn();

						if (data.lost == true)
							stateMachine.win();
					}
				}
			}

		} catch (SocketException e) { }
	}

	bool sendData(JengaData data)
	{
		if (udp == null)
			return false;

		try {
			//Serialize data.

			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, data);
				byte[] bytes = ms.ToArray();

				// Once serialized. Send it.

				int res = udp.Send(bytes, bytes.Length, ep);

				if (res != bytes.Length)
					return false;

				return true;
			}
		} catch (System.Exception e)
		{
			Debug.LogError(e.Message);
			return false;
		}
	}

	void fillBlocks(ref JengaData data)
	{
		List<JengaBlockData> lblocks = new List<JengaBlockData>();

		foreach (KeyValuePair<int, JengaBlock> b in dblocks)
		{
			if (b.Value != null)
				lblocks.Add(new JengaBlockData(b.Value));
		}

		data.blocks = lblocks.ToArray();
	}

	void sendBlocks() 
	{
		JengaData data = new JengaData();
		fillBlocks(ref data);
		sendData(data);
	}

	public void sendEndTurn()
	{
		lastData = Time.time;
		JengaData data = new JengaData();
		fillBlocks(ref data);
		data.endTurn = true;
		sendData(data);
	}

	public void sendLost()
	{
		JengaData data = new JengaData();
		fillBlocks(ref data);
		data.lost = true;
		sendData(data);
	}
}
