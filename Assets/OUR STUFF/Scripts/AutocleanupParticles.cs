using UnityEngine;
using System.Collections;

/// <summary>
/// Autocleanup particles.  This makes a particle system automagically die when it's done emitting.
/// </summary>
public class AutocleanupParticles : MonoBehaviour {

	void Update () {
		if(!GetComponent<ParticleSystem>().IsAlive()) {
			Destroy(gameObject);
		}
	}
}
