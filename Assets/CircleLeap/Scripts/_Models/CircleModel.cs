using UnityEngine;
using System;

public class CircleModel 
{
	// four types of cycles of different size
	public enum CircleType
	{
		CIRCLE_1,
		CIRCLE_2,
		CIRCLE_3,
		CIRCLE_4
	}

	public Vector2 position { get; set; }
	public CircleType circleType { get; set; }

	public CircleModel(Vector2 position, CircleType circleType) 
	{
		this.position = position;
		this.circleType = circleType;
	}

	public CircleModel(Vector2 position, string circleType) 
	{
		this.position = position;
		this.circleType = (CircleType)Enum.Parse (typeof(CircleModel.CircleType), circleType, true);
	}
}