using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wu_Manager : MonoBehaviour {
	wu_PlayerController playerController;
	public GameObject wu_player;
	bool behaviourRight = true;
	string[] chordList = {"Dm7", "G7", "Cmaj7", "Cmaj7"};
	Vector3 initFloorPos = new Vector3(2.5f, 2.5f, 0);
	float floorHight = 2.4f + 1;
	public GameObject floor;
	public GameObject chordLabel;
	private StateController stateController;
	private wu_metronome metronome;
	// Use this for initialization
	void Start () {
		playerController = wu_player.GetComponent<wu_PlayerController>();
		stateController = wu_player.GetComponent<StateController>();
		metronome = GetComponent<wu_metronome>();
		metronome.Play();
		initMap(chordList);
		stateController.SetupAI(playerController);
	}
	
	// Update is called once per frame
	void Update () {
		
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
				curFloorPos.y -= floorHight;
			}
		}
	}
	public void incrementPlayerTick(){
		playerController.incrementTick();
	}
}
