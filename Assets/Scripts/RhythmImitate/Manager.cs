using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MidiJack;
public class Manager : MonoBehaviour {
	Queue<char> key_input = new Queue<char>();
	public Text text;
	public Image panelObject;
	public Image flag;
	private bool colorLerp = true;
	string move = "move";
	string back = "back";
	string dumb = "dumb";
	string dshi = "dshi";
	string spear = "attack";
	float pre_position = 0;
	int[] progress_map = new int[]{0,0,0,0};
	string[] instruction_map = new string[]{"movef","moveb","attack","defence"};
	string[] interval_instruction_map = new string[]{"P5","M3","P9","D7"};
	string[] notice_map = new string[]{"Move forward!", "Move backward!", "Attack!!!", "Defence!!"};
	int curInstruction = 0;
	playerController player_controller;
	bossController boss_controller;
	MonsterController monster_controller;
	mentorController mentor_controller;
	public GameObject player;
	public GameObject boss;
	public GameObject mentor;
	public GameObject monster;
	SpriteRenderer boss_renderer;
	private MetronomePro metro;
	AudioSource bt_source;
	// AudioSource solo_source;
	// AudioClip[] bt_clips;
	AudioClip[] ins_clips;
	private AudioSource metronomeAudioSource;
	private AudioSource ins_source;
	private MusicAnalysis musicAnalysis;
	private bool firePrepare;
	private int fireTickCounter;
	private Dictionary<string, int> instructionToIdx = new Dictionary<string, int>();
	public int mode = 0;
	
	void Awake()
	{
		//Object[] bt_clips_raw = Resources.LoadAll("AudioFiles/backing_track", typeof(AudioClip));
		Object[] ins_clips_raw = Resources.LoadAll("AudioFiles/instruction_rhythm", typeof(AudioClip));
		Object[] ins_interval_clips_raw = Resources.LoadAll("AudioFiles/instruction_interval", typeof(AudioClip));
		// bt_clips = new AudioClip[bt_clips_raw.Length];
		ins_clips = new AudioClip[ins_clips_raw.Length];
		for (int i = 0; i<ins_clips_raw.Length;i++){
			if (mode == 2){
				ins_clips[i] = (AudioClip)ins_interval_clips_raw[i];
			} else {
				ins_clips[i] = (AudioClip)ins_clips_raw[i];
			}
			instructionToIdx.Add(ins_clips[i].name, i);
		}
		AudioSource[] source_array = GetComponents<AudioSource>();
		
		bt_source = source_array[0];
		ins_source = source_array[1];
		metronomeAudioSource = source_array[2];

		// bt_source.clip = bt_clips[0];
		if(mode == 2){
			ins_source.clip = ins_clips[instructionToIdx["P5"]];
		} else {
			ins_source.clip = ins_clips[instructionToIdx["movef"]];
		}
		boss_controller = boss.GetComponent<bossController>();
		boss_renderer = boss.GetComponent<SpriteRenderer>();
		mentor_controller = mentor.GetComponent<mentorController>();
		player_controller = player.GetComponent<playerController>();
		monster_controller = monster.GetComponent<MonsterController>();
		metro = GetComponent<MetronomePro>();
		musicAnalysis = GetComponent<MusicAnalysis>();
	}

	// Use this for initialization
	void Start () {
		text.text = "welcome!";
		firePrepare = false;
		//boss_controller = boss.GetComponent<bossController>();
		pre_position = player.transform.position.x;
		// bt_source.Play();
		// solo_source.Play();
		// metro.Play();
	}
	
	// Update is called once per frame
	void Update () {
		keyStore();
		if (colorLerp){
			flag.color = Color.Lerp(Color.white,Color.clear,Mathf.PingPong(Time.time/0.5f, 1));
		} 
		if (player.transform.position.x > pre_position + 1.92f){
			player_controller.stop();
			//boss_controller.stop();
			if (mentor_controller != null) mentor_controller.stop();
			
		}
		if (player.transform.position.x < pre_position - 1.92f){
			player_controller.stop();
			if (mentor_controller != null) mentor_controller.stop();
		}
		// if (Input.GetKey("f")){
		// 	Debug.Log("fire");
		// 	boss_controller.attack(0);
		// }
		// if (Input.GetKey("r")){
		// 	boss_controller.attack(1);
		// }
		if (Input.GetKey("p")){
			bt_source.Play();
			metro.Play();
		}
		if (Input.GetKey("f")) {
			bt_source.Stop();
			metro.Stop();
			GlobalControl.Instance.completed[(int)GlobalControl.Stages.kRhythm] = true;
			panelObject.rectTransform.sizeDelta = new Vector2(314,410);
			text.rectTransform.sizeDelta = new Vector2(253, 250);
			text.text = "Cool, you have passed my rhythm test, now find the dial in the room, and try to play around the circle to resolve the chords";
			// text.text = "haha";
		}
		if (Input.GetKey("k")) {
			GlobalControl.Instance.completed[(int)GlobalControl.Stages.kRhythm] = true;
			SceneManager.LoadScene(0);
		}
	}

