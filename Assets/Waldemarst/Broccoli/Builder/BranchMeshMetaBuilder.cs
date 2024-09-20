﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

using Broccoli.Model;

namespace Broccoli.Builder
{
	/// <summary>
	/// Provides methods to set uv, uv2 and color32 values on branch meshes.
	/// </summary>
	public class BranchMeshMetaBuilder {
		#region UV Job
		struct UVJob : IJobParallelFor {
			public NativeArray<Vector4> uvs;
			/// <summary>
			/// UV8 information of the mesh.
			/// x: radial position.
			/// y: global length position.
			/// z: girth.
			/// w: unallocated.
			/// </summary>
			public NativeArray<Vector4> uv8s;
			public float mappingXDisplacement;
			/// <summary>
			/// The mapping Y displacement.
			/// </summary>
			public float mappingYDisplacement;
			public int mappingXTiles;
			public int mappingYTiles;
			/// <summary>
			/// The UV mapping is sensitive to branch girth changes..
			/// </summary>
			public bool isGirthSensitive;
			public float minGirth;
			public float maxGirth;
			public bool applyLimits;
			public float mappingLimitXOffset;
			public float mappingLimitYOffset;
			public float mappingLimitWidth;
			public float mappingLimitHeight;
			public void Execute(int i) {
				Vector4 uv = uvs[i];
				Vector4 uv8 = uv8s[i];
				float girthFactor = 1;
				if (isGirthSensitive) {
					girthFactor = (uv8.z - minGirth) / (maxGirth - minGirth) * 0.4f + 0.6f;
				}
				float uvU = uv8.x; 
				float uvV = uv8.y;
				if (applyLimits) {
					uvU = uv8.x * mappingLimitWidth + mappingLimitXOffset;
					uvV = uv8.y * mappingLimitHeight + mappingLimitYOffset;
				}
				/*
				uv.x = (uv8.x * -mappingXTiles) + (mappingXDisplacement * uv8.y / 5f);
				uv.y = (uv8.y * mappingYTiles / 4f) * (1f - (mappingYDisplacement / 5f)) / girthFactor;
				*/
				uv.x = (uvU * mappingXTiles) + (mappingXDisplacement * uvV / 5f);
				uv.y = (uvV * mappingYTiles / 4f) * (1f - (mappingYDisplacement / 5f)) / girthFactor;
				uvs[i] = uv;
			}
		}
		#endregion

		#region Vars
		/// <summary>
		/// Max girth found at the tree.
		/// </summary>
		private float maxGirth = 1f;
		/// <summary>
		/// Relationship between branches given their id and the branch skin they belong to.
		/// </summary>
		/// <typeparam name="int">Branch id.</typeparam>
		/// <typeparam name="BranchMeshBuilder.BranchSkin">Branch skin instance.</typeparam>
		private Dictionary<int, BranchMeshBuilder.BranchSkin> _branchIdToBranchSkin = new Dictionary<int, BranchMeshBuilder.BranchSkin> ();
		/// <summary>
		/// Relationship between branches given their id.
		/// </summary>
		/// <typeparam name="int">Branch id.</typeparam>
		/// <typeparam name="BroccoTree.Branch">Branch instance.</typeparam>
		private Dictionary<int, BroccoTree.Branch> _branchIdToBranch = new Dictionary<int, BroccoTree.Branch> ();
		/// <summary>
		/// Relationship between a branch and its length at the tree.
		/// </summary>
		/// <typeparam name="int">Branch id.</typeparam>
		/// <typeparam name="float">Length at position 0 of the branch.</typeparam>
		private Dictionary<int, float> _branchIdToAccumLength = new Dictionary<int, float> ();
		#endregion

		#region Singleton
		/// <summary>
		/// This class singleton instance.
		/// </summary>
		static BranchMeshMetaBuilder _branchMeshMetaBuilder = null;
		/// <summary>
		/// Gets the singleton instance for this class.
		/// </summary>
		/// <returns>The instance.</returns>
		public static BranchMeshMetaBuilder GetInstance () {
			if (_branchMeshMetaBuilder == null) {
				_branchMeshMetaBuilder = new BranchMeshMetaBuilder ();
			}
			return _branchMeshMetaBuilder;
		}
		#endregion

		#region Setup
		/// <summary>
		/// Process branch skins and tree to apply mapping to branch meshes.
		/// </summary>
		/// <param name="tree">BroccoTree to process.</param>
		/// <param name="branchSkins">List of branch skins belonging to the tree.</param>
		public void BeginUsage (BroccoTree tree,List<BranchMeshBuilder.BranchSkin> branchSkins) {
			BuildBranchIdToBranchSkin (branchSkins);
			_branchIdToBranch.Clear ();
			_branchIdToAccumLength.Clear ();
			for (int i = 0; i < tree.branches.Count; i++) {
				BuildBranchIdToBranch (tree.branches[i], 0f);
			}
			maxGirth = tree.maxGirth;
		}
		/// <summary>
		/// Clears data after this meta builder has been used.
		/// </summary>
		public void EndUsage () {
			_branchIdToBranchSkin.Clear ();
			_branchIdToBranch.Clear ();
			_branchIdToAccumLength.Clear ();
		}
		/// <summary>
		/// Builds a relationship structure between branch ids and the branch skin instance they belong to.
		/// </summary>
		/// <param name="branchSkins">List of BranchSkin instances.</param>
		void BuildBranchIdToBranchSkin (List<BranchMeshBuilder.BranchSkin> branchSkins) {
			_branchIdToBranchSkin.Clear ();
			for (int i = 0; i < branchSkins.Count; i++) {
				for (int j = 0; j < branchSkins [i].totalSegments; j++) {
					if (!_branchIdToBranchSkin.ContainsKey ((int)branchSkins [i].idsSegs [j].x)) {
						_branchIdToBranchSkin.Add ((int)branchSkins [i].idsSegs [j].x, branchSkins [i]);
					}
				}
			}
		}
		/// <summary>
		/// Builds a relationship between branches and their ids.
		/// </summary>
		/// <param name="branch">Branch to process.</param>
		void BuildBranchIdToBranch (BroccoTree.Branch branch, float accumLength) {
			_branchIdToBranch.Add (branch.id, branch);
			_branchIdToAccumLength.Add (branch.id, accumLength);
			for (int i = 0; i < branch.branches.Count; i++) {
				BuildBranchIdToBranch (branch.branches[i], accumLength + branch.length * branch.branches[i].position);
			}
		}
		#endregion

