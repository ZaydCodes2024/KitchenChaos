using System;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private AudioSource audioSource;
    private float burnShowProgressAmount = 0.5f;
    private float warningSoundTimer;
    private float warningSoundTimerMax = 0.2f;
    private bool playWarningSound;
    [SerializeField] private StoveCounter stoveCounter;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer < 0)
            {
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayStoveBurnWarningTimerSound(stoveCounter.transform.position);
            }
        }
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        playWarningSound = stoveCounter.IsFried() && e.progressNormalized > burnShowProgressAmount;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;

        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
