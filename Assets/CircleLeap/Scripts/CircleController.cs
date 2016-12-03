using UnityEngine;
using System.Collections;

public class CircleController : MonoBehaviour 
{
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	public float Radius 
	{
		get
		{
			return spriteRenderer.bounds.extents.x;
		}
		set
		{
			float scale = value / Radius;
			transform.localScale = new Vector3(scale, scale, 1);
		}
	}

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	public void FadeOut()
	{
		if(gameObject.name != "StartCircle")
			animator.SetTrigger("FadeOut");
	}

	public void Recycle()
	{
		gameObject.SetActive(false);

		ArrowController arrow = ArrowController.Instance;

		if(arrow.circle == this)
			arrow.ArrowState = ArrowController.State.DEAD;
	}

	public void Reset()
	{
		spriteRenderer.color = new Color(1, 1, 1, 1);
		gameObject.SetActive(true);
	}
}