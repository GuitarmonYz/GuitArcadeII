using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using MidiJack;
public class wu_MusicAnalysis : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < 128; i++) {
			MidiMaster.GetKey(i);
		}
	}
}
