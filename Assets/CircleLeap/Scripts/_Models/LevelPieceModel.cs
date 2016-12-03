using System.Collections.Generic;

public class LevelPieceModel 
{
	public List<CircleModel> circles { get; set; }
	public List<SpikeModel> spikes { get; set; }

	// level piece top border Y position
	public float topBorderY { get; set; }

	// level piece bottom border Y position
	public float bottomBorderY { get; set; }

	public int levelPieceIndex { get; set; }
	public bool isEnabled { get; set; }

	public LevelPieceModel()
	{
		circles = new List<CircleModel>();
		spikes = new List<SpikeModel>();
	}
}