	public string checkProgress(){
		return instruction_map[curInstruction];
	}

	public string checkProgressInterval(){
		return interval_instruction_map[curInstruction];
	}

	public void incrementProgress(bool succeed){
		if (succeed) {
			progress_map[curInstruction]++;
			if (progress_map[curInstruction]>=2) curInstruction++;
		}
	}

	public playerController GetPlayerController(){
		return this.player_controller;
	}

	public IEnumerator OnTick (int CurrentTick, List<double> songTickTimes, int barOffset, int Step, int roundLength, int Mode) {
		metronomeAudioSource.Play ();
		if (CurrentTick >= barOffset * Step){
			if (mode == 0){
				StartCoroutine(mentorMode(CurrentTick, songTickTimes, barOffset, Step, roundLength));
			}else if (mode == 1){
				StartCoroutine(monsterMode(CurrentTick, songTickTimes, barOffset, Step, roundLength));
			} else if (mode == 2) {
				StartCoroutine(mentorIntervalMode(CurrentTick, songTickTimes, barOffset, Step, roundLength*2));
			}
			
		}
		yield return null;
	}

	private IEnumerator mentorIntervalMode(int CurrentTick, List<double> songTickTimes, int barOffset, int Step, int roundLength){
		if ((CurrentTick - barOffset * Step - 1) % (roundLength * Step * 2) == 0){
			mentor_controller.teach();
			if (CurrentTick == barOffset * Step + 1){
				ins_source.Play();
				text.text = "move forward";
			} else {
				string instruction = musicAnalysis.analysisMusic(CurrentTick, songTickTimes, mode);
				if(!ins_source.isPlaying) ins_source.clip = ins_clips[instructionToIdx[checkProgressInterval()]];
				yield return null;
				ins_source.Play();
				text.text = notice_map[curInstruction];
				switch (instruction){
					case "P5":
						//text.text = "move forward";
						player_controller.move(2);
						mentor_controller.move(2);
						pre_position = player.transform.position.x;
						break;
					case "M3":
						//text.text = "move backward";
						player_controller.move(-2);
						mentor_controller.move(-2);
						pre_position = player.transform.position.x;
						break;
					case "P9":
						//text.text = "attack";
						player_controller.throw_spear();
						break;
					case "D7":
						//text.text = "defence";
						player_controller.use_shield();
						break;
					case "failed":
						text.text = "failed";
						Debug.Log("failed");
						break;
				}
			}
		} else if ((CurrentTick - barOffset * Step - 1 - roundLength * Step) % (roundLength * Step * 2) == 0) {
			mentor_controller.listen();
		}
	}

	private IEnumerator mentorMode(int CurrentTick, List<double> songTickTimes, int barOffset, int Step, int roundLength){
		if ((CurrentTick - barOffset * Step - 1) % (roundLength * Step * 2) == 0){
			mentor_controller.teach();
			if (CurrentTick == barOffset * Step + 1){
				ins_source.Play();
				text.text = "move forward";
			} else {
				string instruction = musicAnalysis.analysisMusic(CurrentTick, songTickTimes, mode);
				if(!ins_source.isPlaying) ins_source.clip = ins_clips[instructionToIdx[checkProgress()]];
				yield return null;
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
		} else if ((CurrentTick - barOffset * Step - 1 - roundLength * Step) % (roundLength * Step * 2) == 0) {
			mentor_controller.listen();
		}
	}

	private IEnumerator monsterMode(int CurrentTick, List<double> songTickTimes, int barOffset, int Step, int roundLength){
		if ((CurrentTick - barOffset * Step - 1) % (roundLength * Step * 2) == 0){
			colorLerp = true;
			player_controller.deprepare();
			if (!firePrepare){
				int random = Random.Range(1,11);
				if (random <= 9) {
					monster_controller.prepareFire();
					firePrepare = true;
					fireTickCounter = 0;
				}
			} else {
				fireTickCounter++;
				if (fireTickCounter >= 2){
					firePrepare = false;
					monster_controller.shootFire();
				}
			}	
		} else if ((CurrentTick - barOffset * Step - 1 - roundLength * Step) % (roundLength * Step * 2) == 0) {
			colorLerp = false;
			flag.color = Color.white;
			string instruction = musicAnalysis.analysisMusic(CurrentTick, songTickTimes, mode);
			if(!ins_source.isPlaying && instruction!="failed") ins_source.clip = ins_clips[instructionToIdx[instruction]];
			yield return null;
			if (instruction != "failed") ins_source.Play();
			
			switch (instruction){
				case "movef":
					//text.text = "move forward";
					player_controller.move(2);
					pre_position = player.transform.position.x;
					break;
				case "moveb":
					//text.text = "move backward";
					player_controller.move(-2);
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
					Debug.Log("failed");
					break;
			}
		}
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
