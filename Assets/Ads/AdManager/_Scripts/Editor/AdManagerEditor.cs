using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AdManager))]
public class AdManagerEditor : Editor
{
    AdManager am;
    GUIStyle centeredStyle;

    private void OnEnable()
    {
        if (am == null)
            am = (AdManager)target;

        centeredStyle = new GUIStyle();
        centeredStyle.fontSize = 14;
        centeredStyle.fontStyle = FontStyle.BoldAndItalic;
        centeredStyle.alignment = TextAnchor.UpperCenter;
    }

    public override void OnInspectorGUI()
    {
        if (am.testMode)
        {
            EditorGUILayout.HelpBox("You Are Currently In Test Mode", MessageType.Info);
            DrawUILine(1);
        }


        EditorGUILayout.LabelField("Main Settings", centeredStyle);
        GUILayout.Space(20);


        am.testMode = EditorGUILayout.Toggle("TestMode", am.testMode);
        

        am.targetPlatform = (AdManager.TargetPlatform)EditorGUILayout.EnumPopup("Target", am.targetPlatform);
        am.adNetwork = (AdManager.AdNetwork)EditorGUILayout.EnumPopup("Ad Network", am.adNetwork);

        GUILayout.Space(20);
        DrawUILine(1);

        EditorGUILayout.LabelField("Interstitial Frequency", centeredStyle);
        am.interstitialFrequency = EditorGUILayout.IntSlider(am.interstitialFrequency, 1, 5);

        GUILayout.Space(10);

        am.enableBanner = EditorGUILayout.Toggle("Enable Banner", am.enableBanner);

        if (am.enableBanner)
            ShowBannerPositionSetting();

        GUILayout.Space(10);
        DrawUILine(1);


        if (am.adNetwork == AdManager.AdNetwork.AdMob)
        {
            ShowAdmobSettings();
        }
        else if (am.adNetwork == AdManager.AdNetwork.UnityAds)
        {
            ShowUnityAdsSettings();
        }





        if (GUI.changed)
        {
            EditorUtility.SetDirty(am);
        }

        //base.OnInspectorGUI();
    }

    private void ShowAdmobSettings()
    {
        if (am.targetPlatform == AdManager.TargetPlatform.Android)
        {
            EditorGUILayout.LabelField("Android Settings", centeredStyle);
            GUILayout.Space(20);

            am.appId = EditorGUILayout.TextField("App ID", am.appId);
            am.interstitialId = EditorGUILayout.TextField("Interstitial ID", am.interstitialId);

            if (am.enableBanner)
                am.bannerId = EditorGUILayout.TextField("Banner ID", am.bannerId);

            am.rewardedId = EditorGUILayout.TextField("Rewarded Video ID", am.rewardedId);
        }
        else if (am.targetPlatform == AdManager.TargetPlatform.IOS)
        {
            EditorGUILayout.LabelField("IOS Settings", centeredStyle);
            GUILayout.Space(20);

            am.iOS_appID = EditorGUILayout.TextField("iOS App ID", am.iOS_appID);
            am.iOS_interstitialID = EditorGUILayout.TextField("iOS Interstitial ID", am.iOS_interstitialID);

            if (am.enableBanner)
                am.iOS_bannerID = EditorGUILayout.TextField("iOS Banner ID", am.iOS_bannerID);

            am.iOS_rewardedId = EditorGUILayout.TextField("Rewarded Video ID", am.iOS_rewardedId);
        }
    }

    private void ShowUnityAdsSettings()
    {
        if (am.targetPlatform == AdManager.TargetPlatform.Android)
        {
            EditorGUILayout.LabelField("Android Settings", centeredStyle);
            GUILayout.Space(20);

            am.androidGameID = EditorGUILayout.TextField("Android Game ID", am.androidGameID);

            if(am.enableBanner)
                am.androidBannerID = EditorGUILayout.TextField("Android Banner ID", am.androidBannerID);
            

            am.androidRewardedVideoID = EditorGUILayout.TextField("Android Rewarded Video ID", am.androidRewardedVideoID);
        }
        else if (am.targetPlatform == AdManager.TargetPlatform.IOS)
        {
            EditorGUILayout.LabelField("IOS Settings", centeredStyle);
            GUILayout.Space(20);

            am.iosGameID = EditorGUILayout.TextField("IOS Game ID", am.iosGameID);

            if(am.enableBanner)
                am.iosBannerID = EditorGUILayout.TextField("IOS Banner ID", am.iosBannerID);

            am.iosRewardedVideoID = EditorGUILayout.TextField("IOS Rewarded Video ID", am.iosRewardedVideoID);
        }
    }

    private void ShowBannerPositionSetting()
    {
        am.bannerPosition = (AdManager.CustomBannerPosition)EditorGUILayout.EnumPopup("Banner Position", am.bannerPosition);
    }

#if UNITY_EDITOR
    public static void DrawUILine(int thickness = 2, int padding = 30)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, Color.white);
    }
#endif
}
#endif