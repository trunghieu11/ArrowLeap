using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager> 
{
	private const string _bestScore = "BestScore";

	private int _currentScore;

	public Text scoreText;

	void Start()
	{
		GameManager.Instance.gameStateChangeEvent += (GameManager.State state) => {

			switch(state)
			{
			case GameManager.State.Playing:
				_currentScore = 0;
				CurrentScore = 0;
				break;
			case GameManager.State.Failed:
				if(_currentScore > BestScore)
					BestScore = _currentScore;
				break;
			}
		};
	}

	public int BestScore
	{
		get
		{
			return PlayerPrefs.GetInt(_bestScore, 0);
		}
		set
		{
			PlayerPrefs.SetInt(_bestScore, value);
		}
	}

	public int CurrentScore
	{
		get
		{
			return _currentScore;
		}
		set
		{
			if(value >= _currentScore)
			{
				_currentScore = value;
				scoreText.text = _currentScore.ToString();
			}
		}
	}

	public void DisplayScore(int score)
	{
		StartCoroutine(UpdateScore(score));
	}

	private IEnumerator UpdateScore(int score)
	{
		while(_currentScore < score)
		{
			CurrentScore = _currentScore + 1;
			yield return new WaitForSeconds(0.05f);
		}
	}
}