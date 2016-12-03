using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float rotationTime;
	public bool isClockWise;

	// Update is called once per frame
	void Update () 
	{
		Quaternion rot = transform.rotation;
		Vector3 rotation = rot.eulerAngles;
		rotation.z += (360f / rotationTime) * Time.smoothDeltaTime * (isClockWise ? -1 : 1);

		rot.eulerAngles = rotation;

		transform.rotation = rot;
	}
}
