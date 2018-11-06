using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

[RequireComponent(typeof(ParticleSystem))]
public class FXCollisionBlood : MonoBehaviour {

		public BehaviourPuppet puppet;
		public float minCollisionImpulse = 100f;
		public int emission = 2;
		public float emissionImpulseAdd = 0.01f;
		public int maxEmission = 7;

		private ParticleSystem particles;

		void Start () {
			particles = GetComponent<ParticleSystem>();

			puppet.OnCollisionImpulse += OnCollisionImpulse;
		}

		void OnCollisionImpulse(MuscleCollision m, float impulse) {
			if (m.collision.contacts.Length == 0) return;
			if (impulse < minCollisionImpulse) return;

			transform.position = m.collision.contacts[0].point;
			transform.rotation = Quaternion.LookRotation(m.collision.contacts[0].normal);

			particles.Emit(Mathf.Min(emission + (int)(emissionImpulseAdd * impulse), maxEmission));
		}

		void OnDestroy() {
			if (puppet != null) puppet.OnCollisionImpulse -= OnCollisionImpulse;
		}
	}
}
