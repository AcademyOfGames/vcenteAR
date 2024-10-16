﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Broccoli.Factory;
using Broccoli.Component;
using Broccoli.Pipe;
using Broccoli.Model;
using Broccoli.Generator;

namespace Broccoli.Examples 
{
    /// <summary>
    /// This line is used to prevent conflicts with other libraries using 'Pipeline'
    /// without namespaces.
    /// </summary>
    using Pipeline = Broccoli.Pipe.Pipeline;
    [ExecuteInEditMode]
    public class ModifyPipeline : MonoBehaviour
    {
        #region Vars
        /// <summary>
        /// The TreeFactory instance to modify.
        /// </summary>
        private TreeFactory _treeFactory;
        /// <summary>
        /// The pipeline inside the TreeFactory instance.
        /// </summary>
        private Pipeline _pipeline;
        /// <summary>
        /// Keeps track of the TreeFactory instance scale.
        /// </summary>
        private float _factoryScale = 1f;
        /// <summary>
        /// Return the TreeFactory scale.
        /// </summary>
        /// <value>Scale value for the factory.</value>
        public float factoryScale {
            get { return _factoryScale; }
        }
        /// <summary>
        /// Key names for SproutMapper elements withing the target pipeline.
        /// </summary>
        /// <value>Key name value.</value>
        public static string[] sproutMapperKeyNames = new string[]{"Autumn", "Green"};
        /// <summary>
        /// Index value for the SproutMapper key name selected.
        /// </summary>
        private int _sproutMapperSelected = 0;
        /// <summary>
        /// Index value for the SproutMapper key name selected.
        /// </summary>
        /// <value>Key name for the SproutMapper.</value>
        public int sproutMapperSelected {
            get { return _sproutMapperSelected; }
        }
        /// <summary>
        /// Flag to enable or disable the SproutGenerator element in the pipeline.
        /// </summary>
        private bool _isSproutMeshGeneratorActive = true;
        /// <summary>
        /// True if the sproutMeshGenerator is enabled, otherwise false.
        /// </summary>
        /// <value></value>
        public bool isSproutMeshGeneratorActive {
            get { return _isSproutMeshGeneratorActive; }
        }
        /// <summary>
        /// Girth value for branches.
        /// </summary>
        private float _girthValue = 1f;
        /// <summary>
        /// Girth value for branches.
        /// </summary>
        public float girthValue {
            get { return _girthValue; }
        }
        #endregion

        #region Monobehaviour Events
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        void Start()
        {
            // Grab the TreeFactory component.
            _treeFactory = GetComponent<TreeFactory>();
            if (_treeFactory != null && _treeFactory.HasValidPipeline ()) {
                _pipeline = _treeFactory.localPipeline;
                _factoryScale = _treeFactory.treeFactoryPreferences.factoryScale;
                GirthTransformElement girthTransformElement =
                    (GirthTransformElement) _pipeline.root.GetDownstreamElement (PipelineElement.ClassType.GirthTransform);
                if (girthTransformElement != null) {
                    _girthValue = girthTransformElement.minGirthAtBase;
                }
            }
        }
        #endregion

