using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wu_Manager : MonoBehaviour {
	wu_PlayerController playerController;
	public GameObject wu_player;
	GameObject[] enemies;
	ArrayList enemiesPos = new ArrayList(); 
	bool behaviourRight = true;
	string[] chordList = {"Dm7", "G7", "Cmaj7"};
	Vector3 initFloorPos = new Vector3(1.5f, 2.5f, 0);
	float floorHight = 2.4f;
	public GameObject floor;
	public GameObject chordLabel;
	// Use this for initialization
	void Start () {
		playerController = wu_player.GetComponent<wu_PlayerController>();
		// enemies = GameObject.FindGameObjectsWithTag("enemy");
		// foreach (GameObject enemy in enemies) {
		// 	enemiesPos.Add(enemy.transform.position);
		// }
		initMap(chordList);

	}
	
	// Update is called once per frame
	void Update () {
		// updateEnemyPos();
		// if (behaviourRight) {
		// 	playerController.Patrol();
		// 	playerController.Attack(enemiesPos);
		// } else {
		// 	playerController.Patrol();
		// }
	}
	// void updateEnemyPos(){
	// 	enemiesPos.Clear();
	// 	foreach (GameObject enemy in enemies) {
	// 		enemiesPos.Add(enemy.transform.position);
	// 	}
	// }
	void initMap(string[] chordList){
		Vector3 curFloorPos = initFloorPos;
		for (int i = 0; i < chordList.Length; i++) {
			curFloorPos.x *= -1;
			GameObject newfloor = Instantiate(floor, curFloorPos, Quaternion.identity);
			newfloor.gameObject.name = "Floor_" + i;
			GameObject newChordLabel = Instantiate(chordLabel, new Vector3(curFloorPos.x + 7 * (i%2 == 0 ? 1 : -1), curFloorPos.y+1), Quaternion.identity);
			newChordLabel.gameObject.name = "ChordLabel_" + i;
			newChordLabel.GetComponentInChildren<Text>().text = chordList[i];
			curFloorPos.y -= floorHight;
		}
	}
}
