using UnityEngine;

public class SpikeModel
{
	public Vector2 position { get; set; }
	public float scale;

	public SpikeModel(Vector2 position, float scale)
	{
		this.position = position;
		this.scale = scale;
	}
}