		#region UV Methods
		public List<Vector4> SetMeshUVs (Mesh mesh,
			float mappingXDisplacement = 1f,
			float mappingYDisplacement = 1f,
			int mappingXTiles = 1,
			int mappingYTiles = 1,
			float minGirth = 1f,
			float maxGirth = 1f,
			bool isGirthSensitive = false,
			bool applyLimits = false,
			float mappingXOffset = 0f,
			float mappingYOffset = 0f,
			float mappingWidth = 1f,
			float mappingHeight = 1f)
		{
			// Mark mesh as dynamic.
			mesh.MarkDynamic ();

			// Create job and set variables.
			UVJob uvJob = new UVJob ();
			uvJob.mappingXDisplacement = mappingXDisplacement;
			uvJob.mappingYDisplacement = mappingYDisplacement;
			uvJob.mappingXTiles = mappingXTiles;
			uvJob.mappingYTiles = mappingYTiles;
			uvJob.isGirthSensitive = isGirthSensitive;
			uvJob.minGirth = minGirth;
			uvJob.maxGirth = maxGirth;
			uvJob.mappingLimitXOffset = mappingXOffset;
			uvJob.mappingLimitYOffset = mappingYOffset;
			uvJob.mappingLimitWidth = mappingWidth;
			uvJob.mappingLimitHeight = mappingHeight;
			uvJob.applyLimits = applyLimits;


			// Set UVs
			List<Vector4> uvs = new List<Vector4> ();
			List<Vector4> uv8s = new List<Vector4> ();
			mesh.GetUVs (0, uvs);
			mesh.GetUVs (7, uv8s);
			uvJob.uvs = new NativeArray<Vector4> (uvs.ToArray (), Allocator.TempJob);
			uvJob.uv8s = new NativeArray<Vector4> (uv8s.ToArray (), Allocator.TempJob);

			// Execute job.
			JobHandle uvJobHandle = uvJob.Schedule (uvs.Count, 16);

			// Complete job.
			uvJobHandle.Complete ();

			// Set UVs
			mesh.SetUVs (0, uvJob.uvs);
			mesh.GetUVs (0, uvs);

			// Dispose.
			uvJob.uvs.Dispose ();
			uvJob.uv8s.Dispose ();

			// Return new UVs.
			return uvs;
		}
		#endregion

		#region Tangents
		/// <summary>
		/// Recalculates tangents for a mesh.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		public void RecalculateTangents(Mesh mesh)
		{
			int triangleCount = mesh.triangles.Length;
			int vertexCount = mesh.vertices.Length;

			Vector3[] tan1 = new Vector3[vertexCount];
			Vector3[] tan2 = new Vector3[vertexCount];
			Vector4[] tangents = new Vector4[vertexCount];
			for(long a = 0; a < triangleCount; a+=3)
			{
				long i1 = mesh.triangles[a+0];
				long i2 = mesh.triangles[a+1];
				long i3 = mesh.triangles[a+2];
				Vector3 v1 = mesh.vertices[i1];
				Vector3 v2 = mesh.vertices[i2];
				Vector3 v3 = mesh.vertices[i3];
				Vector2 w1 = mesh.uv[i1];
				Vector2 w2 = mesh.uv[i2];
				Vector2 w3 = mesh.uv[i3];
				float x1 = v2.x - v1.x;
				float x2 = v3.x - v1.x;
				float y1 = v2.y - v1.y;
				float y2 = v3.y - v1.y;
				float z1 = v2.z - v1.z;
				float z2 = v3.z - v1.z;
				float s1 = w2.x - w1.x;
				float s2 = w3.x - w1.x;
				float t1 = w2.y - w1.y;
				float t2 = w3.y - w1.y;
				float r = 1.0f / (s1 * t2 - s2 * t1);
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;
				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;
			}
			for (long a = 0; a < vertexCount; ++a)
			{
				Vector3 n = mesh.normals[a];
				Vector3 t = tan1[a];
				Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
				tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
				tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			}
			mesh.tangents = tangents;
		}
		/// <summary>
		/// Set the mesh tangents to zero.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		public void TangentsToZero (Mesh mesh) {
			int i = mesh.vertices.Length;
			Vector4[] tangents = new Vector4[i];
			for (int j = 0; j < i; j++) {
				tangents[j] = new Vector4 (1, 0, 0, 0);
			}
			mesh.tangents = tangents;
		}
		#endregion
	}
}