        #region Pipeline Processing
        /// <summary>
        /// Process the pipeline loaded at the TreeFactory component.
        /// </summary>
        public void ProcessPipeline () {
            if (_treeFactory != null && _treeFactory.HasValidPipeline ()) {
                _treeFactory.ProcessPipelinePreview ();
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Switches between the available SproutMappers elements found on the pipeline.
        /// </summary>
        public void SwitchSproutMappers () {
            if (_pipeline != null && _pipeline.IsValid ()) {
                if (_sproutMapperSelected == 0) {
                    _pipeline.ReplaceElements (sproutMapperKeyNames[0], sproutMapperKeyNames[1]);
                    _sproutMapperSelected = 1;
                } else {
                    _pipeline.ReplaceElements (sproutMapperKeyNames[1], sproutMapperKeyNames[0]);
                    _sproutMapperSelected = 0;
                }
                // Calling with cache parameter true to get the same tree seed.
                _treeFactory.ProcessPipelinePreview (null, true);
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Toggles the SproutMeshGenerator state..
        /// </summary>
        public void ToggleSproutMeshGenerator () {
            if (_pipeline != null && _pipeline.IsValid ()) {
                // Getting the element downstream from the root element of the pipeline.
                SproutMeshGeneratorElement sproutMeshGeneratorElement =
                    (SproutMeshGeneratorElement) _pipeline.root.GetDownstreamElement (PipelineElement.ClassType.SproutMeshGenerator);
                if (sproutMeshGeneratorElement != null) {
                    sproutMeshGeneratorElement.isActive = !sproutMeshGeneratorElement.isActive;
                    _isSproutMeshGeneratorActive = sproutMeshGeneratorElement.isActive;
                    // Calling with cache parameter true to get the same tree seed.
                    _treeFactory.ProcessPipelinePreview (null, true);
                } else {
                    Debug.LogWarning ("No SproutMeshGenerator was found on this pipeline.");    
                }
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Sets a factory scale for the trees.
        /// </summary>
        /// <param name="newFactoryScale">New value for the factory scale.</param>
        public void SetFactoryScale (float newFactoryScale) {
            if (_treeFactory != null && _treeFactory.HasValidPipeline ()) {
                _treeFactory.treeFactoryPreferences.factoryScale = newFactoryScale;
                _factoryScale = newFactoryScale;
                // Calling with cache parameter true to get the same tree seed.
                _treeFactory.ProcessPipelinePreview (null, true);
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Sets a girth value to the branches.
        /// </summary>
        /// <param name="newGirth">New girth value.</param>
        public void SetGirth (float newGirth) {
            if (_pipeline != null && _pipeline.IsValid ()) {
                // Getting the element downstream from the root element of the pipeline.
                GirthTransformElement girthTransformElement =
                    (GirthTransformElement) _pipeline.root.GetDownstreamElement (PipelineElement.ClassType.GirthTransform);
                if (girthTransformElement != null) {
                    girthTransformElement.minGirthAtBase = newGirth;
                    girthTransformElement.maxGirthAtBase = newGirth;
                    _girthValue = newGirth;
                    // Calling with cache parameter true to get the same tree seed.
                    _treeFactory.ProcessPipelinePreview (null, true);
                } else {
                    Debug.LogWarning ("No SproutMeshGenerator was found on this pipeline.");    
                }
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Randomizes the tree trunk length.
        /// </summary>
        public void RandomizeTrunkLength () {
            if (_pipeline != null && _pipeline.IsValid ()) {
                // Getting the root element of the pipeline, usually the structure generator element (node).
                StructureGeneratorElement structureGeneratorElement =
                    (StructureGeneratorElement) _pipeline.root;
                if (structureGeneratorElement != null) {
                    // Set the max and min length on the roots structure level (trunk generator).
                    structureGeneratorElement.rootStructureLevel.minLengthAtBase = Random.Range (2f, 8f);
                    structureGeneratorElement.rootStructureLevel.maxLengthAtBase = Random.Range (2f, 8f);

                    // Unlock all tree first level branches (in case those are tuned by the curve randomizer function). 
                    BroccoTree tree = _treeFactory.previewTree;
                    // For each first level branch (trunk) in the tree.
                    for (int i = 0; i < tree.branches.Count; i++) {
                        // Clear isTuned on the branch.
                        tree.branches[i].isTuned = false;
                        // Reset curve handles.
                        tree.branches[i].curve.bezierCurves[0].n1.handle2 = Vector3.zero;
                        tree.branches[i].curve.bezierCurves[0].n2.handle1 = Vector3.zero;
                    }

                    // Calling with cache parameter true to get the same tree seed.
                    _treeFactory.ProcessPipelinePreview (null, true);
                } else {
                    Debug.LogWarning ("No StructureGenerator was found on this pipeline.");    
                }
            } else {
                Debug.LogWarning ("The TreeFactory component has no valid pipeline.");
            }
        }
        /// <summary>
        /// Randomizes the tree trunk curve.
        /// </summary>
        public void RandomizeTrunkCurve () {
            if (_treeFactory != null && _pipeline != null && _pipeline.IsValid ()) {
                // Getting the root element of the pipeline, usually the structure generator element (node).
                StructureGeneratorElement structureGeneratorElement =
                    (StructureGeneratorElement) _pipeline.root;
                // Get the structure generator component (any change to a branch curve needs to be commited to this component)
                StructureGeneratorComponent structureGeneratorComponent = 
                    (StructureGeneratorComponent)_treeFactory.componentManager.GetFactoryComponent (structureGeneratorElement);

                // Get the preview tree on the Tree Factory.
                BroccoTree tree = _treeFactory.previewTree;
                BroccoTree.Branch trunk;
                CubicBezierCurve trunkCurve;
                // For each first level branch (trunk) in the tree.
                for (int i = 0; i < tree.branches.Count; i++) {
                    Vector3 handlePos = Vector3.zero;
                    // Set the trunk model.
                    trunk = tree.branches[i];
                    // Get the curve for the trunk (a bezier curve may have multiple chained cubic bezier curves)
                    trunkCurve = trunk.curve.bezierCurves[0];

                    // Set the first node (curve base) handle
                    // Y position (parallel to trunk curve)
                    handlePos.y = trunkCurve.length * Random.Range (0.1f, 0.4f);
                    // XZ position.
                    handlePos.x = trunkCurve.length * Random.Range (-0.3f, 0.3f);
                    handlePos.z = trunkCurve.length * Random.Range (-0.3f, 0.3f);
                    // Set the curve node1 (at base) handle2 (up handle).
                    trunkCurve.n1.handle2 = handlePos;

                    // Set the second node (curve top) handle
                    // Y position (parallel to trunk curve)
                    handlePos.y = -trunkCurve.length * Random.Range (0.1f, 0.4f);
                    // XZ position.
                    handlePos.x = trunkCurve.length * Random.Range (-0.3f, 0.3f);
                    handlePos.z = trunkCurve.length * Random.Range (-0.3f, 0.3f);
                    // Set the curve node2 (at top) handle1 (down handle).
                    trunkCurve.n2.handle1 = handlePos;

                    // Set the branch as tuned, so that the pipeline process won't rewrite the curve.
                    // If you set isTuned to false, the next randomized pipeline process with set the curve handles
                    // to values set by the structure and bend nodes.
                    structureGeneratorComponent.CommitBranchCurve (trunk.guid, 0, Vector3.zero);
                    structureGeneratorComponent.CommitBranchCurve (trunk.guid, 1, Vector3.zero);
                }
                // Calling with cache parameter true to get the same tree seed.
                _treeFactory.ProcessPipelinePreview (null, true);
            } else {
                Debug.LogWarning ("The TreeFactory component is null.");
            }
        }
        #endregion
    }
}
