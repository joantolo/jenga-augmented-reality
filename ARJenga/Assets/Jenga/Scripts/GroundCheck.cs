using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to determine to State Machine which block has fallen.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////
///

public class GroundCheck : MonoBehaviour
{
	public JengaStateMachine stateMachine;

	void OnTriggerEnter(Collider c) 
	{
		if (c == null)
			return;
		if (c.GetComponent<Rigidbody>() == null)
			return;
		if (stateMachine == null)
			return;

		stateMachine.blockTouchesGround(c.GetComponent<Rigidbody>());
	}
}
