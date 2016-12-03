using UnityEngine;
using System.Collections;

public class SpikeController : MonoBehaviour 
{
	public GameObject spikeSprite;
	public GameObject spikeShadowSprite;

	public float rotationTime;

	// is spike rotating clockwise
	public bool isClockWise;

	public void FixShadowPosition ()
	{
		spikeShadowSprite.transform.localPosition = 
			new Vector3(0.04f /  transform.localScale.x,
					   -0.04f / transform.localScale.x, 
						0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Quaternion rot = spikeSprite.transform.rotation;
		Vector3 rotation = rot.eulerAngles;
		rotation.z += (360f / rotationTime) * Time.smoothDeltaTime * (isClockWise ? -1 : 1);

		rot.eulerAngles = rotation;

		spikeSprite.transform.rotation = rot;
		spikeShadowSprite.transform.rotation = rot;
	}
}
