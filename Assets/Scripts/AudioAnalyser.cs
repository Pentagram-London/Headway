using UnityEngine;
using System.Collections;

/*
    
    This script monitors the microphone input, and analyses it.

*/

public class AudioAnalyser : MonoBehaviour
{

    // audio input properties
    AudioSource audio;

    float[] audioSampleData;
    public int audioSamples = 32;

    Vector2 FreqCutoffs = new Vector2(3, 3);

    bool micExists;

    public float audioInLevel;

    int resetCount;
    int resetDuration = 1000;

    void Start()
    {

        if (Microphone.devices.Length > 0)
            micExists = true;

        if (micExists)
            InitMicInput();
    }

    void Update()
    {
        if (micExists)
            audioInLevel = AnalyseMicInput() * 10000.0f;
        else
            audioInLevel = 50.0f;

    }

    // setup the audio source, and assign the default microphone as an input
    void InitMicInput()
    {

        if (audio)
            audio.Stop();

        audioSampleData = new float[audioSamples];
        audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { } // wait for the audio to start recording
        audio.Play();

        resetCount = resetDuration;

    }

    float AnalyseMicInput()
    {

        // occasionally refresh the adio system, as the buffer seems to fill up...
        if (resetCount == 0)
            InitMicInput();

        float audioMeanLevel = 0.0f;

        audio.GetOutputData(audioSampleData, 0);

        for (int i = 0; i < audioSampleData.Length; i++)
        {
            audioMeanLevel += Mathf.Abs(audioSampleData[i]);
        }
        audioMeanLevel /= audioSampleData.Length;

        resetCount -= 1;

        return audioMeanLevel;
    }

}
