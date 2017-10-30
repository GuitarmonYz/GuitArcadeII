﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMetro : MonoBehaviour
{
    public double bpm = 140.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;
	private int tick_times = 0;
	private manager manager;
    void Start()
    {
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
		Debug.Log("start_time"+startTick);
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;
		manager = GetComponent<manager>();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                // if (++accent > signatureHi)
                // {
                //     accent = 1;
                //     amp *= 2.0F;
                // }
				//manager.OnTick(tick_times);
				//BroadcastMessage("OnTick", tick_times);
				tick_times++;
                Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }
}
