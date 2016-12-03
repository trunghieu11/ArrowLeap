using UnityEngine;
using System.Collections;

public class ScreenshotRecorder : MonoBehaviour 
{
	public string folder = "ScreenshotFolder";
	public int frameRate = 60;
	private int counter = 0;

	void Start() 
	{
		//Time.captureFramerate = frameRate;
		System.IO.Directory.CreateDirectory(folder);
	}

	void Update()
	{
		if(counter % 30 == 0)
		{
			string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
			Application.CaptureScreenshot(name);
		}
		counter++;
	}
}