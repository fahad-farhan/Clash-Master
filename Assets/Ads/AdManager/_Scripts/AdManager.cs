using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdManager : MonoBehaviour//, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private UnityEvent rewardVideoWatchedCallback;

    //[Header(" Delegates ")]
    public delegate void OnRewardVideoWatched();
    public static OnRewardVideoWatched onRewardVideoWatched;

    public enum TargetPlatform { Android, IOS }
    public TargetPlatform targetPlatform;

    public enum AdNetwork { AdMob, UnityAds }
    public AdNetwork adNetwork;

    public enum CustomBannerPosition { BOTTOM, TOP }
    public CustomBannerPosition bannerPosition;

    [Header(" Settings ")]
    public bool testMode;
    public int interstitialFrequency;
    public bool enableBanner;

    #region Admob

    [Header(" Android ")]
    public string appId;
    public string interstitialId;
    public string bannerId;
    public string rewardedId;


    [Header(" iOS ")]
    public string iOS_appID;
    public string iOS_interstitialID;
    public string iOS_bannerID;
    public string iOS_rewardedId;

    private InterstitialAd interstitial;
    private BannerView bannerView;
    private RewardedAd rewardBasedVideo;


    #endregion


    #region Unity Ads

    [Header(" Game ID ")]
    public string androidGameID;
    public string iosGameID;

    public string androidBannerID;
    public string iosBannerID;
    private string unityAdsBannerID;

    public string androidRewardedVideoID;
    public string iosRewardedVideoID;
    private string unityAdsRewardedVideoID;

    #endregion

    int counter = 0;

    public static AdManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //MobileAds.Initialize(initstatus => { } );




#if UNITY_ANDROID

        unityAdsRewardedVideoID = androidRewardedVideoID;
        unityAdsBannerID = androidBannerID;


        if (adNetwork == AdNetwork.AdMob)
            MobileAds.Initialize(null);
        else if (adNetwork == AdNetwork.UnityAds)
        {
            //Advertisement.AddListener(this);
            Advertisement.Initialize(androidGameID, testMode);
        }

#elif UNITY_IOS

        unityAdsRewardedVideoID = iosRewardedVideoID;
        unityAdsBannerID = iosBannerID;

        if (adNetwork == AdNetwork.AdMob)
            MobileAds.Initialize(iOS_appID);
        else if (adNetwork == AdNetwork.UnityAds)
        {
            //Advertisement.AddListener(this);
            Advertisement.Initialize(iosGameID, testMode);
        }

