using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { STOPPEDATFLOOR, LYINGONBED, WALKING, JUMPINGHIGH, JUMPINGLOW }

public class AnimalControllerScript : MonoBehaviour {
	Animation anim;
	NavMeshAgent agent;
	Vector3 center_point;
	Vector3 bed1_point;
	Vector3 bed2_point;
	State state;

	void Start () {
		//rb = GetComponent<Rigidbody> ();
		center_point = GameObject.Find ("center_point").transform.position;
		bed1_point = GameObject.Find ("bed1_point").transform.position;
		bed2_point = GameObject.Find ("bed2_point_dest").transform.position;
		anim = GetComponent<Animation> ();
		agent = GetComponent<NavMeshAgent> ();
		agent.autoTraverseOffMeshLink = false;
		agent.destination = agent.transform.position;
		anim["Walk"].wrapMode = WrapMode.Loop;
		handleNewState (State.STOPPEDATFLOOR);
	}

	void Update () {

		// move
		if (Input.GetKeyDown (KeyCode.F1))
			goToCenter ();
		if (Input.GetKeyDown (KeyCode.F2))
			goToBed1 ();
		if (Input.GetKeyDown (KeyCode.F3))
			goToBed2 ();
		if (Input.GetKeyDown (KeyCode.S))
			stopMovement ();
	}

	private void logState (State newState) {
		Debug.Log ("State : " + this.state + " -->" + newState);
	}

	private void handleNewState (State newState) {
		if (state == newState) {
			return;
		}
		logState (newState);
		switch (newState) {
			case State.STOPPEDATFLOOR:
				{
					this.anim.CrossFade ("Idled");
					break;
				}
			case State.LYINGONBED:
				{
					if (state == State.JUMPINGHIGH) {
						this.anim.CrossFade ("Lie Down");
						this.anim.CrossFadeQueued ("Rest");
					} else
						return;
					break;
				}
			case State.WALKING:
				{
					if (state == State.JUMPINGLOW || state == State.WALKING || state == State.STOPPEDATFLOOR) {
						this.anim.CrossFade ("Walk");
					} else
						return;
					break;

				}
			case State.JUMPINGHIGH:
				{
					if (state == State.WALKING || state == State.STOPPEDATFLOOR) {
						this.anim.CrossFade ("Jump High");
					} else
						return;
					break;

				}
			case State.JUMPINGLOW:
				{
					if (state == State.LYINGONBED) {
						this.anim.CrossFade ("Jump Low");
					} else
						return;
					break;
				}
			default:
				{ break; }
		}
		this.state = newState;
	}

	void FixedUpdate () {
		//	Debug.Log("State : " + this.state);
		if (Vector3.Distance (agent.transform.position, agent.destination) <= 0.5f) {
			if (isUp ()) {
				handleNewState (State.LYINGONBED);
			} else {
				handleNewState (State.STOPPEDATFLOOR);
			}
		} else {
			if (agent.isOnOffMeshLink) {
				if (!isUp ()) {
					if (state != State.JUMPINGLOW)
						handleNewState (State.JUMPINGHIGH);
				} else {
					if (state != State.JUMPINGHIGH)
						handleNewState (State.JUMPINGLOW);
				}

				OffMeshLinkData data = agent.currentOffMeshLinkData;
				Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
				agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed * Time.deltaTime);
				// Agent reached jump/drop target point
				if (agent.transform.position == endPos) {
					agent.CompleteOffMeshLink ();
					if (!isUp ()) {						
						handleNewState (State.WALKING);
					} else {
						handleNewState (State.LYINGONBED);
					}
				}
			} else {
				handleNewState (State.WALKING);
			}
		}
	}

	// Movements
	public void moveTo (Vector3 destination) {
		agent.destination = destination;
	}

	public void goToBed1 () {
		moveTo (bed1_point);
	}
	public void goToBed2 () {
		moveTo (bed2_point);
	}
	public void goToCenter () {
		moveTo (center_point);
	}

	public void stopMovement () {
		agent.destination = agent.transform.position;
	}

	private bool isUp () {
		return agent.transform.position.y > 1.00f;
	}

}