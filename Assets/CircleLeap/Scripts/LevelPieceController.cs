using UnityEngine;
using System.Collections.Generic;

public class LevelPieceController : MonoBehaviour 
{
	public TriggerDetector topBorder;
	public GameObject bottomBorder;

	public LevelPieceModel levelPieceModel { get; set; }

	public List<CircleController> circles { get; set; }
	public List<SpikeController> spikes { get; set; }

	public int levelPieceIndex
	{ 
		get
		{
			return levelPieceModel.levelPieceIndex;
		}
		set
		{
			levelPieceModel.levelPieceIndex = value;
		}
	}

	void Awake()
	{
		circles = new List<CircleController>();
		spikes = new List<SpikeController>();

		topBorder.TriggerEnterEvent += (Collider2D collider) => {

			if(collider.gameObject.CompareTag("LevelPieceKiller"))
			{
				LevelLoader.Instance.RecycleBottomLevelPiece(this);
			}
		};
	}

	public void RecyclePiece()
	{
		for(int i = 0; i < circles.Count; i++)
		{
			if(circles[i] != null)
			{
				circles[i].gameObject.Recycle();
			}
		}
			
		for(int i = 0; i < spikes.Count; i++)
		{
			if(spikes[i] != null)
			{
				spikes[i].gameObject.Recycle();
			}
		}

		gameObject.Recycle();

		Reset();
	}

	public void Reset()
	{
		circles.Clear();
		spikes.Clear();
	}
}