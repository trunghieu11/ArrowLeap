using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GenerateJson))]
public class GenerateJsonEditor : Editor
{
	private string path;

	public override void OnInspectorGUI()
	{
		GenerateJson generateJsonScript = (GenerateJson)target;
		path = AssetDatabase.GetAssetPath(generateJsonScript.levelJsonFile);

		base.DrawDefaultInspector();

		if(GUILayout.Button("Generate Json"))
		{
			SaveLevelJson(generateJsonScript.GenerateLevelJson());
		}
	}

	public void SaveLevelJson(string levelJson)
	{
		using (FileStream fs = new FileStream(path, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(fs))
			{
				writer.Write(levelJson);
			}
		}
		
		UnityEditor.AssetDatabase.Refresh ();
	}
}