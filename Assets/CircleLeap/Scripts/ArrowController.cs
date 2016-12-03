using UnityEngine;
using System;

public class ArrowController : Singleton<ArrowController> 
{
	public enum State
	{
		LEVEL_START,
		ON_CIRCLE,
		JUMPING,
		DEAD
	}

	private State arrowState; 

	// period of arrow rotation when it's on circle
	public float rotationDuration = 1.5f;

	// velocity of arrow when it's not on circle
	public float linearVelocity = 10f;

	// arrow crash callback
	public Action arrowCrashed;

	// time passed since arrow hit the circle
	private float timeSinceStart;

	// start rotation around circle center
	private float startRotation = 0;

	// current rotation around circle center
	private float? currentRotationX;

	private bool movingClockwise = false;

	private const float circleBorderWidth = 0.04f;

	// is arrow currently outside of world borders
	[HideInInspector]
	public bool? outOfBorder = null;

	[HideInInspector]
	public TriggerDetector triggerDetector;

	[HideInInspector]
	public Rigidbody2D rb;

	[HideInInspector]
	public CircleController circle;

	private PolygonCollider2D col2D;

	public State ArrowState
	{
		get 
		{
			return arrowState;
		}
		set
		{
			if (arrowState == value)
			{
				return;
			}

			arrowState = value;

			switch (value) 
			{
			case State.JUMPING:
				circle = null;
				break;

			case State.DEAD:
				arrowCrashed();
				break;
			}
		}
	}

	void Awake()
	{
		col2D = GetComponent<PolygonCollider2D>();
		triggerDetector = GetComponent<TriggerDetector>();

		GameManager.Instance.gameStateChangeEvent += (GameManager.State gameState) => {
			
			switch(gameState)
			{
			case GameManager.State.Playing:
				ResetArrow();
				break;
			}
		};

		arrowCrashed += () => {

			GameManager.Instance.GameState = GameManager.State.Failed;

			CameraController.Instance.ShakeCamera();
			SoundManager.Instance.PlayCrush();

			circle = null;
			col2D.enabled = false;
			rb.velocity = Vector2.zero;
			GetComponent<Renderer>().enabled = false;
			transform.FindChild("ArrowShadow").GetComponent<Renderer>().enabled = false;
		};
	}
		
	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();

		currentRotationX = 0;

		triggerDetector.TriggerEnterEvent += (Collider2D coll) => {

			if(circle != null)
				return;

			if(coll.gameObject.CompareTag("Obstacles"))
			{
				ArrowState = State.DEAD;
			} 
			else if(coll.gameObject.CompareTag("WorldTopTrigger") || coll.gameObject.CompareTag("WorldBottomTrigger"))
			{
				if(!outOfBorder.HasValue)
					outOfBorder = false;
				else
					outOfBorder = true;
			}
			else if(coll.CompareTag("Circle"))
			{
				HitCircle(coll.GetComponent<CircleController>());
			}
		};
	}


	// Update is called once per frame
	void Update () 
	{
		if(ArrowState != State.DEAD)
		{
			if(InputManager.Instance.TouchDown && currentRotationX != null)
			{
				if(movingClockwise)
				{
					rb.velocity = new Vector2(
						linearVelocity * Mathf.Sin(currentRotationX.Value), 
						-linearVelocity * Mathf.Cos(currentRotationX.Value)
					);
				}
				else
				{
					rb.velocity = new Vector2(
						-linearVelocity * Mathf.Sin(currentRotationX.Value), 
						linearVelocity * Mathf.Cos(currentRotationX.Value)
					);
				}
				ArrowState = State.JUMPING;
			}
			else 
			{
				if(circle == null)
				{
					return;
				}

				timeSinceStart += Time.smoothDeltaTime;

				currentRotationX = startRotation + (2 * Mathf.PI) * timeSinceStart / rotationDuration * (movingClockwise ? -1 : 1);

				Transform center = circle.transform;

				transform.position = new Vector3(
					center.position.x + Mathf.Cos(currentRotationX.Value) * (circle.Radius - circleBorderWidth),
					center.position.y + Mathf.Sin(currentRotationX.Value) * (circle.Radius - circleBorderWidth),
					0
				);
					
				float rotationZ;

				if(movingClockwise) 
				{
					rotationZ = Mathf.Rad2Deg * (currentRotationX.Value + Mathf.PI);
				}
				else
				{
					rotationZ = Mathf.Rad2Deg * (currentRotationX.Value);
				}

				transform.rotation = Quaternion.Euler(0, 0, rotationZ);
			}
		}
	}

	private void HitCircle(CircleController circle)
	{
		this.circle = circle;
		ArrowState = State.ON_CIRCLE;

		outOfBorder = false;
		timeSinceStart = 0;

		Vector2 touchPosition = transform.position;
		Vector2 circleCenter = circle.gameObject.transform.position;
		Vector2 touchToCenter = circleCenter - touchPosition;
		Vector2 prevRbVelocity = rb.velocity;

		rb.velocity = Vector2.zero;

		float angleBetVectors = Vector2.Angle(circleCenter - touchPosition, Vector2.right);
		startRotation =  Mathf.PI + angleBetVectors * Mathf.Deg2Rad;

		currentRotationX = null;

		Vector2 circleTangent = new Vector2(touchToCenter.y, -touchToCenter.x) / touchToCenter.magnitude;

		float perpArrowAngle = Vector2.Angle(circleTangent, prevRbVelocity);

		if(perpArrowAngle >= 90)
		{
			movingClockwise = true;
		}
		else
		{
			movingClockwise = false;
		}

		// play circle fade out animation
		circle.FadeOut();

		SoundManager.Instance.PlayHit();
		ScoreManager.Instance.DisplayScore(Mathf.FloorToInt(transform.position.y + Camera.main.orthographicSize));
	}

	private void ResetArrow()
	{
		ArrowState = State.JUMPING;

		GetComponent<Renderer>().enabled = true;

		transform.FindChild("ArrowShadow").GetComponent<Renderer>().enabled = true;

		transform.position = new Vector3(0, -7, 0);
		transform.rotation = Quaternion.identity;

		movingClockwise = false;
		col2D.enabled = true;
		currentRotationX = 0;
		outOfBorder = null;

		Camera.main.transform.position = new Vector3(0, 0, -10);
	}
}