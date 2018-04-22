using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wu_Manager : MonoBehaviour {
	
	public GameObject wu_player;
	public GameObject enemy;
	public GameObject floor;
	public GameObject chordLabel;
	private wu_PlayerController playerController;
	private AudioSource songAudioSource;
	private StateController stateController;
	private wu_metronome metronome;
	private string[] chordList = {"Dm7", "G7", "Cmaj7", "Cmaj7"};
	private Vector3 initFloorPos = new Vector3(2.5f, 2.5f, 0);
	private float floorHight = 2.4f + 1.5f;
	private Queue enemyQueue = new Queue();
	private bool barEnded = false;
	private int curBar = 0;
	void Start () {
		playerController = wu_player.GetComponent<wu_PlayerController>();
		stateController = wu_player.GetComponent<StateController>();
		metronome = GetComponent<wu_metronome>();
		songAudioSource = GetComponents<AudioSource>()[0];
		initMap(chordList);
		stateController.SetupAI(playerController);
	}
	void Update () {
		if(Input.GetKey("p")) {
			songAudioSource.Play();
			metronome.Play();
		}
		if (playerController.barEnd()) {
			if (barEnded == false) {
				barEnded = true;
				if (curBar != 0) {
					GameObject popedEnemy = (GameObject)enemyQueue.Dequeue();
					wu_EnemyController ec = popedEnemy.GetComponent<wu_EnemyController>();
					ec.selfDestroy();
				}
				curBar++;
			}
		} else {
			barEnded = false;
		}
	}
	void initMap(string[] chordList){
		Vector3 curFloorPos = initFloorPos;
		for (int j = 0; j < 5; j++) {
			for (int i = 0; i < chordList.Length; i++) {
				curFloorPos.x *= -1;
				GameObject newfloor = Instantiate(floor, curFloorPos, Quaternion.identity);
				newfloor.gameObject.name = "Floor_" + (chordList.Length * j + i);
				GameObject newChordLabel = Instantiate(chordLabel, new Vector3(curFloorPos.x + 7 * (i%2 == 0 ? 1 : -1), curFloorPos.y+1), Quaternion.identity);
				newChordLabel.gameObject.name = "ChordLabel_" + (chordList.Length * j + i);
				newChordLabel.GetComponentInChildren<Text>().text = chordList[i];
				if (!(i==0 && j==0)) {
					GameObject newEnemy = Instantiate(enemy, new Vector3(curFloorPos.x + (i%2 == 0 ? 1 : -1), curFloorPos.y+1), Quaternion.identity);
					enemyQueue.Enqueue(newEnemy);
				}
				curFloorPos.y -= floorHight;
			}
		}
	}
	public void incrementPlayerTick(){
		playerController.incrementTick();
	}
}
