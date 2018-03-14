using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_Manager : MonoBehaviour {
	wu_PlayerController playerController;
	public GameObject wu_player;
	GameObject[] enemies;
	ArrayList enemiesPos = new ArrayList(); 
	bool behaviourRight = true;
	// Use this for initialization
	void Start () {
		playerController = wu_player.GetComponent<wu_PlayerController>();
		enemies = GameObject.FindGameObjectsWithTag("enemy");
		foreach (GameObject enemy in enemies) {
			enemiesPos.Add(enemy.transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {
		updateEnemyPos();
		if (behaviourRight) {
			playerController.Patrol();
			playerController.Attack(enemiesPos);
		} else {
			playerController.Patrol();
		}
	}
	void updateEnemyPos(){
		enemiesPos.Clear();
		foreach (GameObject enemy in enemies) {
			enemiesPos.Add(enemy.transform.position);
		}
	}
}
