using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {
	GameObject dalmatianGO;
	GameObject bowlGO;
	// Use this for initialization
	void Start () {
		dalmatianGO = GameObject.Find ("DalmatianLP");
		bowlGO = GameObject.Find ("bowl");
	}

	private void hide (GameObject go) {
		go.SetActive (false);
	}

	private void show (GameObject go) {
		go.SetActive (true);
	}

	public void hideAnimal () {
		hide (dalmatianGO);
	}

	public void showAnimal () {
		show (dalmatianGO);
	}

	public void hideBowl () {
		hide (bowlGO);
	}

	public void showBowl () {
		show (bowlGO);
	}

}