#endif


        if (adNetwork == AdNetwork.AdMob)
        {
            RequestInterstitial();

            if (enableBanner)
                RequestBanner();

            // Get singleton reward based video ad reference.
            //this.rewardBasedVideo = RewardedAd.Instance;
            this.rewardBasedVideo = new RewardedAd(GetAdmobRewardedAdUnitId());

            // Called when an ad request has successfully loaded.
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // Called when an ad request failed to load.
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // Called when an ad is shown.
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;

            // Called when the ad starts to play.
            //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;

            // Called when the user should be rewarded for watching a video.
            rewardBasedVideo.OnUserEarnedReward += HandleRewardBasedVideoRewarded;

            // Called when the ad is closed.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

            // Called when the ad click caused the user to leave the application.
            //rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

            this.RequestRewardBasedVideo();
        }
        else if(adNetwork == AdNetwork.UnityAds)
        {
            if(enableBanner)
                StartCoroutine(ShowBannerWhenReady());
        }
    }

    public void ShowInterstitialAd()
    {
        if (!this.enabled) return;

        //Debug.Log("Increase Interstitial Counter");
        counter++;

        if (counter >= interstitialFrequency)
        {
            if (adNetwork == AdNetwork.AdMob)
                ShowInterstitial();
            else if (adNetwork == AdNetwork.UnityAds)
                Advertisement.Show("video");

            counter = 0;
        }
    }

    public void ShowBanner()
    {


        if (!this.enabled) return;

        if (adNetwork == AdNetwork.AdMob)
            bannerView.Show();
        else if (adNetwork == AdNetwork.UnityAds)
            Advertisement.Banner.Show(unityAdsBannerID);
    }

    public void HideBanner()
    {
        if (!this.enabled) return;

        if (adNetwork == AdNetwork.AdMob)
            bannerView.Hide();
        else if (adNetwork == AdNetwork.UnityAds)
            Advertisement.Banner.Hide();
    }

    #region Unity Ads

    /// <summary>
    /// Unity Ads Banner
    /// </summary>
    IEnumerator ShowBannerWhenReady()
    {
        BannerPosition bannerPos = GetBannerPosition();
        Advertisement.Banner.SetPosition(bannerPos);

        string placementId = androidBannerID;

        if (targetPlatform == TargetPlatform.IOS)
            placementId = iosBannerID;

        //while (!Advertisement.IsReady(placementId))
        //{
            yield return new WaitForSeconds(0.5f);
        //}
        
        Advertisement.Banner.Show(placementId);
    }

    private BannerPosition GetBannerPosition()
    {
        BannerPosition bannerPos;
        switch (bannerPosition)
        {
            case CustomBannerPosition.BOTTOM:
                bannerPos = BannerPosition.BOTTOM_CENTER;
                break;

            case CustomBannerPosition.TOP:
                bannerPos = BannerPosition.TOP_CENTER;
                break;

            default:
                bannerPos = BannerPosition.BOTTOM_CENTER;
                break;
        }

        return bannerPos;
    }

    private void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }

        RequestInterstitial();
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            if(placementId.Equals(androidRewardedVideoID) || placementId.Equals(iosRewardedVideoID))
            {
                onRewardVideoWatched?.Invoke();
                rewardVideoWatchedCallback?.Invoke();
            }
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    public void ShowRewardedVideo(UnityEvent rewardAction)
    {
        rewardVideoWatchedCallback = rewardAction;
        ShowRewardedVideo();
    }

    public void ShowRewardedVideo()
    {
        
        string rewardedVideoID = androidRewardedVideoID;

#if UNITY_IOS
        rewardedVideoID = iosRewardedVideoID;
#endif


        if (adNetwork == AdNetwork.AdMob)
            ShowAdmobRewardedVideo();
        else if (adNetwork == AdNetwork.UnityAds)
            Advertisement.Show(rewardedVideoID);
    }

    

    #endregion

    #region AdMob

    private void RequestInterstitial()
    {
        string adUnitId;
        if (testMode)
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS
               adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
               adUnitId = "unexpected_platform";
#endif
        }
        else
        {
#if UNITY_ANDROID
            adUnitId = interstitialId;
#elif UNITY_IOS
            adUnitId = iOS_interstitialID;
#else
            adUnitId = "unexpected_platform";
#endif
        }
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    private void RequestBanner()
    {
        string adUnitId;
        if (testMode)
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            adUnitId = "unexpected_platform";
#endif
        }
        else
        {
#if UNITY_ANDROID
            adUnitId = bannerId;
#elif UNITY_IPHONE
            adUnitId = iOS_bannerID;
#else
            adUnitId = "unexpected_platform";
#endif
        }

        // Create a 320x50 banner at the top of the screen.
        AdPosition adPosition = GetAdPosition();

        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, adPosition);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
        //bannerView.Show();
    }

    private AdPosition GetAdPosition()
    {
        AdPosition bannerPos;
        switch (bannerPosition)
        {
            case CustomBannerPosition.BOTTOM:
                bannerPos = AdPosition.Bottom;
                break;

            case CustomBannerPosition.TOP:
                bannerPos = AdPosition.Top;
                break;

            default:
                bannerPos = AdPosition.Bottom;
                break;
        }

        return bannerPos;

    }

    private void RequestRewardBasedVideo()
    {
        string adUnitId = GetAdmobRewardedAdUnitId();


        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request);
        
    }

    private void ShowAdmobRewardedVideo()
    {
        if (rewardBasedVideo.IsLoaded())
        {
            rewardBasedVideo.Show();
        }
    }

    private string GetAdmobRewardedAdUnitId()
    {
        string adUnitId;


        if (testMode)
        {
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif
        }
        else
        {
#if UNITY_ANDROID
            adUnitId = rewardedId;
#elif UNITY_IPHONE
            adUnitId = iOS_rewardedId;
#else
            adUnitId = "unexpected_platform";
#endif
        }

        return adUnitId;
    }


    #region Rewarded Video Callbacks

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {

    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {


    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {

    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {

    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        // Reward the user for watching the ad to completion.
        onRewardVideoWatched?.Invoke();
        rewardVideoWatchedCallback?.Invoke();
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {

    }

    #endregion



    public void HandleOnAdLoaded(object sender, EventArgs args)
    {

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {/*
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        */

        RequestInterstitial();
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        /*
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
        */

        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }

    #endregion
}
