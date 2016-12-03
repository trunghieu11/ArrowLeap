using UnityEngine;
using System.Collections;
using System;

public class TriggerDetector : MonoBehaviour 
{
	public LayerMask _layerMask;

	public event Action<Collider2D> TriggerEnterEvent;
	public event Action<Collider2D> TriggerExitEvent;


	void OnTriggerEnter2D(Collider2D other)
	{
		if (!_layerMask.ContainsLayer(other.gameObject.layer)) 
		{
		//	return;
		}

		if (TriggerEnterEvent != null) 
		{
			TriggerEnterEvent(other);
		}
	}

	void OnTriggerExit2D(Collider2D other) 
	{
		if (!_layerMask.ContainsLayer(other.gameObject.layer)) 
		{
			return;
		}

		if (TriggerExitEvent != null) 
		{
			TriggerExitEvent(other);
		}
	}
}