﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Broccoli.Pipe {
	/// <summary>
	/// Element to position trees created by the factory.
	/// </summary>
	[System.Serializable]
	public class BakerElement : PipelineElement {
		#region Vars
		/// <summary>
		/// Gets the type of the connection.
		/// </summary>
		/// <value>The type of the connection.</value>
		public override ConnectionType connectionType {
			get { return PipelineElement.ConnectionType.Sink; }
		}
		/// <summary>
		/// Gets the type of the element.
		/// </summary>
		/// <value>The type of the element.</value>
		public override ElementType elementType {
			get { return PipelineElement.ElementType.Baker; }
		}
		/// <summary>
		/// Gets unique class type of the element.
		/// </summary>
		/// <value>The type of the class.</value>
		public override ClassType classType {
			get { return PipelineElement.ClassType.Baker; }
		}
		/// <summary>
		/// Value used to position elements in the pipeline. The greater the more towards the end of the pipeline.
		/// </summary>
		/// <value>The position weight.</value>
		public override int positionWeight {
			get { return PipelineElement.effectWeight + 20; }
		}
		/// <summary>
		/// Enables ambient occlusion (AO) on the final prefab product.
		/// </summary>
		public bool enableAO = false;
		/// <summary>
		/// Enables AO on the preview tree.
		/// </summary>
		public bool enableAOInPreview = true;
		/// <summary>
		/// Enables AO when processing trees at runtime.
		/// </summary>
		public bool enableAOAtRuntime = false;
		/// <summary>
		/// Samples to use on AO.
		/// </summary>
		public int samplesAO = 4;
		/// <summary>
		/// Amount of AO to bake.
		/// </summary>
		public float strengthAO = 0.5f;
		/// <summary>
		/// If true then a list of positions is used for the trees.
		/// </summary>
		public bool useCustomPositions = false;
		/// <summary>
		/// List of positions.
		/// </summary>
		public List<Position> positions = new List<Position> ();
		/// <summary>
		/// The index on of the selected position.
		/// </summary>
		//[System.NonSerialized]
		public int selectedPositionIndex = -1;
		/// <summary>
		/// The default position.
		/// </summary>
		static Position defaultPosition = new Position ();
		/// <summary>
		/// Temp variable to save enabled positions when requesting one.
		/// </summary>
		List<Position> enabledPositions = new List<Position> ();
		/// <summary>
		/// Modes to animate transition between LOD states.
		/// </summary>
		public enum LODFade {
			None = 0,
			Crossfade = 1,
			SpeedTree = 2
		}
		/// <summary>
		/// Current LOD animation fade mode.
		/// </summary>
		public LODFade lodFade = LODFade.Crossfade;
		/// <summary>
		/// Flag for LOD fade animation.
		/// </summary>
		public bool lodFadeAnimate = false;
		/// <summary>
		/// LOD transition width for crossfade mode.
		/// </summary>
		public float lodTransitionWidth = 0.4f;
		/// <summary>
		/// Flag to unwrap the mesh to the UV channel 1 at runtie.
		/// </summary>
		public bool unwrapUV1sAtRuntime = false;
		/// <summary>
		/// Flag to unwrap the mesh to the UV channel 1 when exporting the prefab.
		/// </summary>
		public bool unwrapUV1sAtPrefab = false;
		/// <summary>
		/// Creates a GameObject per submesh in the final LOD.
		/// </summary>
		public bool splitSubmeshes = false;
		/// <summary>
		/// Option to add a collision object at trunk level.
		/// </summary>
		public bool addCollider = false;
		/// <summary>
		/// Collider types.
		/// </summary>
		public enum ColliderType {
			Capsule = 0,
			Convex = 1,
			NonConvex = 2
		}
		/// <summary>
		/// Type of collider to add to the tree.
		/// </summary>
		public ColliderType colliderType = ColliderType.Capsule;
		/// <summary>
		/// Increases the size of the collider for capsule collider type.
		/// </summary>
		public float colliderScale = 1f;
		/// <summary>
		/// Resolution scale from the original tree mesh.
		/// </summary>
		public float colliderMeshResolution = 0.5f;
		/// <summary>
		/// Minimum branch/root structure for the collider.
		/// </summary>
		public int colliderMinLevel = 0;
		/// <summary>
		/// Maximum branch/root structure for the collider.
		/// </summary>
		public int colliderMaxLevel = 1;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Broccoli.Pipe.BakerElement"/> class.
		/// </summary>
		public BakerElement () {
			this.elementName = "Baker";
			this.elementHelpURL = "https://docs.google.com/document/d/1Nr6Z808i7X2zMFq8PELezPuSJNP5IvRx9C5lJxZ_Z-A/edit#heading=h.r0h3ix5ubi8g";
			this.elementDescription = "This node display options to apply to the final tree GameObject, including LOD, Collider and Ambien Oclussion parameters.";
		}
		#endregion

		#region Validation
		/// <summary>
		/// Validate this instance.
		/// </summary>
		public override bool Validate () {
			log.Clear ();
			if (useCustomPositions) {
				if (useCustomPositions && positions.Count == 0) {
					log.Enqueue (LogItem.GetWarnItem ("Custom positions is enabled but the list of positions is empty."));
				} else {
					bool allDisabled = true;
					for (int i = 0; i < positions.Count; i++) {
						if (positions[i].enabled) {
							allDisabled = false;
							break;
						}
					}
					if (allDisabled) {
						log.Enqueue (LogItem.GetWarnItem ("Custom positions is enabled but all positions on the list are disabled."));
					}
				}
			}
			this.RaiseValidateEvent ();
			return true;
		}
		/// <summary>
		/// Determines whether this instance has any valid position.
		/// </summary>
		/// <returns><c>true</c> if this instance has any valid position; otherwise, <c>false</c>.</returns>
		public bool HasValidPosition () {
			for (int i = 0; i < positions.Count; i++) {
				if (positions[i].enabled)
					return true;
			}
			return false;
		}
		#endregion

		#region Position
		/// <summary>
		/// Gets a position either from a list of custom positions or a default one.
		/// </summary>
		/// <returns>The position.</returns>
		public Position GetPosition () {
			Position position;
			if (useCustomPositions) {
				enabledPositions.Clear ();
				for (int i = 0; i < positions.Count; i++) {
					if (positions [i].enabled)
						enabledPositions.Add (positions [i]);
				}
			}
			if (enabledPositions.Count > 0) {
				position = enabledPositions [Random.Range(0, enabledPositions.Count)];
				enabledPositions.Clear ();
			} else {
				position = defaultPosition;
			}
			return position;
		}
		#endregion

		#region Cloning
		/// <summary>
		/// Clone this instance.
		/// </summary>
		/// <param name="isDuplicate">If <c>true</c> then the clone has elements with new ids.</param>
		/// <returns>Clone of this instance.</returns>
		override public PipelineElement Clone (bool isDuplicate = false) {
			BakerElement clone = ScriptableObject.CreateInstance<BakerElement> ();
			SetCloneProperties (clone, isDuplicate);
			clone.enableAO = enableAO;
			clone.enableAOInPreview = enableAOInPreview;
			clone.enableAOAtRuntime = enableAOAtRuntime;
			clone.samplesAO = samplesAO;
			clone.strengthAO = strengthAO;
			clone.lodFade = lodFade;
			clone.lodFadeAnimate = lodFadeAnimate;
			clone.lodTransitionWidth = lodTransitionWidth;
			clone.unwrapUV1sAtRuntime = unwrapUV1sAtRuntime;
			clone.unwrapUV1sAtPrefab = unwrapUV1sAtPrefab;
			clone.splitSubmeshes = splitSubmeshes;
			clone.addCollider = addCollider;
			clone.colliderType = colliderType;
			clone.colliderScale = colliderScale;
			clone.colliderMeshResolution = colliderMeshResolution;
			clone.colliderMinLevel = colliderMinLevel;
			clone.colliderMaxLevel = colliderMaxLevel;
			return clone;
		}
		#endregion
	}
}