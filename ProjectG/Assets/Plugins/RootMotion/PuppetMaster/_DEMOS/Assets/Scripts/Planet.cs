using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	public class Planet : MonoBehaviour {

		public float mass = 1000;
		public Rigidbody[] rigidbodies;

		// The gravitational constant (also known as "universal gravitational constant", or as "Newton's constant"), denoted by the letter G, is an empirical physical constant involved in the calculation of gravitational effects in Sir Isaac Newton's law of universal gravitation and in Albert Einstein's general theory of relativity.
		private const float G = 6.672e-11f;

		void Start() {
			rigidbodies = (Rigidbody[])GameObject.FindObjectsOfType<Rigidbody>();

			foreach (Rigidbody r in rigidbodies) {
				r.useGravity = false;
			}
		}

		void FixedUpdate() {
			// Add gravity to all the Rigidbodies in the scene
			foreach (Rigidbody r in rigidbodies) {
				if (!r.isKinematic) {
					ApplyGravity(r);
				}
			}
		}

		private void ApplyGravity(Rigidbody r) {
			Vector3 direction = transform.position - r.position;
			float sqrMag = direction.sqrMagnitude;
			float distance = Mathf.Sqrt(sqrMag);
			
			r.velocity += (direction / distance) * G * (mass / sqrMag) * Time.fixedDeltaTime;
		}
	}
}
