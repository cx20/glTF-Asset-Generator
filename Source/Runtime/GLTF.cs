﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    /// <summary>
    /// Wrapper class for abstracting the glTF Loader API
    /// </summary>
    internal class GLTF
    {
        /// <summary>
        /// List of scenes in the gltf wrapper
        /// </summary>
        public List<Runtime.Scene> Scenes { get; set; }
        /// <summary>
        /// index of the main scene
        /// </summary>
        public int? MainScene { get; set; }

        public List<string> ExtensionsUsed { get; set; }
        public List<string> ExtensionsRequired { get; set; }

        /// <summary>
        /// Initializes the gltf wrapper
        /// </summary>
        public GLTF()
        {
            Scenes = new List<Runtime.Scene>();
        }
        /// <summary>
        /// Holds the Asset data
        /// </summary>
        public Asset Asset { get; set; }

        /// <summary>
        /// converts the wrapper data into a gltf loader object. 
        /// </summary>
        /// <param name="gltf"></param>
        /// <param name="geometryData"></param>
        public void BuildGLTF(ref glTFLoader.Schema.Gltf gltf, Data geometryData)
        {
            if (Asset != null)
            {
                gltf.Asset = Asset.ConvertToSchema();
            }

            // local variables for generating gltf indices
            List<glTFLoader.Schema.Buffer> buffers = new List<glTFLoader.Schema.Buffer>();
            List<glTFLoader.Schema.BufferView> bufferViews = new List<glTFLoader.Schema.BufferView>();
            List<glTFLoader.Schema.Accessor> accessors = new List<glTFLoader.Schema.Accessor>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            List<glTFLoader.Schema.Node> nodes = new List<glTFLoader.Schema.Node>();
            List<glTFLoader.Schema.Scene> scenes = new List<glTFLoader.Schema.Scene>();
            List<glTFLoader.Schema.Image> images = new List<glTFLoader.Schema.Image>();
            List<glTFLoader.Schema.Sampler> samplers = new List<glTFLoader.Schema.Sampler>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Mesh> meshes = new List<glTFLoader.Schema.Mesh>();

            glTFLoader.Schema.Buffer gBuffer = new glTFLoader.Schema.Buffer
            {
                Uri = geometryData.Name,
            };
            int bufferIndex = 0;


            // for each scene, create a node for each mesh and compute the indices for the scene object
            foreach (Runtime.Scene gscene in Scenes)
            {
                List<int> sceneIndicesSet = new List<int>();
                // loops through each mesh and converts it into a Node, with optional transformation info if available
                for(int nodeIndex = 0; nodeIndex < gscene.Nodes.Count(); ++nodeIndex)
                {
                    glTFLoader.Schema.Node node = gscene.Nodes[nodeIndex].ConvertToSchema(this, nodes, samplers, images, textures, meshes, accessors, materials, bufferViews, ref gBuffer, geometryData, bufferIndex);
                    nodes.Add(node);
                    sceneIndicesSet.Add(nodes.Count() - 1);
                }

                scenes.Add(new glTFLoader.Schema.Scene
                {
                    Nodes = sceneIndicesSet.ToArray()
                });
            }
            if (scenes != null && scenes.Count > 0)
            {
                gltf.Scenes = scenes.ToArray();
                gltf.Scene = 0;
            }
            
            if (meshes != null && meshes.Count > 0)
            {
                gltf.Meshes = meshes.ToArray();
            }
            if (materials != null && materials.Count > 0)
            {
                gltf.Materials = materials.ToArray();
            }
            if (accessors != null && accessors.Count > 0)
            {
                gltf.Accessors = accessors.ToArray();
            }
            if (bufferViews != null && bufferViews.Count > 0)
            {
                gltf.BufferViews = bufferViews.ToArray();
            }

            gltf.Buffers = new[] { gBuffer };
            if (nodes != null && nodes.Count > 0)
            {
                gltf.Nodes = nodes.ToArray();
            }

            if (images.Count > 0)
            {
                gltf.Images = images.ToArray();

            }
            if (textures.Count > 0)
            {
                gltf.Textures = textures.ToArray();
            }
            if (samplers.Count > 0)
            {
                gltf.Samplers = samplers.ToArray();
            }
            if (MainScene.HasValue)
            {
                gltf.Scene = MainScene.Value;
            }
            if (ExtensionsUsed != null)
            {
                gltf.ExtensionsUsed = ExtensionsUsed.ToArray();
            }
            if (ExtensionsRequired != null)
            {
                gltf.ExtensionsRequired = ExtensionsRequired.ToArray();
            }
        }
    }
}
