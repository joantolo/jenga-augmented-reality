using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to describe Jenga block info and management.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class JengaBlock : MonoBehaviour
{
	//== Properties ============================================================

	public int floor;								// Floor number of block.

	public int color;								// Color of block.

	public int id;									// Id of the block.

	public Vector3 p								// Position of the block.
	{
		get { return transform.localPosition; }
		set { transform.localPosition = value; }
	}

	public Quaternion r								// Rotation of the block.
	{
		get { return transform.localRotation; }
		set { transform.localRotation = value; }
	}

	private bool _e;

	public bool e									// Activation of the block.
	{
		get { return _e; }
		set 
		{ 
			_e = value;
			gameObject.SetActive(value);
		}
	}

	//== Methods ===============================================================

	public void Start() 
	{
		e = true;
	}
}
