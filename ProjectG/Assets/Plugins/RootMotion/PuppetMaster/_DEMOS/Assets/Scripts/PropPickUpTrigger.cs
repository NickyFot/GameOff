using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	public class PropPickUpTrigger : MonoBehaviour {

		public Prop prop;
		public LayerMask characterLayers;

		private CharacterPuppet characterPuppet;

		void OnTriggerEnter(Collider collider) {
			if (prop.isPickedUp) return;
			if (!LayerMaskExtensions.Contains(characterLayers, collider.gameObject.layer)) return;

			characterPuppet = collider.GetComponent<CharacterPuppet>();
			if (characterPuppet == null) return;

			if (characterPuppet.puppet.state != BehaviourPuppet.State.Puppet) return;

			if (characterPuppet.propRoot == null) return;
			if (characterPuppet.propRoot.currentProp != null) return;

			characterPuppet.propRoot.currentProp = prop;
		}
	}
}
