using UnityEngine;
using System;

#if GOOGLE_ADMOB
using GoogleMobileAds.Api;
#endif

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager : Singleton<AdsManager>  
{
	private const string AdCounter = "AdCounter";

	public enum AdType
	{
		NO_ADS,
		UNITY_ADS,
		ADMOB
	}

	public AdType adType;
	public bool showAdOnStart;

	private int adCounter = 0;

	// after how many plays show ad
	[Range(1, 50)]
	public int showAdAfterPlays;

	#if GOOGLE_ADMOB
	private InterstitialAd admobInterstitial;
	#endif

	public string admobAndroidInterstitialAdUnityId;
	public string admobIOSInterstitialAdUnityId;

	// Use this for initialization
	void Start () 
	{
		#if GOOGLE_ADMOB
		LoadAdmobInterstitial();
		#endif

		if(showAdOnStart)
		{
			if(adType == AdType.ADMOB)
			{
				#if GOOGLE_ADMOB
				admobInterstitial.OnAdLoaded += (object sender, EventArgs e) => ShowAd();
				#endif
			}
			else
			{
				ShowAd();
			}
		}
	}

	#if GOOGLE_ADMOB
	private void LoadAdmobInterstitial()
	{
		string adUnitId;

		#if UNITY_ANDROID
		adUnitId = admobAndroidInterstitialAdUnityId;
		#elif UNITY_IPHONE
		adUnitId = admobiOSInterstitialAdUnityId;
		#else
		adUnitId = "unexpected_platform";
		#endif

		admobInterstitial = new InterstitialAd(adUnitId);

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		
		// Load the interstitial with the request.
		admobInterstitial.LoadAd(request);
		
	}
	#endif

	// show ad if number of plays >= showAdAfterPlays
	public void RequestAd()
	{
		adCounter = PlayerPrefs.GetInt(AdCounter, 0);

		if(adCounter >= showAdAfterPlays)
		{
			ShowAd();
		}

		// save counter
		PlayerPrefs.SetInt(AdCounter, ++adCounter);
	}

	private void ShowAd()
	{
		// reset counter
		adCounter = 0;
		PlayerPrefs.SetInt(AdCounter, adCounter);

		switch(adType)
		{
		case AdType.ADMOB:
			#if GOOGLE_ADMOB
			if (admobInterstitial.IsLoaded())
			{
				admobInterstitial.Show();
				LoadAdmobInterstitial();
			}
			#endif
			break;
		case AdType.UNITY_ADS:
			#if UNITY_ADS
			if (Advertisement.IsReady())
			{
				Advertisement.Show();
			}
			#endif
			break;
		}
	}
}