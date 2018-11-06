using UnityEngine;
using System.Collections;
using System;

namespace RootMotion.Dynamics {

	[System.Serializable]
	public enum MuscleRemoveMode {
		Sever, // Severs the body part disconnecting the first joit
		Explode, // Explodes the body part disconnecting all joints
		Numb, // Removes the muscles, keeps the joints connected, but disables spring and damper forces
	}
	
	// Contains high level API calls for changing the PuppetMaster's muscle structure.
	public partial class PuppetMaster: MonoBehaviour {

		/// <summary>
		/// NB! Make sure to call this from FixedUpdate!
		/// Creates a new muscle for the specified "joint" and targets it to the "target". The joint will be connected to the specified "connectTo" Muscle.
		/// Note that the joint will be binded to it's current position and rotation relative to the "connectTo", so make sure the added object is positioned correctly when calling this.
		/// This method allocates memory, avoid using it each frame.
		/// </summary>
		public void AddMuscle(ConfigurableJoint joint, Transform target, Rigidbody connectTo, Transform targetParent, Muscle.Props muscleProps = null, bool forceTreeHierarchy = false, bool forceLayers = true) {
			if (!CheckIfInitiated()) return;

			if (!initiated) {
				Debug.LogWarning("PuppetMaster has not been initiated.", transform);
				return;
			}
			
			if (ContainsJoint(joint)) {
				Debug.LogWarning("Joint " + joint.name + " is already used by a Muscle", transform);
				return;
			}
			
			if (target == null) {
				Debug.LogWarning("AddMuscle was called with a null 'target' reference.", transform);
				return;
			}
			
			if (connectTo == joint.GetComponent<Rigidbody>()) {
				Debug.LogWarning("ConnectTo is the joint's own Rigidbody, can not add muscle.", transform);
				return;
			}

			if (!isActive) {
				Debug.LogWarning("Adding muscles to inactive PuppetMasters is not currently supported.", transform);
				return;
			}
			
			if (muscleProps == null) muscleProps = new Muscle.Props();
			
			Muscle muscle = new Muscle();
			muscle.props = muscleProps;
			muscle.joint = joint;
			muscle.target = target;
			muscle.joint.transform.parent = (hierarchyIsFlat || connectTo == null) && !forceTreeHierarchy? transform: connectTo.transform;

			if (forceLayers) {
				joint.gameObject.layer = gameObject.layer; //@todo what if collider is on a child gameobject?
				target.gameObject.layer = targetRoot.gameObject.layer;
			}

			if (connectTo != null) {
				muscle.target.parent = targetParent;
				
				Vector3 relativePosition = GetMuscle(connectTo).transform.InverseTransformPoint(muscle.target.position);
				Quaternion relativeRotation = Quaternion.Inverse(GetMuscle(connectTo).transform.rotation) * muscle.target.rotation;
				
				joint.transform.position = connectTo.transform.TransformPoint(relativePosition);
				joint.transform.rotation = connectTo.transform.rotation * relativeRotation;
				
				joint.connectedBody = connectTo;
			}
			
			muscle.Initiate(muscles);
			
			if (connectTo != null) {
				muscle.rigidbody.velocity = connectTo.velocity;
				muscle.rigidbody.angularVelocity = connectTo.angularVelocity;
			}
			
			// Ignore internal collisions
			if (!internalCollisions) {
				for (int i = 0; i < muscles.Length; i++) {
					muscle.IgnoreCollisions(muscles[i], true);
				}
			}
			
			Array.Resize(ref muscles, muscles.Length + 1);
			muscles[muscles.Length - 1] = muscle;
			
			// Update angular limit ignoring
			muscle.IgnoreAngularLimits(!angularLimits);

			if (behaviours.Length > 0) {
				muscle.broadcaster = muscle.joint.gameObject.AddComponent<MuscleCollisionBroadcaster>();
				muscle.broadcaster.puppetMaster = this;
				muscle.broadcaster.muscleIndex = muscles.Length - 1;
			}

			muscle.jointBreakBroadcaster = muscle.joint.gameObject.AddComponent<JointBreakBroadcaster>();
			muscle.jointBreakBroadcaster.puppetMaster = this;
			muscle.jointBreakBroadcaster.muscleIndex = muscles.Length - 1;

			UpdateHierarchies();
			CheckMassVariation(100f, true);

			foreach (BehaviourBase b in behaviours) b.OnMuscleAdded(muscle);
		}

