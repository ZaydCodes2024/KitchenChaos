using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footStepTimer;
    private float footStepTimerMax = .1f;
    [SerializeField] private float footStepSound = 1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footStepTimer -= Time.deltaTime;

        if (footStepTimer < 0f)
        {
            footStepTimer = footStepTimerMax;

            if (player.IsWalking())
            {
                SoundManager.Instance.PlayFootStepSound(player.transform.position, footStepSound);
            }
        }
    }
}
