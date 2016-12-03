using UnityEngine;

public class GenerateJson : MonoBehaviour 
{
	public TextAsset levelJsonFile;

	public GameObject[] levelPieces;

	public string GenerateLevelJson ()
	{
		string json = "{\n\t\"LevelPieces\":\n\t[\n";

		foreach(GameObject piece in levelPieces)
		{
			Transform levelPiece = Object.Instantiate(piece).transform;
			levelPiece.gameObject.SetActive(false);
			levelPiece.position = new Vector3(0, 0, 0);
			levelPiece.transform.parent = transform;

			json += "\n\t\t{";

			Transform topBorder = levelPiece.FindChild("BorderTop");
			Transform bottomBorder = levelPiece.FindChild("BorderBottom");

			Transform circles = levelPiece.FindChild("Circles");
			Transform spikes = levelPiece.FindChild("Obstacles");

			json += "\n\t\t\t\"TopBorder\"" + " : " + topBorder.position.y + ", \n";
			json += "\n\t\t\t\"BottomBorder\"" + " : " + bottomBorder.position.y + ", \n";

			json += "\n\t\t\t\"Circles\":\n\t\t\t[";

			foreach(Transform circle in circles)
			{
				if(!circle.gameObject.activeSelf) 
				{
					continue;
				}

				if(circle.CompareTag("Circle"))
				{
					json += "\n\t\t\t\t{";

					json += "\n\t\t\t\t\t\"Position\"" + ":";
					json += "\n\t\t\t\t\t\t{";
					json += "\n\t\t\t\t\t\t\t \"x\": " + circle.transform.position.x + ", ";
					json += "\n\t\t\t\t\t\t\t \"y\": "  + circle.transform.position.y;
					json += "\n\t\t\t\t\t\t},";

					json += "\n\t\t\t\t\t\"Type\"" + ": ";

					switch(circle.GetComponent<SpriteRenderer>().sprite.name.ToLower())
					{
					case "circle1":
						json += "\"CIRCLE_1\"";
						break;
					case "circle2":
						json += "\"CIRCLE_2\"";
						break;
					case "circle3":
						json += "\"CIRCLE_3\"";
						break;
					case "circle4":
						json += "\"CIRCLE_4\"";
						break;
					}

					json += "\n\t\t\t\t},";
				}
			}
			json = json.Substring(0, json.Length - 1);

			json += "\n\t\t\t],";


			json += "\n\t\t\t\"Spikes\":\n\t\t\t[";

			foreach(Transform spike in spikes)
			{
				if(!spike.gameObject.activeSelf) 
				{
					continue;
				}

				json += "\n\t\t\t\t{";

				json += "\n\t\t\t\t\t\"Position\"" + ":";
				json += "\n\t\t\t\t\t\t{";
				json += "\n\t\t\t\t\t\t\t \"x\": " + spike.transform.position.x + ", ";
				json += "\n\t\t\t\t\t\t\t \"y\": "  + spike.transform.position.y;
				json += "\n\t\t\t\t\t\t},";

				json += "\n\t\t\t\t\t\"Scale\"" + ": " + spike.transform.localScale.x;

				json += "\n\t\t\t\t},";
			}

			json = json.Substring(0, json.Length - 1);

			json += "\n\t\t\t]";

			json += "\n\t\t},";

			DestroyImmediate(levelPiece.gameObject);
		}

		json = json.Substring(0, json.Length - 1);

		json += "\n\t]\n}\n";
	
		return json;
	}
}
