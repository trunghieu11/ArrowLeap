using UnityEngine;
using System.Collections;

public class CameraController : Singleton<CameraController> 
{
	public float cameraMovingSpeed;
	public float offset;

	// How long the object should shake for.
	public float shakeDuration = 1f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;

	public ArrowController arrow;

	void Start() 
	{		
		StartCoroutine(ChangeColor(new Color[] {
				new Color(189 / 255f, 38  / 255f, 27  / 255f),
				new Color(242 / 255f, 110 / 255f, 55  / 255f),
				new Color(209 / 255f, 188 / 255f, 49  / 255f),
				new Color(52  / 255f, 123 / 255f, 36  / 255f),
				new Color(66  / 255f, 187 / 255f, 249 / 255f),
				new Color(75  / 255f, 85  / 255f, 222 / 255f),
				new Color(144 / 255f, 40  / 255f, 193 / 255f),
				new Color(222 / 255f, 84  / 255f, 170 / 255f),
				new Color(249 / 255f, 89  / 255f, 132 / 255f)
			}
		));
	}

	void LateUpdate() 
	{
		if(arrow.circle == null && arrow.outOfBorder == true)
		{
			Vector3 pos = transform.position;
			pos.y = arrow.transform.position.y;

			transform.position = Vector3.Lerp(transform.position, pos, 6 * Time.deltaTime);
		}
		else if( arrow.circle != null)
		{
			Vector3 pos = transform.position;
			pos.y = arrow.circle.transform.position.y + offset;

			transform.position = Vector3.Lerp(transform.position, pos, cameraMovingSpeed * Time.deltaTime);
		}
	}

	private IEnumerator ChangeColor(Color[] colors)
	{
		int currentIndex = UnityEngine.Random.Range(0, colors.Length - 1);
		Camera.main.backgroundColor = colors[currentIndex];
		currentIndex = (currentIndex + 1) % colors.Length;
		yield return null;

		Color prevColor;
		Color nextColor;

		float lerpT = 0;

		while(true)
		{
			prevColor = colors[(currentIndex + colors.Length - 1) % colors.Length];
			nextColor = colors[currentIndex];

			while(lerpT <= 1)
			{
				lerpT += Time.deltaTime / 10;
				Camera.main.backgroundColor = Color.Lerp(prevColor, nextColor, Mathf.PingPong(lerpT, 1));
				yield return null;
			}

			lerpT = 0;
			currentIndex = (currentIndex + 1) % colors.Length;
			
			yield return null;
		}
	}

	public void ShakeCamera()
	{
		StartCoroutine("Shake");
	}

	private IEnumerator Shake()
	{
		Vector3 initialPos = transform.position;
		float initialShakeDuration = shakeDuration;
		float initialShakeAmount = shakeAmount;

		while (shakeDuration > 0)
		{
			if( arrow.outOfBorder == true)
			{
				initialPos.y = transform.position.y;
			}
			transform.localPosition = initialPos + UnityEngine.Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime ;
			shakeAmount *= 0.95f;
			yield return null;
		}

		shakeDuration = initialShakeDuration;
		shakeAmount = initialShakeAmount;
		transform.localPosition = initialPos;
	}
}