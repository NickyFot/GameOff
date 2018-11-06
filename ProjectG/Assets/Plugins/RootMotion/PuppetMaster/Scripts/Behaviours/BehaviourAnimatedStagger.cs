using UnityEngine;
using System.Collections;

namespace RootMotion.Dynamics {

	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourAnimatedStagger")]
	public class BehaviourAnimatedStagger : BehaviourBase {

		protected override void OnInitiate() {
			centerOfMass.Initiate(this as BehaviourBase, groundLayers);
		}

		protected override void OnActivate() {
			StartCoroutine(LoseBalance());
		}

		[System.Serializable]
		public struct FallParams {
			public float startPinWeightMlp;
			public float startMuscleWeightMlp;
			public float losePinSpeed;
		}
		
		[System.Serializable]
		public struct FallParamsGroup {
			public Muscle.Group[] groups;
			public FallParams fallParams;
		}

		[Header("Master Properties")]
		public LayerMask groundLayers;
		public float animationBlendSpeed = 2f;
		public float animationMag = 5f;
		public float momentumMag = 0.1f;
		public float unbalancedMuscleWeightMlp = 0.05f;
		public float unbalancedMuscleDamperAdd = 1f;
		public bool dropProps;
		public float maxGetUpVelocity = 0.3f;
		public float minHipHeight = 0.3f;

		public SubBehaviourCOM centerOfMass;

		[Header("Muscle Group Properties")]
		public FallParams defaults;
		public FallParamsGroup[] groupOverrides;

		[Header("Events")]
		public PuppetEvent onUngrounded;
		public PuppetEvent onFallOver;
		public PuppetEvent onRest;

		[HideInInspector] public Vector3 moveVector;
		//[HideInInspector] public float speed;
		[HideInInspector] public bool isGrounded = true;
		[HideInInspector] public Vector3 forward;

		public override void OnReactivate() {
			// Called when the PuppetMaster has been deactivated (by parenting it to an inactive hierarchy or calling SetActive(false)).
		}

		private IEnumerator LoseBalance() {
			foreach (Muscle m in puppetMaster.muscles) {
				var fallParams = GetFallParams(m.props.group);

				m.state.pinWeightMlp = Mathf.Min(m.state.pinWeightMlp, fallParams.startPinWeightMlp);
				m.state.muscleWeightMlp = Mathf.Min(m.state.muscleWeightMlp, fallParams.startMuscleWeightMlp);
				m.state.muscleDamperAdd = -puppetMaster.muscleDamper;
			}
			
			puppetMaster.internalCollisions = true;
			
			bool done = false;

			while(!done) {
				Vector3 vel = Quaternion.Inverse(puppetMaster.targetRoot.rotation) * centerOfMass.direction * animationMag;

				moveVector = Vector3.Lerp(moveVector, vel, Time.deltaTime * animationBlendSpeed);
				moveVector = Vector3.ClampMagnitude(moveVector, 2f);

				foreach (Muscle m in puppetMaster.muscles) {
					var fallParams = GetFallParams(m.props.group);
					
					m.state.pinWeightMlp = Mathf.MoveTowards(m.state.pinWeightMlp, 0f, Time.deltaTime * fallParams.losePinSpeed);
					m.state.mappingWeightMlp = Mathf.MoveTowards(m.state.mappingWeightMlp, 1f, Time.deltaTime * animationBlendSpeed);
				}
				
				done = true;

				foreach (Muscle m in puppetMaster.muscles) {
					if (m.state.pinWeightMlp > 0f || m.state.mappingWeightMlp < 1f) {
						done = false;
						break;
					}
				}

				if (puppetMaster.muscles[0].rigidbody.position.y - puppetMaster.targetRoot.position.y < minHipHeight) done = true;

				yield return null;
			}

			if (dropProps) {
				RemoveMusclesOfGroup(Muscle.Group.Prop);
			}
			
			if (!isGrounded) {
				foreach (Muscle m in puppetMaster.muscles) {
					m.state.pinWeightMlp = 0f;
					m.state.muscleWeightMlp = 1f;
				}

				onUngrounded.Trigger(puppetMaster);
				if (onUngrounded.switchBehaviour) yield break;
			}

			moveVector = Vector3.zero;
			//speed = 0f;
			puppetMaster.mappingWeight = 1f;
			
			foreach (Muscle m in puppetMaster.muscles) {
				m.state.pinWeightMlp = 0f;
				m.state.muscleWeightMlp = unbalancedMuscleWeightMlp;
				m.state.muscleDamperAdd = unbalancedMuscleDamperAdd;
			}

			onFallOver.Trigger(puppetMaster);
			if (onFallOver.switchBehaviour) yield break;

			// @todo catch fall behaviour

			yield return new WaitForSeconds(1);

			while(puppetMaster.muscles[0].rigidbody.velocity.magnitude > maxGetUpVelocity || !isGrounded) {
				yield return null;
			}

			onRest.Trigger(puppetMaster);
			if (onRest.switchBehaviour) yield break;
		}

		private FallParams GetFallParams(Muscle.Group group) {
			foreach (FallParamsGroup g in groupOverrides) {
				foreach (Muscle.Group muscleGroup in g.groups) {
					if (muscleGroup == group) return g.fallParams;
				}
			}
			return defaults;
		}
	}
}
