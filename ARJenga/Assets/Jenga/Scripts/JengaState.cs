using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// 
/// Class to determine behaviour in relation with animator events.
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////

public class JengaState : StateMachineBehaviour 
{
	//== Methods ===============================================================

	// ---- Unity events ----

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		JengaStateMachine sm = animator.GetComponent<JengaStateMachine>();
		if (sm == null)
			return;

		if (stateInfo.IsName("Playing"))
		{
			sm.startTurn();			
		}

		if (stateInfo.IsName("End Turn")) 
		{
			sm.endTurn();
		}

		if (stateInfo.IsName("Lose"))
		{
			sm.lose();
		}

		if (stateInfo.IsName("Win"))
		{
			sm.win();
		}

		if (stateInfo.IsName("Viewing"))
		{
			sm.disableHandler();			
		}

		if (stateInfo.IsName("Block Fallen"))
		{
			animator.ResetTrigger("Block Falls");
		}

	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		JengaStateMachine sm = animator.GetComponent<JengaStateMachine>();
		if (sm == null)
			return;

		if (stateInfo.IsName("Playing")) 
		{
			sm.disableHandler();			
		}

	}
}
