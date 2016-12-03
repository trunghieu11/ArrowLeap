using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager> 
{
	public bool TouchDown 
	{
		get 
		{
			if (Input.GetMouseButtonDown(0))
			{
				// if pointer is not over ui element return true
				if(!EventSystem.current.IsPointerOverGameObject())
				{
					return true;
				}
			} 

			return false;
		}
	}
}