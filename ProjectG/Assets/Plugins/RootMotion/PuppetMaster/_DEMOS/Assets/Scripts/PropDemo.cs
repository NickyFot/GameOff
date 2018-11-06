using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	// Code example for picking up/dropping props.
	public class PropDemo : MonoBehaviour {

		[Tooltip("The Prop you wish to pick up.")] 
		public Prop prop;

		[Tooltip("The PropRoot of the left hand.")] 
		public PropRoot propRootLeft;

		[Tooltip("The PropRoot of the right hand.")] 
		public PropRoot propRootRight;

		[Tooltip("If true, the prop will be picked up when PuppetMaster initiates")]
		public bool pickUpOnStart;

		private bool right = true;

		void Start() {
			if (pickUpOnStart) connectTo.currentProp = prop;
		}

		void Update () {
			// Picking up
			if (Input.GetKeyDown(KeyCode.P)) {
				// Makes the prop root drop any existing props and pick up the newly assigned one.
				connectTo.currentProp = prop;
			}

			// Dropping
			if (Input.GetKeyDown(KeyCode.X)) {
				// By setting the prop root's currentProp to null, the prop connected to it will be dropped.
				connectTo.currentProp = null;
			}

			// Switching prop roots.
			if (Input.GetKeyDown(KeyCode.S)) {
				// Dropping/Picking up normally works in the fixed update cycle where joints can be properly connected. Swapping within a single frame can be done by calling PropRoot.DropImmediate();
				connectTo.DropImmediate();

				// Switch hands
				right = !right;

				// Assign the prop to the other hand
				connectTo.currentProp = prop;
			}
		}

		private PropRoot connectTo {
			get {
				return right? propRootRight: propRootLeft;
			}
		}
	}
}
