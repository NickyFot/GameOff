using UnityEngine;
using UnityEditor;
using System.Collections;

namespace RootMotion.Dynamics {

	[CustomEditor(typeof(PuppetMaster))]
	public class PuppetMasterInspector : Editor {

		private PuppetMaster script { get { return target as PuppetMaster; }}
		private Transform animatedCharacter;
		private bool isValid;
		private int characterControllerLayer = 8;
		private int ragdollLayer = 9;

		// Colors
		private GUIStyle style = new GUIStyle();
		private GUIStyle miniLabelStyle = new GUIStyle();
		private static Color pro = new Color(0.5f, 0.7f, 0.3f, 1f);
		private static Color free = new Color(0.2f, 0.3f, 0.1f, 1f);

		private MonoScript monoScript;
		private Animator targetAnimator;
		private SkinnedMeshRenderer[] skinnedMeshRenderers = new SkinnedMeshRenderer[0];

		#region Inspector

		void OnEnable() {
			if (!Application.isPlaying) {
				monoScript = MonoScript.FromMonoBehaviour(script);
				int currentExecutionOrder = MonoImporter.GetExecutionOrder(monoScript);
				if (currentExecutionOrder != 10100) MonoImporter.SetExecutionOrder(monoScript, 10100);

				// Fix negative collider sizes for ragdolls created before PuppetMaster 0.3
				BoxCollider[] boxColliders = script.GetComponentsInChildren<BoxCollider>();
				foreach (BoxCollider box in boxColliders) {
					box.size = new Vector3(Mathf.Abs(box.size.x), Mathf.Abs(box.size.y), Mathf.Abs(box.size.z));
				}

				CapsuleCollider[] capsuleColliders = script.GetComponentsInChildren<CapsuleCollider>();
				foreach (CapsuleCollider capsule in capsuleColliders) {
					capsule.height = Mathf.Abs(capsule.height);
					capsule.radius = Mathf.Abs(capsule.radius);
				}

				if (script.transform.parent != null) {
					targetAnimator = script.transform.parent.GetComponentInChildren<Animator>();
				}
			} else {
				if (script.initiated) targetAnimator = script.targetAnimator;
			}

			if (script.transform.parent != null) {
				skinnedMeshRenderers = script.transform.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
			}

			isValid = IsValid();
		}

		private bool IsValid() {
			if (script.muscles.Length > 0) return true;
			
			ConfigurableJoint[] joints = script.gameObject.GetComponentsInChildren<ConfigurableJoint>();
			
			if (joints == null || joints.Length == 0) {
				return false;
			}
			
			if (animatedCharacter == null) animatedCharacter = script.transform;
			return true;
		}

