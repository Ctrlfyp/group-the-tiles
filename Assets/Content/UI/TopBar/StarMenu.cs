using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Events;
using System;


public class StarMenu : MonoBehaviour
{
    public UI.DialogSystem dialogSystem;
    private RewardedAd rewardedAd;


    void Start()
    {
        CreateAndLoadRewardedAd();
    }


    private void CreateAndLoadRewardedAd()
    {
        this.rewardedAd = new RewardedAd("ca-app-pub-3940256099942544/5224354917");

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

        this.rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
        this.rewardedAd.OnAdClosed += OnRewardedAdClosed;
        this.rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
    }

    private void OnRewardedAdLoaded(object sender, EventArgs args)
    {
    }

    private void OnRewardedAdClosed(object sender, EventArgs args)
    {
        CreateAndLoadRewardedAd();
    }

    private void OnUserEarnedReward(object sender, Reward reward)
    {
        // Save the stuff
        GameManager.saveManager.AddCurrency((int)reward.Amount);

        // Show the stuff
        string coinMessage = TranslationSystem.GetText("UIMainMenu", "MainMenuRewardEarnedMessage", new object[] { (int)reward.Amount });
        ShowDialogPrompt(coinMessage);
    }

    public void ShowDialogPrompt(string text, UnityAction yesAction = null, UnityAction noAction = null)
    {
        UI.DialogSystem ds = Instantiate(dialogSystem, this.transform).GetComponent<UI.DialogSystem>();
        ds.SetDisplayText(text, yesAction, noAction);
    }


    public void OnEarnCurrencyClicked()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

}
