using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
public class manager : MonoBehaviour {
	Queue<char> key_input = new Queue<char>();
	Queue<float[]> key_down = new Queue<float[]>();
	Queue<float[]> key_up = new Queue<float[]>();
	string move = "move";
	string back = "back";
	string dumb = "dumb";
	string dshi = "dshi";
	string spear = "spea";
	float pre_position = 0;
	int[] progress_map = {128,144,272,304};
	playerController player_controller;
	bossController boss_controller;
	public GameObject player;
	public GameObject boss;
	SpriteRenderer boss_renderer;
	public MetronomePro metro;
	AudioSource bt_source;
	AudioSource solo_source;
	bool input_lock = false;
	int[] instruction_mask = new int[]{0,0,1};
	float starting_time = 0;
	AudioClip[] bt_clips;
	AudioClip[] solo_clips;
	HashSet<int> key_set = new HashSet<int>(){1,3,5,8,10};
	void Awake()
	{
		// Object[] bt_clips_raw = Resources.LoadAll("AudioFiles/backing_track", typeof(AudioClip));
		// Object[] solo_clips_raw = Resources.LoadAll("AudioFiles/solo", typeof(AudioClip));
		// bt_clips = new AudioClip[bt_clips_raw.Length];
		// solo_clips = new AudioClip[solo_clips_raw.Length];
		// for (int i = 0; i<bt_clips_raw.Length;i++){
		// 	bt_clips[i] = (AudioClip)bt_clips_raw[i];
		// }
		// for (int i = 0; i<solo_clips_raw.Length;i++){
		// 	solo_clips[i] = (AudioClip)solo_clips_raw[i];
		// }
		AudioSource[] source_array = GetComponents<AudioSource>();
		bt_source = source_array[0];
		// solo_source = source_array[1];
		// bt_source.clip = bt_clips[0];
		// solo_source.clip = solo_clips[0];
		boss_controller = boss.GetComponent<bossController>();
		boss_renderer = boss.GetComponent<SpriteRenderer>();
		metro = GetComponent<MetronomePro>();

	}

	// Use this for initialization
	void Start () {
		player_controller = player.GetComponent<playerController>();
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
			boss_controller.stop();
		}
		if (player.transform.position.x < pre_position - 1.92f){
			player_controller.stop();
			boss_controller.stop();
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
		
		react_music();
	}
	// public void OnTick(int tick_times){
	// 	Debug.Log("ontick");
	// 	int progress = checkProgress(progress_map, tick_times);
	// 	switch (progress){
	// 		case 0:
	// 			if ((tick_times+1)%32 == 0){
	// 				int random = Random.Range(0,13);
	// 				solo_source.clip = solo_clips[random];
	// 				solo_source.Play();
	// 				StartCoroutine(music_analysis(key_down,key_up));
	// 			}
	// 			break;
	// 		case 1:
	// 			if (tick_times == progress_map[progress]+1){
	// 				bt_source.clip = bt_clips[1];
	// 				bt_source.Play();
	// 			}
	// 			break;
	// 		case 2:
	// 			if (tick_times == progress_map[progress]+1){
	// 				bt_source.clip = bt_clips[2];
	// 				bt_source.Play();
	// 			}
	// 			if ((tick_times+1)%32 == 0){
	// 				int random = Random.Range(14,22);
	// 				solo_source.clip = solo_clips[random];
	// 				solo_source.Play();
	// 			}
	// 			break;
	// 		case 3:
	// 			if (tick_times == progress_map[progress]+1){
	// 				bt_source.clip = bt_clips[3];
	// 				bt_source.Play();
	// 			}
	// 			break;
	// 		default:
	// 			bt_source.Stop();
	// 			break;
	// 	}
	// }

	IEnumerator music_analysis(Queue<float[]> key_down, Queue<float[]> key_up){
		input_lock = true;
		float duration = 60/162;
		float beat_off = 0;
		//onset
		//note analysis
		instruction_mask[1] = 1;
		float num_right_note = 0;
		foreach (float[] note_val in key_down){
			if(key_set.Contains((int)(note_val[0]%12))) num_right_note+=note_val[0];
			float tmp = (note_val[2]-starting_time)/(duration/4);
			beat_off += Mathf.Abs(Mathf.Round(tmp)-tmp);
		}
		beat_off /= key_up.Count;
		Debug.Log(beat_off);
		float ratio = num_right_note/key_up.Count;
		if (ratio>0.85f){
			player_controller.throw_spear();
		}
		key_down.Clear();
		key_up.Clear();
		input_lock = false;
		Debug.Log("Analysis finished");
		yield return null;
	}

	int checkProgress(int[] progress_map, int tick_times){
		for (int i = 0; i < progress_map.Length; i++) 
			if (tick_times/4 < progress_map[i]) return i;
		return -1;
	}

	void react_music(){
		if (instruction_mask[1] == 1){
			Debug.Log("mask works");
			instruction_mask[1] = 0;
		}
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
