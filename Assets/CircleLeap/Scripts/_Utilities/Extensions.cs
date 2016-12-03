using UnityEngine;
using System.Collections.Generic;

public static class Extensions
{
	public static void SetX(this Transform transform, float x)
	{
		Vector3 newPosition = new Vector3(x, transform.position.y, transform.position.z);
		transform.position = newPosition;
	}

	public static void SetY(this Transform transform, float y)
	{
		Vector3 newPosition = new Vector3(transform.position.x, y, transform.position.z);
		transform.position = newPosition;
	}

	public static void SetZ(this Transform transform, float z)
	{
		Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, z);
		transform.position = newPosition;
	}

	public static void SetRotationX(this Transform transform, float x)
	{
		Quaternion newRotation = Quaternion.Euler(x, transform.rotation.y, transform.rotation.z);// new Vector3(x, transform.rotation.y, transform.rotation.z);
		transform.rotation = newRotation;
	}

	public static void SetRotationY(this Transform transform, float y)
	{
		Quaternion newRotation = Quaternion.Euler( transform.rotation.x, y, transform.rotation.z);
		transform.rotation = newRotation;
	}

	public static void SetRotationZ(this Transform transform, float z)
	{
		Quaternion newRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, z);
		transform.rotation = newRotation;
	}

	public static bool ContainsLayer(this LayerMask layerMask, int layer) 
	{
		return (layerMask.value & (1 << layer)) != 0;
	}

	public static void Shuffle<T>(this IList<T> list)  
	{  
		System.Random rng = new System.Random();  

		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
