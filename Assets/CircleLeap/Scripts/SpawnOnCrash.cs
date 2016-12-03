using UnityEngine;

public class SpawnOnCrash : MonoBehaviour 
{
	public ArrowController arrow;
	private ParticleSystem _particleSystem;

	void Start() 
	{
		_particleSystem = GetComponent<ParticleSystem>();

		arrow.arrowCrashed += () => {

			_particleSystem.Emit(70);
		};
	}
}
