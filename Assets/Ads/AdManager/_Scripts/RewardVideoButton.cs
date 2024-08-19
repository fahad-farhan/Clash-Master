using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class RewardVideoButton : MonoBehaviour
{
    Button button;
    public UnityEvent rewardVideoWatchedEvent;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(WatchRewardedVideo);
    }

    private void WatchRewardedVideo()
    {
        AdManager.instance.ShowRewardedVideo(rewardVideoWatchedEvent);
    }
}