		/// <summary>
		/// Removes the muscle with the specified joint and all muscles connected to it from PuppetMaster management. This will not destroy the body part/prop, but just release it from following the target.
		/// If you call RemoveMuscleRecursive on an upper arm muscle, the entire arm will be disconnected from the rest of the body.
		/// </summary>
		/// <param name="joint">The joint of the muscle (and the muscles connected to it) to remove.</param>
		/// <param name="attachTarget">If set to <c>true</c> , the target Transform of the first muscle will be parented to the disconnected limb.</param>
		/// <param name="blockTargetAnimation">If set to <c>true</c>, will add AnimationBlocker.cs to the removed target bones. That will override animation that would otherwise still be writing on those bones.</param>
		/// <param name="removeMode">Remove mode. Sever cuts the body part by disconnecting the first joint. Explode explodes the body part disconnecting all joints. Numb removes the muscles from PuppetMaster management, keeps the joints connected and disables spring and damper forces.</param>
		public void RemoveMuscleRecursive(ConfigurableJoint joint, bool attachTarget, bool blockTargetAnimation = false, MuscleRemoveMode removeMode = MuscleRemoveMode.Sever) {
			if (!CheckIfInitiated()) return;
			
			if (joint == null) {
				Debug.LogWarning("RemoveMuscleRecursive was called with a null 'joint' reference.", transform);
				return;
			}
			
			if (!ContainsJoint(joint)) {
				Debug.LogWarning("No Muscle with the specified joint was found, can not remove muscle.", transform);
				return;
			}

			int index = GetMuscleIndex(joint);
			Muscle[] newMuscles = new Muscle[muscles.Length - (muscles[index].childIndexes.Length + 1)];
			
			int added = 0;
			for (int i = 0; i < muscles.Length; i++) {
				if (i != index && !muscles[index].childFlags[i]) {
					newMuscles[added] = muscles[i];
					added ++;
				} else {
					if (muscles[i].broadcaster != null) {
						muscles[i].broadcaster.enabled = false;
						Destroy(muscles[i].broadcaster);
					}
					if (muscles[i].jointBreakBroadcaster != null) {
						muscles[i].jointBreakBroadcaster.enabled = false;
						Destroy(muscles[i].jointBreakBroadcaster);
					}
				}
			}

			switch(removeMode) {
			case MuscleRemoveMode.Sever:
				DisconnectJoint(muscles[index].joint);

				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					KillJoint(muscles[muscles[index].childIndexes[i]].joint);
				}
				break;
			case MuscleRemoveMode.Explode:
				DisconnectJoint(muscles[index].joint);

				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					DisconnectJoint(muscles[muscles[index].childIndexes[i]].joint);
				}
				break;
			case MuscleRemoveMode.Numb:
				KillJoint(muscles[index].joint);

				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					KillJoint(muscles[muscles[index].childIndexes[i]].joint);
				}
				break;
			}

			muscles[index].transform.parent = null;
			
			for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
				if (removeMode == MuscleRemoveMode.Explode || muscles[muscles[index].childIndexes[i]].transform.parent == transform) {
					muscles[muscles[index].childIndexes[i]].transform.parent = null;
				}
			}
			

			foreach (BehaviourBase b in behaviours) {
				b.OnMuscleRemoved(muscles[index]);
				
				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					b.OnMuscleRemoved(c);
				}
			}
			
			if (attachTarget) {
				muscles[index].target.parent = muscles[index].transform;
				muscles[index].target.position = muscles[index].transform.position;
				muscles[index].target.rotation = muscles[index].transform.rotation * muscles[index].targetRotationRelative;
				
				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					c.target.parent = c.transform;
					c.target.position = c.transform.position;
					c.target.rotation = c.transform.rotation;
				}
			}
			
			if (blockTargetAnimation) {
				muscles[index].target.gameObject.AddComponent<AnimationBlocker>();
				
				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					c.target.gameObject.AddComponent<AnimationBlocker>();
				}
			}

			if (OnMuscleRemoved != null) OnMuscleRemoved(muscles[index]);
			for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
				var c = muscles[muscles[index].childIndexes[i]];
				if (OnMuscleRemoved != null) OnMuscleRemoved(c);
			}

			// Enable collisions between the new muscles and the removed colliders
			if (!internalCollisionsEnabled) {
				foreach (Muscle newMuscle in newMuscles) {
					foreach (Collider newMuscleCollider in newMuscle.colliders) {
						foreach (Collider removedMuscleCollider in muscles[index].colliders) {
							Physics.IgnoreCollision(newMuscleCollider, removedMuscleCollider, false);
						}

						for (int childMuscleIndex = 0; childMuscleIndex < muscles[index].childIndexes.Length; childMuscleIndex++) {
							foreach (Collider childMuscleCollider in muscles[childMuscleIndex].colliders) {
								Physics.IgnoreCollision(newMuscleCollider, childMuscleCollider, false);
							}
						}
					}
				}
			}
			
			muscles = newMuscles;
			
			UpdateHierarchies();
		}
		
		/// <summary>
		/// Removes the muscle with the specified joint and all muscles connected to it from PuppetMaster management. This will not destroy the body part/prop, but just release it from following the target.
		/// If you call RemoveMuscleRecursive on an upper arm muscle, the entire arm will be disconnected from the rest of the body.
		/// If attachTarget is true, the target Transform of the first muscle will be parented to the disconnected limb.
		/// This method allocates some memory, avoid using it each frame.
		/// </summary>
		/*
		public void RemoveMuscleRecursive(ConfigurableJoint joint, bool attachTarget, bool blockTargetAnimation = false, bool detachChildren = false, bool detachJoint = true) {
			if (!CheckIfInitiated()) return;
			
			if (joint == null) {
				Debug.LogWarning("RemoveMuscleRecursive was called with a null 'joint' reference.", transform);
				return;
			}
			
			if (!ContainsJoint(joint)) {
				Debug.LogWarning("No Muscle with the specified joint was found, can not remove muscle.", transform);
				return;
			}

			int index = GetMuscleIndex(joint);
			Muscle[] newMuscles = new Muscle[muscles.Length - (muscles[index].childIndexes.Length + 1)];

			int added = 0;
			for (int i = 0; i < muscles.Length; i++) {
				if (i != index && !muscles[index].childFlags[i]) {
					newMuscles[added] = muscles[i];
					added ++;
				} else {
					if (muscles[i].broadcaster != null) Destroy(muscles[i].broadcaster);
					if (muscles[i].jointBreakBroadcaster != null) Destroy(muscles[i].jointBreakBroadcaster);
				}
			}

			if (detachJoint) DisconnectJoint(muscles[index].joint);
			else KillJoint(muscles[index].joint);

			if (detachChildren) {
				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					if (detachJoint) DisconnectJoint(muscles[muscles[index].childIndexes[i]].joint);
					else KillJoint(muscles[muscles[index].childIndexes[i]].joint);
				}
			}

			muscles[index].transform.parent = null;

			for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
				if (detachChildren || muscles[muscles[index].childIndexes[i]].transform.parent == transform) muscles[muscles[index].childIndexes[i]].transform.parent = null;
			}

			foreach (BehaviourBase b in behaviours) {
				b.OnMuscleRemoved(muscles[index]);

				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					b.OnMuscleRemoved(c);
				}
			}
			
			if (attachTarget) {
				muscles[index].target.parent = muscles[index].transform;
				muscles[index].target.position = muscles[index].transform.position;
				muscles[index].target.rotation = muscles[index].transform.rotation * muscles[index].targetRotationRelative;

				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					c.target.parent = c.transform;
					c.target.position = c.transform.position;
					c.target.rotation = c.transform.rotation;
				}
			}

			if (blockTargetAnimation) {
				muscles[index].target.gameObject.AddComponent<AnimationBlocker>();
				
				for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
					var c = muscles[muscles[index].childIndexes[i]];
					c.target.gameObject.AddComponent<AnimationBlocker>();
				}
			}

			if (OnMuscleRemoved != null) OnMuscleRemoved(muscles[index]);
			for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
				var c = muscles[muscles[index].childIndexes[i]];
				if (OnMuscleRemoved != null) OnMuscleRemoved(c);
			}
			
			muscles = newMuscles;
			
			UpdateHierarchies();
		}
		*/

		/// <summary>
		/// NB! Make sure to call this from FixedUpdate!
		/// Replaces a muscle with a new one. This can be used to replace props, 
		/// but in most cases it would be faster and more efficient to do it by maintaining the muscle (the Joint and the Rigidbody) and just replacing the colliders and the graphical model.
		/// This method allocates memory, avoid using it each frame.
		/// </summary>
		public void ReplaceMuscle(ConfigurableJoint oldJoint, ConfigurableJoint newJoint) {
			if (!CheckIfInitiated()) return;
			
			// @todo
			Debug.LogWarning("@todo", transform);
		}
		
		/// <summary>
		/// NB! Make sure to call this from FixedUpdate!
		/// Completely replaces the muscle structure. Make sure the new muscle objects are positioned and rotated correctly relative to their targets.
		/// This method allocates memory, avoid using it each frame.
		/// </summary>
		public void SetMuscles(Muscle[] newMuscles) {
			if (!CheckIfInitiated()) return;
			
			// @todo
			Debug.LogWarning("@todo", transform);
		}
		
		/// <summary>
		/// Disables the muscle with the specified joint and all muscles connected to it. This is a faster and more efficient alternative to RemoveMuscleRecursive,
		/// as it will not require reinitiating the muscles.
		/// </summary>
		public void DisableMuscleRecursive(ConfigurableJoint joint) {
			if (!CheckIfInitiated()) return;
			
			// @todo
			Debug.LogWarning("@todo", transform);
		}
		
		/// <summary>
		/// Re-enables a previously disabled muscle and the muscles connected to it.
		/// </summary>
		public void EnableMuscleRecursive(ConfigurableJoint joint) {
			if (!CheckIfInitiated()) return;
			
			// @todo
			Debug.LogWarning("@todo", transform);
		}

		/// <summary>
		/// Flattens the ragdoll hierarchy so that all muscles are parented to the PuppetMaster.
		/// </summary>
		[ContextMenu("Flatten Muscle Hierarchy")]
		public void FlattenHierarchy() {
			foreach (Muscle m in muscles) {
				if (m.joint != null) m.joint.transform.parent = transform;
			}

			hierarchyIsFlat = true;
		}

		/// <summary>
		/// Builds a hierarchy tree from the muscles.
		/// </summary>
		[ContextMenu("Tree Muscle Hierarchy")]
		public void TreeHierarchy() {
			foreach (Muscle m in muscles) {
				if (m.joint != null) {
					m.joint.transform.parent = m.joint.connectedBody != null? m.joint.connectedBody.transform: transform;
				}
			}

			hierarchyIsFlat = false;
		}

		/// <summary>
		/// Moves all muscles to the positions of their targets.
		/// </summary>
		[ContextMenu("Fix Muscle Positions")]
		public void FixMusclePositions() {
			foreach (Muscle m in muscles) {
				if (m.joint != null && m.target != null) {
					m.joint.transform.position = m.target.position;
				}
			}
		}

		private void AddIndexesRecursive(int index, ref int[] indexes) {
			int l = indexes.Length;
			Array.Resize(ref indexes, indexes.Length + 1 + muscles[index].childIndexes.Length);
			indexes[l] = index;
			
			if (muscles[index].childIndexes.Length == 0) return;
			
			for (int i = 0; i < muscles[index].childIndexes.Length; i++) {
				AddIndexesRecursive(muscles[index].childIndexes[i], ref indexes);
			}
		}

		// Are all the muscles parented to the PuppetMaster Transform?
		private bool HierarchyIsFlat() {
			foreach (Muscle m in muscles) {
				if (m.joint.transform.parent != transform) return false;
			}
			return true;
		}

		// Disconnects a joint without destroying it
		private void DisconnectJoint(ConfigurableJoint joint) {
			joint.connectedBody = null;
			
			KillJoint(joint);

			joint.xMotion = ConfigurableJointMotion.Free;
			joint.yMotion = ConfigurableJointMotion.Free;
			joint.zMotion = ConfigurableJointMotion.Free;
			joint.angularXMotion = ConfigurableJointMotion.Free;
			joint.angularYMotion = ConfigurableJointMotion.Free;
			joint.angularZMotion = ConfigurableJointMotion.Free;
		}

		// Disables joint target rotation, position spring and damper
		private void KillJoint(ConfigurableJoint joint) {
			joint.targetRotation = Quaternion.identity;
			JointDrive j = new JointDrive();
			j.positionSpring = 0f;
			j.positionDamper = 0f;
			
			#if UNITY_5_2
			j.mode = JointDriveMode.None;
			#endif
			
			joint.slerpDrive = j;
		}
	}
}
