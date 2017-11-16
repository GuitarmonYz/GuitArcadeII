using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;
public class Manager : MonoBehaviour {
	Queue<char> key_input = new Queue<char>();
	public Text text;
	string move = "move";
	string back = "back";
	string dumb = "dumb";
	string dshi = "dshi";
	string spear = "spea";
	float pre_position = 0;
	int[] progress_map = new int[]{0,0,0,0};
	string[] instruction_map = new string[]{"movef","moveb","attack","defence"};
	string[] notice_map = new string[]{"move forward", "move backward", "attack", "defence"};
	int curInstruction = 0;
	playerController player_controller;
	bossController boss_controller;
	mentorController mentor_controller;
	public GameObject player;
	public GameObject boss;
	public GameObject mentor;
	SpriteRenderer boss_renderer;
	public MetronomePro metro;
	AudioSource bt_source;
	AudioSource solo_source;
	float starting_time = 0;
	AudioClip[] bt_clips;
	AudioClip[] ins_clips;
	private AudioSource metronomeAudioSource;
	private AudioSource ins_source;
	private MusicAnalysis musicAnalysis;
	private Dictionary<string, int> instructionToIdx = new Dictionary<string, int>();
	void Awake()
	{
		//Object[] bt_clips_raw = Resources.LoadAll("AudioFiles/backing_track", typeof(AudioClip));
		Object[] ins_clips_raw = Resources.LoadAll("AudioFiles/instruction_rhythm", typeof(AudioClip));
		// bt_clips = new AudioClip[bt_clips_raw.Length];
		ins_clips = new AudioClip[ins_clips_raw.Length];
		// for (int i = 0; i<bt_clips_raw.Length;i++){
		// 	bt_clips[i] = (AudioClip)bt_clips_raw[i];
		// }
		for (int i = 0; i<ins_clips_raw.Length;i++){
			ins_clips[i] = (AudioClip)ins_clips_raw[i];
			instructionToIdx.Add(ins_clips[i].name, i);
		}
		AudioSource[] source_array = GetComponents<AudioSource>();
		
		bt_source = source_array[0];
		ins_source = source_array[1];
		metronomeAudioSource = source_array[2];

		// bt_source.clip = bt_clips[0];
		ins_source.clip = ins_clips[instructionToIdx["movef"]];
		boss_controller = boss.GetComponent<bossController>();
		boss_renderer = boss.GetComponent<SpriteRenderer>();
		mentor_controller = mentor.GetComponent<mentorController>();
		player_controller = player.GetComponent<playerController>();
		metro = GetComponent<MetronomePro>();
		musicAnalysis = GetComponent<MusicAnalysis>();
	}

	// Use this for initialization
	void Start () {
		text.text = "";
		//boss_controller = boss.GetComponent<bossController>();
		pre_position = player.transform.position.x;
		// bt_source.Play();
		// solo_source.Play();
		// metro.Play();
		starting_time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		keyStore();
		if (player.transform.position.x > pre_position + 1.92f){
			player_controller.stop();
			//boss_controller.stop();
			mentor_controller.stop();
		}
		if (player.transform.position.x < pre_position - 1.92f){
			player_controller.stop();
			mentor_controller.stop();
		}
		if (Input.GetKey("f")){
			Debug.Log("fire");
			boss_controller.attack(0);
		}
		if (Input.GetKey("r")){
			boss_controller.attack(1);
		}
		if (Input.GetKey("p")){
			bt_source.Play();
			metro.Play();
		}
	}

	public string checkProgress(){
		//Debug.Log(instruction_map[curInstruction]);
		return instruction_map[curInstruction];
	}

	public void incrementProgress(bool succeed){
		if (succeed) {
			progress_map[curInstruction]++;
			if (progress_map[curInstruction]>=4) curInstruction++;
		}
	}

	public playerController GetPlayerController(){
		return this.player_controller;
	}

	public IEnumerator OnTick (int CurrentTick, List<double> songTickTimes, int barOffset, int Step, int roundLength) {
		metronomeAudioSource.Play ();
		// Debug.Log("tick");
		if (CurrentTick >= barOffset * Step){
			if ((CurrentTick - barOffset * Step - 1) % (roundLength * Step * 2) == 0){
				//Debug.Log(CurrentTick);
				
				if (CurrentTick == barOffset * Step + 1){
					ins_source.Play();
					text.text = "move forward";
				} else {
					string instruction = musicAnalysis.analysisMusic(CurrentTick, songTickTimes);
					if(!ins_source.isPlaying) ins_source.clip = ins_clips[instructionToIdx[checkProgress()]];
					ins_source.Play();
					text.text = notice_map[curInstruction];
					switch (instruction){
						case "movef":
							//text.text = "move forward";
							player_controller.move(2);
							mentor_controller.move(2);
							pre_position = player.transform.position.x;
							break;
						case "moveb":
							//text.text = "move backward";
							player_controller.move(-2);
							mentor_controller.move(-2);
							pre_position = player.transform.position.x;
							break;
					    case "attack":
							//text.text = "attack";
							player_controller.throw_spear();
							break;
						case "defence":
							//text.text = "defence";
							player_controller.use_shield();
							break;
						case "failed":
							text.text = "failed";
							Debug.Log("failed");
							break;
					}
				}
			}
		}

		//Debug.Log ("Current Step: " + CurrentStep + "/" + Step);
		yield return null;
	}

	private void keyStore(){
		foreach (char input in Input.inputString){
			key_input.Enqueue(input);
			if (input == '\n'){
				key_input.Clear();
				Debug.Log("Clear");
			} 
		}
		if (string.Compare(move, new string(key_input.ToArray()))==0){
			Debug.Log("move");
			key_input.Clear();
			player_controller.move(2);
			boss_controller.move(2);
			pre_position = player.transform.position.x;
		}
		if (string.Compare(back, new string(key_input.ToArray()))==0){
			Debug.Log("back");
			key_input.Clear();
			player_controller.back(-2);
			boss_controller.back(-2);
			pre_position = player.transform.position.x;
		}
		if (string.Compare(dumb, new string(key_input.ToArray()))==0){
			Debug.Log("dumb");
			key_input.Clear();
			player_controller.use_umbrella();
		}
		if (string.Compare(dshi, new string(key_input.ToArray()))==0){
			Debug.Log("dshi");
			key_input.Clear();
			player_controller.use_shield();
		}
		if (string.Compare(spear, new string(key_input.ToArray()))==0){
			Debug.Log("spear");
			key_input.Clear();
			player_controller.throw_spear();
		}
	}
}
