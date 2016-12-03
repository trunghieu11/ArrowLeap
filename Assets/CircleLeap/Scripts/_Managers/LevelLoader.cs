using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : Singleton<LevelLoader> 
{
	// list of level piece models leaded from json file.
	private List<LevelPieceModel> levelPieceModels;

	// list of currently displayed level pieces.
	private List<LevelPieceController> loadedLevelPieces = new List<LevelPieceController>();

	// list of (levelPieceIndex, position) pairs of recycled level pieces.
	private List<KeyValuePair<int, float>> recycledLevelPieces = new List<KeyValuePair<int, float>>{};

	// is level already initialized
	private bool isInitialized = false;

	// triggers for detecting collistions and loading/recycling level pieces.
	public TriggerDetector topPieceBottom;
	public TriggerDetector bottomPieceTop;

	public LevelPieceController levelPiecePrefab;

	public CircleController circle1Prefab;
	public CircleController circle2Prefab;
	public CircleController circle3Prefab;
	public CircleController circle4Prefab;

	public SpikeController spikePrefab;

	void Start()
	{
		// create prefab pools to avoid runtime initialization
		InitPrefabsPool();

		// load level model from json file
		LoadLevelJSON();

		InitializeLevel();

		GameManager.Instance.gameStateChangeEvent += (GameManager.State state)  => {

			if(state == GameManager.State.Failed)
			{
				Invoke("ClearLevel", 1.5f);
			}
			else if(state == GameManager.State.Playing)
			{
				InitializeLevel();
			}
		};

		topPieceBottom.TriggerEnterEvent += (Collider2D collider) => {

			if(collider.gameObject.CompareTag("WorldTopTrigger"))
			{
				LevelLoader.Instance.LoadNextLevelPiece();
			}
		};

		bottomPieceTop.TriggerEnterEvent += (Collider2D collider) => {

			if(collider.gameObject.CompareTag("WorldBottomTrigger"))
			{
				ArrowController arrow = ArrowController.Instance;

				// if arrow is going down load previous piece
				if(arrow.circle == null && arrow.rb.velocity.y < 0)
				{
					LevelLoader.Instance.LoadPreviousLevelPiece();
				}
			}
		};
	}

	void InitPrefabsPool()
	{
		levelPiecePrefab.CreatePool(7);

		circle1Prefab.CreatePool(10);
		circle2Prefab.CreatePool(10);
		circle3Prefab.CreatePool(10);
		circle4Prefab.CreatePool(10);

		spikePrefab.CreatePool(20);
	}

	// load level piece models from json file
	public void LoadLevelJSON()
	{
		levelPieceModels = new List<LevelPieceModel>();
		string jsonString = Resources.Load<TextAsset>("level").text;
		JSONObject levelJson = new JSONObject(jsonString);

		int levelPieceIndex = 0;

		foreach(JSONObject _levelPiece in levelJson["LevelPieces"].list)
		{
			LevelPieceModel levelPieceModel = new LevelPieceModel();
			levelPieceModels.Add(levelPieceModel);

			levelPieceModel.levelPieceIndex = levelPieceIndex++;

			levelPieceModel.topBorderY = _levelPiece["TopBorder"].n;
			levelPieceModel.bottomBorderY = _levelPiece["BottomBorder"].n;

			List<CircleModel> cirlceModels = levelPieceModel.circles;
			List<SpikeModel> spikeModels = levelPieceModel.spikes;

			foreach(JSONObject circle in _levelPiece["Circles"].list)
			{
				float posX = circle["Position"]["x"].n;
				float posY = circle["Position"]["y"].n;

				cirlceModels.Add(new CircleModel(new Vector2(posX, posY), circle["Type"].str));
			}

			foreach(JSONObject spike in _levelPiece["Spikes"].list)
			{
				float posX = spike["Position"]["x"].n;
				float posY = spike["Position"]["y"].n;

				float scale = spike["Scale"].n;

				spikeModels.Add(new SpikeModel(new Vector2(posX, posY), scale));
			}
		}
	}

	public void InitializeLevel()
	{
		isInitialized = false;

		// clear loaded level pieces
		loadedLevelPieces.Clear();

		// randomize order of level pieces
		RandomizeLevel();

		// initialize 3 pieces on start
		LoadLevelPieces(new List<int> { 0, 1, 2 }, false);
	
		isInitialized = true;
	}


	public void LoadLevelPieces(List<int> levelPieceIndices, bool loadRecycledPieces)
	{
		for(int i = 0; i < levelPieceIndices.Count; i++)
		{
			int pieceIndex = levelPieceIndices[i];

			LevelPieceModel levelPieceModel = levelPieceModels[pieceIndex];
			levelPieceModel.isEnabled = true;

			LevelPieceController levelPiece = levelPiecePrefab.Spawn(transform, Vector3.zero, Quaternion.identity);
			levelPiece.Reset();
			levelPiece.levelPieceModel = levelPieceModel;

			levelPiece.topBorder.transform.localPosition = new Vector3(0, levelPieceModel.topBorderY, 0);
			levelPiece.bottomBorder.transform.localPosition = new Vector3(0, levelPieceModel.bottomBorderY, 0);

			Transform levelPieceTransform = levelPiece.gameObject.transform;

			if((loadRecycledPieces || !isInitialized) && i == 0)
			{
				bottomPieceTop.transform.position = levelPiece.topBorder.transform.position;
			}

			if(loadRecycledPieces)
			{
				float piecePos = recycledLevelPieces[recycledLevelPieces.Count - 1].Value;
				recycledLevelPieces.RemoveAt(recycledLevelPieces.Count - 1);

				levelPieceTransform.Translate(new Vector3(0, piecePos, 0));
				loadedLevelPieces.Insert(0, levelPiece);

				bottomPieceTop.transform.position = levelPiece.topBorder.transform.position;
			}
			else
			{
				if(loadedLevelPieces.Count != 0)
				{
					LevelPieceController topPiece = loadedLevelPieces[loadedLevelPieces.Count - 1];
					levelPieceTransform.Translate(new Vector3(0, topPiece.topBorder.transform.position.y + 2, 0));
				}
				else
				{
					levelPieceTransform.Translate(new Vector3(0, 0, 0));
				}

				if(i == levelPieceIndices.Count - 1)
				{
					topPieceBottom.transform.position = levelPiece.bottomBorder.transform.position;
				}

				loadedLevelPieces.Add(levelPiece);
			}

			// create cicles
			foreach(CircleModel circle in levelPieceModel.circles)
			{
				CircleController circleInstance = null;
				switch(circle.circleType)
				{
					case CircleModel.CircleType.CIRCLE_1:
					{
						circleInstance = circle1Prefab.Spawn(levelPieceTransform, circle.position, Quaternion.identity);
						break;
					}
					case CircleModel.CircleType.CIRCLE_2:
					{
						circleInstance = circle2Prefab.Spawn(levelPieceTransform, circle.position, Quaternion.identity);
						break;
					}
					case CircleModel.CircleType.CIRCLE_3:
					{
						circleInstance = circle3Prefab.Spawn(levelPieceTransform, circle.position, Quaternion.identity);
						break;
					}
					case CircleModel.CircleType.CIRCLE_4:
					{
						circleInstance = circle4Prefab.Spawn(levelPieceTransform, circle.position, Quaternion.identity);
						break;
					}
				}
				circleInstance.Reset();

				levelPiece.circles.Add(circleInstance);
			}

			// create spikes
			foreach(SpikeModel spike in levelPieceModel.spikes)
			{
				SpikeController spikeInstance = spikePrefab.Spawn(levelPieceTransform, spike.position, Quaternion.identity);
				spikeInstance.transform.localScale = new Vector3(spike.scale, spike.scale, 1);
				spikeInstance.FixShadowPosition();

				levelPiece.spikes.Add(spikeInstance);
			}
		}
	}
		
	public void LoadPreviousLevelPiece()
	{
		if(recycledLevelPieces.Count == 0)
			return;

		KeyValuePair<int, float> lastRecycledLevelPiece = recycledLevelPieces[recycledLevelPieces.Count - 1];

		for(int i = 0; i < levelPieceModels.Count; i++)
		{
			if(levelPieceModels[i].levelPieceIndex == lastRecycledLevelPiece.Key)
			{
				LoadLevelPieces(new List<int> { i }, true);
				break;
			}
		}
	}

	public void LoadNextLevelPiece()
	{
		int lastLevelPieceIndex = GetLastLevelPieceIndex();

		if(lastLevelPieceIndex != levelPieceModels.Count - 1)
		{
			LoadLevelPieces(new List<int> { lastLevelPieceIndex + 1 }, false);
		}
		else
		{
			RandomizeLevel();
			LoadLevelPieces(new List<int> { 0 }, false);
		}
	}

	public void RecycleBottomLevelPiece(LevelPieceController levelPiece)
	{
		foreach(LevelPieceModel levelPieceModel in levelPieceModels)
		{
			if(levelPieceModel.levelPieceIndex == levelPiece.levelPieceIndex)
			{
				levelPieceModel.isEnabled = false;
				break;
			}
		}

		// save level piece position to load it again in the same position when arrow is going down
		recycledLevelPieces.Add(new KeyValuePair<int, float>(levelPiece.levelPieceIndex, levelPiece.gameObject.transform.position.y));

		// remove level piece from the list of currently loaded pieces
		loadedLevelPieces.RemoveAt(0);

		// recycle leve piece prefab
		levelPiece.RecyclePiece();

		// since bottom piece is changed update bottomPieceTop position
		bottomPieceTop.transform.position = loadedLevelPieces[0].topBorder.transform.position;
	}

	// randomize order of level pieces
	public void RandomizeLevel()
	{
		levelPieceModels.Shuffle();
	}

	// recycle prefabs
	public void ClearLevel()
	{
		levelPiecePrefab.RecycleAll();

		circle1Prefab.RecycleAll();
		circle2Prefab.RecycleAll();
		circle3Prefab.RecycleAll();
		circle4Prefab.RecycleAll();

		spikePrefab.RecycleAll();
	}

	// get index of last loaded level piece
	public int GetLastLevelPieceIndex()
	{
		for(int i = levelPieceModels.Count - 1; i >=0; i--)
		{
			if(levelPieceModels[i].isEnabled)
			{
				return i;
			}
		}
		return 0;
	}
}