		public override void OnInspectorGUI() {
			if (script == null) return;
			serializedObject.Update();

			if (script.muscles.Length > 0 && script.muscles[0].target != null && script.targetRoot == null) script.targetRoot = script.FindTargetRootRecursive(script.muscles[0].target);

			style.wordWrap = true;
			style.normal.textColor = EditorGUIUtility.isProSkin? pro: free;

			miniLabelStyle.wordWrap = true;
			miniLabelStyle.fontSize = 9;
			miniLabelStyle.normal.textColor = EditorStyles.miniLabel.normal.textColor;

			if (!isValid) {
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(
					"To set up PuppetMaster, add the component to a ragdoll character that is built with ConfigurableJoints. " +
					"If your ragdoll uses other types of joints, you can easily convert them by selecting the character and clicking on GameObject/Convert To ConfigurableJoints."
					, style);
				EditorGUILayout.Space();
				return;
			}

			foreach (Muscle m in script.muscles) {
				if (m.joint == null) {
					m.name = "Missing Joint Reference!";
				} else if (m.target == null) {
					m.name = "Missing Target Reference!";
				} else m.name = m.joint.name;
			}

			if (script.muscles.Length == 0) {
				GUILayout.Space(5);

				animatedCharacter = (Transform)EditorGUILayout.ObjectField(new GUIContent("Animated Target", "If it is assigned to the same GameObject, it will be duplicated and cleaned up of all the ragdoll components. It can also be an instance of the character or even another character as long as the positions of the bones match. The rotations of the bones don't need to be identical. That makes it possible to share ragdoll structures and is also most useful when the feet bones are not aligned to the ground (like Mixamo characters). In that case you can simply rotate the ragdoll's feet to align, keeping the target's feet where they are."), animatedCharacter, typeof(Transform), true);

				GUILayout.Space(5);
				characterControllerLayer = EditorGUILayout.IntField(new GUIContent("Character Controller Layer", "The layer to assign the character controller to. Collisions between this layer and the 'Ragdoll Layer' will be ignored, or else the ragdoll would collide with the character controller."), characterControllerLayer);
				ragdollLayer = EditorGUILayout.IntField(new GUIContent("Ragdoll Layer", "The layer to assign the PuppetMaster and all it's muscles to. Collisions between this layer and the 'Character Controller Layer' will be ignored, or else the ragdoll would collide with the character controller."), ragdollLayer);

				if (characterControllerLayer == ragdollLayer) {
					GUILayout.Space(5);
					EditorGUILayout.LabelField(
						"The 'Character Controller Layer' must not be the same as the 'Ragdoll Layer', your ragdoll bones would collide with the character controller."
						, style);
					EditorGUILayout.Space();
				}

				if (animatedCharacter != null && characterControllerLayer != ragdollLayer) {
					GUILayout.Space(5);

					if (GUILayout.Button("Set Up PuppetMaster", GUILayout.MaxWidth(140))) {
						script.SetUpTo(animatedCharacter, characterControllerLayer, ragdollLayer);

						animatedCharacter = null;
					}

					EditorGUILayout.LabelField("Setting up PuppetMaster is not undoable. Make sure to make a backup duplicate of the character before you do that.", miniLabelStyle);
				}

				GUILayout.Space(5);
			} else {
				if (!Application.isPlaying && script.humanoidConfig != null && script.targetRoot != null && script.targetAnimator != null && script.targetAnimator.isHuman) {
					script.humanoidConfig.ApplyTo(script);

					GUILayout.Space(5);
					EditorGUILayout.BeginVertical("Box");
					GUILayout.Label("Properties of this PuppetMaster are locked to the Humanoid Config file.", miniLabelStyle);
					EditorGUILayout.EndVertical();
					GUILayout.Space(5);
				}

				script.muscleSpring = Mathf.Clamp(script.muscleSpring, 0f, script.muscleSpring);
				script.muscleDamper = Mathf.Clamp(script.muscleDamper, 0f, script.muscleDamper);
				
				DrawDefaultInspector();
			}

			bool animatorWarning = targetAnimator != null && targetAnimator.cullingMode != AnimatorCullingMode.AlwaysAnimate;
			if (animatorWarning) {
				GUILayout.Space(5);
				EditorGUILayout.BeginVertical("Box");
				GUILayout.Label("If Animator 'Culling Mode' is not set to 'Always Animate', animation might pause and cause glitches when the ragdoll is out of SkinnedMeshRenderer.bounds.", miniLabelStyle);
				EditorGUILayout.EndVertical();
			}

			bool rendererWarning = false;
			foreach (SkinnedMeshRenderer s in skinnedMeshRenderers) {
				if (!s.updateWhenOffscreen) {
					rendererWarning = true;
					break;
				}
			}
			if (rendererWarning) {
				if (!animatorWarning) GUILayout.Space(5);
				EditorGUILayout.BeginVertical("Box");
				GUILayout.Label("One of the SkinnedMeshRenderers in the animated character hierarchy has 'Update When Offscreen' set to false. This might cause it to disappear from view when the ragdoll is out of the renderer's bounds.", miniLabelStyle);
				EditorGUILayout.EndVertical();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool IsRagdoll() {
			Rigidbody[] rigidbodies = (Rigidbody[])script.gameObject.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody r in rigidbodies) {
				if (r != script.GetComponent<Rigidbody>()) return true;
			}
			return false;
		}

		#endregion Inspector

		#region Scene

		/*
		void OnSceneGUI() {
			if (script == null) return;

			foreach (Muscle m in script.muscles) {
				Handles.color = Color.green;

				//if (m.joint.gameObject.name == "Support Point") Debug.Log(m.joint.rigidbody.inertiaTensor.x + ", " + m.joint.rigidbody.inertiaTensor.y + ", " + m.joint.rigidbody.inertiaTensor.z);

				Handles.SphereCap(0, m.joint.transform.TransformPoint(m.joint.rigidbody.inertiaTensor), Quaternion.identity, 0.05f);
				//Handles.SphereCap(0, m.joint.rigidbody.worldCenterOfMass, Quaternion.identity, 0.05f);
			}

			Handles.color = Color.white;
		}
		*/

		#endregion Scene

	}
}
