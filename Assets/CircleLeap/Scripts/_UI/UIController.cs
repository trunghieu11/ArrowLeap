using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : Singleton<UIController> 
{
	public Animator mainScreenAnimator;
	public Animator tapToBeginAnimatior;

	public GameObject mainScreenUI;
	public GameObject hud;
	public GameObject pausedScreen;
	public GameObject tapToBeginText;

	public GameObject replayButton;
	public GameObject playButton;
	public GameObject score;

	public Text bestScoreText;
	public Text currentScoreText;

	public GameObject level;

	public Button soundButton;
	private Sprite soundEnabledTexture;
	private Sprite soundDisabledTexture;

	// Use this for initialization
	void Start ()
	{
		soundEnabledTexture = Resources.Load<Sprite>("Sound");
		soundDisabledTexture = Resources.Load<Sprite>("SoundDisabled");

		soundButton.image.sprite = SoundManager.Instance.SoundEnabled ? soundEnabledTexture : soundDisabledTexture;

		GameManager.Instance.gameStateChangeEvent += UpdateUI;
	}

	void Update()
	{
		if (InputManager.Instance.TouchDown)
		{
			tapToBeginAnimatior.SetTrigger("HideText");
		}
	}

	void UpdateUI(GameManager.State gameState)
	{
		switch(gameState)
		{
		case GameManager.State.MainScreen:
			ShowMainScreen();
			break;
		case GameManager.State.Failed:
			Invoke("ShowGameFinishedScreen", 1.5f);
			break;
		}
	}

	void ShowMainScreen()
	{
		hud.SetActive(false);
		tapToBeginText.SetActive(false);

		replayButton.SetActive(false);
		playButton.SetActive(true);
		score.SetActive(false);

		mainScreenAnimator.SetTrigger("MainScreen");
	}

	public void ShowGameFinishedScreen()
	{
		mainScreenUI.SetActive(true);
		hud.SetActive(false);
		tapToBeginText.SetActive(false);

		replayButton.SetActive(true);
		playButton.SetActive(false);
		score.SetActive(true);
		mainScreenAnimator.SetTrigger("GameFinished");

		level.SetActive(false);

		bestScoreText.text = "Best: " + ScoreManager.Instance.BestScore;

		StartCoroutine(ShowCurrentScore(ScoreManager.Instance.CurrentScore));
	}

	IEnumerator ShowCurrentScore(int score)
	{
		int _score = 0;
		while(_score <= score)
		{
			currentScoreText.text = "Current: " + _score;
			_score += Mathf.Max( 1, Mathf.FloorToInt(score / 7));
			yield return new WaitForSeconds(0.05f);
		}
		currentScoreText.text = "Current: " + score;
	}
		
	public void ShowPauseScreen()
	{
		mainScreenUI.SetActive(false);
		hud.SetActive(false);
		pausedScreen.SetActive(true);
	}

	public void ResumeGame()
	{
		mainScreenUI.SetActive(false);
		hud.SetActive(true);
		pausedScreen.SetActive(false);
	}

	public void SoundButtonPressed()
	{
		SoundManager.Instance.EnableDisableSound(
			(isEnabled) => {
				soundButton.image.sprite = isEnabled ? soundEnabledTexture : soundDisabledTexture;
			}
		);
	}

	public void PlayButtonPressed() 
	{
		mainScreenUI.SetActive(false);
		hud.SetActive(true);
		tapToBeginText.SetActive(true);

		level.SetActive(true);
		GameManager.Instance.GameState = GameManager.State.Playing;
	}
}