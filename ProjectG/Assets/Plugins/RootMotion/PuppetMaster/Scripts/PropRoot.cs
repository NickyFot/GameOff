using UnityEngine;
using System.Collections;

namespace RootMotion.Dynamics {

	/// <summary>
	/// A point in the character's bone hierarchy for connecting props to.
	/// </summary>
	[HelpURL("http://root-motion.com/puppetmasterdox/html/page6.html")]
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Prop Root")]
	public class PropRoot : MonoBehaviour {

		// Open the User Manual URL
		[ContextMenu("User Manual")]
		void OpenUserManual() {
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/page6.html");
		}
		
		// Open the Script Reference URL
		[ContextMenu("Scrpt Reference")]
		void OpenScriptReference() {
			Application.OpenURL("http://root-motion.com/puppetmasterdox/html/class_root_motion_1_1_dynamics_1_1_prop_root.html");
		}

		[Tooltip("Reference to the PuppetMaster component.")]
		/// <summary>
		/// Reference to the PuppetMaster component.
		/// </summary>
		public PuppetMaster puppetMaster;

		[Tooltip("If a prop is connected, what will it's joint be connected to?")]
		/// <summary>
		/// If a prop is connected, what will it's joint be connected to?
		/// </summary>
		public Rigidbody connectTo;

		[Tooltip("Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.")]
		/// <summary>
		/// Is there a Prop connected to this PropRoot? Simply assign this value to connect, replace or drop props.
		/// </summary>
		public Prop currentProp;

		/// <summary>
		/// Dropping/Picking up normally works in the fixed update cycle where joints can be properly connected. Use this to drop a prop immediatelly.
		/// </summary>
		public void DropImmediate() {
			if (lastProp == null) return;
			puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);
			lastProp.Drop();
			
			currentProp = null;
			lastProp = null;
		}

		private Prop lastProp;
		private bool fixedUpdateCalled;

		void Awake() {
			// If currentProp has been assigned, it will be picked up AS IS, presuming it is already linked with the joints and held in the right position.
			// To pick up the prop from ground, assign it after Awake, for example in Start.
			if (currentProp != null) currentProp.StartPickedUp(this);
		}

		void Update() {
			if (!fixedUpdateCalled) return;

			// If dropped by another script or PuppetMaster behaviour
			if (currentProp != null && lastProp == currentProp && currentProp.muscle.connectedBody == null) {
				currentProp.Drop();
				currentProp = null;
				lastProp = null;
			}
		}
		
		void FixedUpdate() {
			fixedUpdateCalled = true;

			if (currentProp == lastProp) return;

			// Dropping current prop
			if (currentProp == null) {
				puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);	
				lastProp.Drop();
			}

			// Picking up to an empty slot
			if (lastProp == null) {
				AttachProp(currentProp);
			}

			// Switching props
			if (lastProp != null && currentProp != null) {
				puppetMaster.RemoveMuscleRecursive(lastProp.muscle, true, false, MuscleRemoveMode.Sever);
				AttachProp(currentProp);
			}

			lastProp = currentProp;
		}

		private void AttachProp(Prop prop) {
			prop.transform.position = transform.position;
			prop.transform.rotation = transform.rotation;

			prop.PickUp(this);
			puppetMaster.AddMuscle(prop.muscle, prop.transform, connectTo, transform, prop.muscleProps, false, prop.forceLayers);

			if (prop.additionalPin != null && prop.additionalPinTarget != null) {
				puppetMaster.AddMuscle(prop.additionalPin, prop.additionalPinTarget, prop.muscle.GetComponent<Rigidbody>(), prop.transform, new Muscle.Props(prop.additionalPinWeight, 0f, 0f, 0f, false, Muscle.Group.Prop), true, prop.forceLayers);
			}
		}

	}
}
