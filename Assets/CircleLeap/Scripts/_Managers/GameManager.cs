using UnityEngine;
using System;

public class GameManager : Singleton<GameManager> 
{
	// game state change listener
	public event Action<State> gameStateChangeEvent;

	private State gameState;

	public enum State
	{
		MainScreen,
		Playing,
		Failed,
		Paused,
	}

	public State GameState
	{
		get 
		{
			return gameState;
		}
		set
		{
			if (gameState == value)
			{
				return;
			}

			gameState = value;

			if(gameStateChangeEvent != null)
			{
				gameStateChangeEvent(gameState);
			}
		}
	}

	void Awake () 
	{
		gameState = State.MainScreen;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		GameManager.Instance.gameStateChangeEvent += (GameManager.State _gameState) => {

			switch(_gameState)
			{
			case GameManager.State.Failed:
				Invoke("ShowAd", 1.5f);
				break;
			}
		};
	}

	public void ShowAd()
	{
		if(AdsManager.Instance != null)
		{
			AdsManager.Instance.RequestAd();
		}
	}

	public void PauseGame()
	{
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1;
	}
}