// Staggart Creations (http://staggart.xyz)
// Copyright protected under Unity Asset Store EULA
// Copying or referencing source code for the production of new asset store, or public content, is strictly prohibited!

//^ NOTE: Transforming the spline point to world-space results in the mesh respecting non-uniformly scaled splines.
//But this breaks them when the spline is rotated.
//In favor of rotation, this is not being done
//#define SM_WORLD_SPACE_TRANSFORM

//#define SM_ADDITIONAL_DATA

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

#if MATHEMATICS
using Unity.Mathematics;
#endif
#if SPLINES
using UnityEngine.Splines;
using Interpolators = UnityEngine.Splines.Interpolators;
using System.Linq;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace sc.modeling.splines.runtime
{
    public class SourceMesh
    {
        public Mesh Mesh;
        public Vector3[] SourceVertices;
        public List<int[]> SourceTriangles = new List<int[]>();
        public Vector3[] SourceNormals;
        public List<Vector4> SourceUv0 = new List<Vector4>();
        public Vector4[] SourceTangents;
        public Color[] SourceColors;
        public int SourceVertexCount;
        public int SubMeshCount;
        public Bounds SourceBounds;
        public float MeshLength;
        public float MeshHeight;
        public float SegmentLenght;
        public bool HasUV;
        public bool HasTangents;
        public bool HasSourceVertexColor;
        public int SubMeshesStartIndex;

        public void Initialize(Mesh sourceMesh, Settings settings, int subMeshesStartIndex)
        {
            Mesh = sourceMesh;
            SubMeshesStartIndex = subMeshesStartIndex;
            SubMeshCount = Mesh.subMeshCount;
            SourceVertices = Mesh.vertices;
            SourceVertexCount = SourceVertices.Length;
            SourceNormals = Mesh.normals;
            Mesh.GetUVs(0, SourceUv0);
            SourceTangents = Mesh.tangents;
            SourceColors = Mesh.colors;
            SourceBounds = Mesh.bounds;

            SourceTriangles.Clear();
            for (int submeshIndex = 0; submeshIndex < SubMeshCount; submeshIndex++)
            {
                SourceTriangles.Add(Mesh.GetTriangles(submeshIndex));
            }

            MeshHeight = SourceBounds.size.y;
            MeshLength = math.max(0.1f, SourceBounds.size.z * settings.deforming.scale.z);
            SegmentLenght = MeshLength + settings.distribution.spacing;
            HasUV = SourceUv0.Count > 0;
            HasTangents = SourceTangents.Length > 0;
            HasSourceVertexColor = SourceColors.Length > 0;
        }

        public void RecalculateMeshLength(float zScale, Settings settings)
        {
            MeshLength = math.max(0.1f, SourceBounds.size.z * zScale);
            SegmentLenght = MeshLength + settings.distribution.spacing;
        }
    }

    public static class SplineMeshGenerator
    {
        private static List<SourceMesh> sources = new List<SourceMesh>();
        private static bool hasFirstMesh = false;
        private static bool hasLastMesh = false;

#if SPLINES && MATHEMATICS
        //Mesh data
        private static readonly List<Vector3> vertices = new List<Vector3>();
        private static readonly List<Vector3> normals = new List<Vector3>();
        private static readonly List<Vector4> tangents = new List<Vector4>();
        private static readonly List<Vector4> uv0 = new List<Vector4>();
#if SM_ADDITIONAL_DATA
        private static readonly List<Vector4> uv2 = new List<Vector4>();
#endif

        //Holds an array of indices for each submesh
        private static readonly List<List<int>> triangles = new List<List<int>>();
        private static readonly List<Color> colors = new List<Color>();

        private static bool setVertexColor;

        private static List<CombineInstance> combineInstances = new List<CombineInstance>();

        private static float3 boundsMin;
        private static float3 boundsMax;

        private static float4x4 splineLocalToWorld;

        public static readonly Interpolators.LerpFloat3 Float3Interpolator = new Interpolators.LerpFloat3();
        public static readonly Interpolators.LerpFloat FloatInterpolator = new Interpolators.LerpFloat();

        private static int CalculateSegmentCount(Settings settings, float splineLength, float meshLength, bool closed)
        {
            int segmentCount = settings.distribution.segments;

            if (settings.distribution.autoSegmentCount)
            {
                //Seems to need one extra segment to full close the loop
                if (closed) splineLength += 0.001f;

                if (settings.distribution.stretchToFit)
                {
                    return (int)math.ceil((splineLength / meshLength));
                }

                if (settings.distribution.evenOnly)
                {
                    return (int)math.floor((splineLength / meshLength));
                }
                else
                {
                    return (int)math.ceil((splineLength / meshLength));
                }
            }

            return segmentCount;
        }

        private static int CalculateSequence(Settings settings, float splineLength, bool closed, out List<int> sequence)
        {
            sequence = new List<int>();

            int minSequenceIndex = hasFirstMesh ? 1 : 0;
            int maxSequenceIndex = hasLastMesh ? sources.Count - 1 : sources.Count;

            float totalLength = 0f;
            int segments = 0;
            int sequenceIndex = hasFirstMesh ? 1 : 0;

            if (hasFirstMesh)
            {
                segments++;
                totalLength += sources[0].SegmentLenght;
                sequence.Add(0);
            }

            if (hasLastMesh)
            {
                segments++;
                totalLength += sources[sources.Count - 1].SegmentLenght;
                sequence.Add(sources.Count - 1);
            }

            if (settings.distribution.autoSegmentCount)
            {
                //Seems to need one extra segment to full close the loop
                if (closed) splineLength += 0.001f;

                int index = minSequenceIndex;
                while (totalLength < splineLength)
                {
                    if (settings.distribution.randomSequence)
                        index = UnityEngine.Random.Range(minSequenceIndex, maxSequenceIndex);

                    totalLength += sources[index].SegmentLenght;

                    sequence.Insert(sequenceIndex, index);
                    segments++;
                    sequenceIndex++;

                    if (!settings.distribution.randomSequence)
                    {
                        index++;

                        if (index >= maxSequenceIndex)
                            index = minSequenceIndex;
                    }
                }

                if (settings.distribution.evenOnly && totalLength > splineLength)
                {
                    sequence.Remove(sequence.Last());
                    return segments - 1;
                }
                else
                {
                    return segments;
                }
            }
            else
            {
                int segmentCount = settings.distribution.segments;
                int index = minSequenceIndex;

                while (segments < segmentCount)
                {
                    if (settings.distribution.randomSequence)
                        index = UnityEngine.Random.Range(minSequenceIndex, maxSequenceIndex);

                    sequence.Insert(sequenceIndex, index);
                    segments++;
                    sequenceIndex++;

                    if (!settings.distribution.randomSequence)
                    {
                        index++;

                        if (index >= maxSequenceIndex)
                            index = minSequenceIndex;
                    }
                }

                return segmentCount;
            }
        }

        private static float CalculateMeshLength(List<int> sequence)
        {
            float totalMeshLenght = 0f;
            for (int i = 0; i < sequence.Count; ++i)
            {
                totalMeshLenght += sources[sequence[i]].SegmentLenght;
            }

            return totalMeshLenght;
        }

        /// <summary>
        /// Tiles and deforms the sourceMesh along splines within the container
        /// </summary>
        /// <param name="splineContainer"></param>
        /// <param name="sourceMesh">Input mesh</param>
        /// <param name="worldToLocalMatrix">Transform matrix of the renderer the mesh is to be used on</param>
        /// <param name="settings"></param>
        /// <param name="scaleData"></param>
        /// <param name="rollData"></param>
        /// <param name="redVertexColor"></param>
        /// <param name="greenVertexColor"></param>
        /// <param name="blueVertexColor"></param>
        /// <param name="alphaVertexColor"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Bails out if the spline is too short</exception>
        public static Mesh CreateMesh(SplineContainer splineContainer, List<Mesh> meshSources, float4x4 worldToLocalMatrix, Settings settings,
            List<SplineData<float3>> scaleData = null,
            List<SplineData<float>> rollData = null,
            List<SplineData<SplineMesher.VertexColorChannel>> redVertexColor = null,
            List<SplineData<SplineMesher.VertexColorChannel>> greenVertexColor = null,
            List<SplineData<SplineMesher.VertexColorChannel>> blueVertexColor = null,
            List<SplineData<SplineMesher.VertexColorChannel>> alphaVertexColor = null,
            Mesh firstMesh = null,
            Mesh lastMesh = null
            )
        {
            sources.Clear();
            sources = new List<SourceMesh>();
            Mesh outputMesh = new Mesh();
            var splineCount = splineContainer.Splines.Count;
            //Note: every submesh requires its own CombineInstance
            combineInstances.Clear();

            //Debug.Log($"Generating for {splineCount} spline(s) from {sourceMesh.name} with {submeshCount} submesh(es)");

            boundsMin = Vector3.one * -math.INFINITY;
            boundsMax = Vector3.one * math.INFINITY;

            //Get initial arrays
            int submeshCount = 0;

            hasFirstMesh = firstMesh != null;
            if (hasFirstMesh)
            {
                SourceMesh sourceMesh = new SourceMesh();
                sourceMesh.Initialize(firstMesh, settings, submeshCount);
                sources.Add(sourceMesh);
                submeshCount += sourceMesh.SubMeshCount;
            }

            foreach (var mesh in meshSources)
            {
                SourceMesh sourceMesh = new SourceMesh();
                sourceMesh.Initialize(mesh, settings, submeshCount);
                sources.Add(sourceMesh);
                submeshCount += sourceMesh.SubMeshCount;
            }

            hasLastMesh = lastMesh != null;
            if (hasLastMesh)
            {
                SourceMesh sourceMesh = new SourceMesh();
                sourceMesh.Initialize(lastMesh, settings, submeshCount);
                sources.Add(sourceMesh);
                submeshCount += sourceMesh.SubMeshCount;
            }

            splineLocalToWorld = splineContainer.transform.localToWorldMatrix;

            int vertexCount = 0;

            Profiler.BeginSample($"Spline Mesher: Process {splineCount} Spline(s)");

#if SM_WORLD_SPACE_TRANSFORM
            float3 containerScale = splineContainer.transform.lossyScale;
#endif

            bool hasScaleData = scaleData != null;
            bool hasRollData = rollData != null;
            bool hasRedColorData = redVertexColor != null;
            bool hasGreenColorData = greenVertexColor != null;
            bool hasBlueColorData = blueVertexColor != null;
            bool hasAlphaColorData = alphaVertexColor != null;

            float2 trimming = new float2(settings.distribution.trimStart, settings.distribution.trimEnd);
            for (int splineIndex = 0; splineIndex < splineCount; splineIndex++)
            {
                Spline spline = splineContainer.Splines[splineIndex];

                float splineLength = spline.CalculateLength(splineLocalToWorld);

                //T-values of the trimming
                float2 trimRange = new float2((trimming.x / splineLength), 1f - (trimming.y / splineLength));

                float trimLength = trimming.x + trimming.y;

                splineLength -= trimLength;

                float zScale = settings.deforming.scale.z;

                if (splineLength <= 0.02f)
                {
                    //Debug.LogError($"Spline #{splineIndex} in {splineContainer.name} is too short ({splineLength})");
                    continue;
                }

                //Too short
                /*if (splineLength < segmentLength)
                {
                    //Debug.LogWarning($"Input mesh ({sourceMesh.name}) is larger ({meshLength}) than the length of the spline #{splineIndex} ({splineLength}), no output mesh possible");
                    //continue;
                }*/

                List<int> sequence = new List<int>();
                int segments = CalculateSequence(settings, splineLength, spline.Closed, out sequence);
                if (segments == 0) continue;

                //Scale each segment by the right amount so that they all evenly fit along the spline
                if (settings.distribution.stretchToFit)
                {
                    float totalMeshLength = CalculateMeshLength(sequence);

                    //Scale value needed for each individual segment to achieve full coverage
                    float zScaleDelta = splineLength / totalMeshLength;
                    zScale *= zScaleDelta;

                    //Factor in new scale
                    foreach (var source in sources)
                    {
                        source.RecalculateMeshLength(zScale, settings);
                    }

                    //Recalculate
                    //segments = CalculateSequence(settings, splineLength, spline.Closed, out sequence);

                    //Debug.Log($"Segments:{segments} Segment length: {segmentLength} - Total mesh length: {segments * segmentLength} Spline length {splineLength}. Delta:{zScaleDelta}. Z-scale: {zScale} (new mesh length:{segments * bounds.size.z * zScale})");
                    if (segments == 0) continue;
                }

                //Clear data for current spline, which is to get its own mesh.
                triangles.Clear();
                for (int submeshIndex = 0; submeshIndex < submeshCount; submeshIndex++)
                {
                    triangles.Add(new List<int>());
                }
                vertices.Clear();
                normals.Clear();
                tangents.Clear();
                uv0.Clear();
                colors.Clear();
#if SM_ADDITIONAL_DATA
                uv2.Clear();
#endif

                float3 origin = 0f;
                float3 tangent = 0f;
                float3 up = 0f;
                float3 forward = 0f;
                float3 right = 0f;
                quaternion rotation = quaternion.identity;
                quaternion normalRotation = quaternion.identity;
                float3 splineScale = new float3(1f);

                //int splineSamples = 0;

                int currentVertexCount = 0;
                float segmentOffset = 0f;
                for (int i = 0; i < segments; i++)
                {
                    var currentSource = sources[sequence[i]];
                    float prevZ = -1;

                    Color vertexColor = Color.black;
                    setVertexColor = currentSource.HasSourceVertexColor;

                    for (int v = 0; v < currentSource.SourceVertexCount; v++)
                    {
                        //t-value of vertex over the length of the mesh. Normalized value 0-1
                        float localVertPos = (currentSource.SourceVertices[v].z - currentSource.SourceBounds.min.z) / (currentSource.SourceBounds.max.z - currentSource.SourceBounds.min.z);
                        //localVertPos = math.clamp(localVertPos, 0f, 1f);

                        float distance = (localVertPos * currentSource.MeshLength) + segmentOffset;
                        float center = (0.5f * currentSource.MeshLength) + segmentOffset;

                        //Check if Z-value of vertex is changing, meaning sampling moves forward
                        var resample = (math.abs(distance - prevZ) > 0f);
                        //resample = true;
                        if (resample) prevZ = distance;

                        float3 splinePoint = origin;

                        float t = distance / splineLength;

                        //Important optimization. If a mesh has edge loops (vertices sharing the same Z-value) the spline gets unnecessarily re-sampled
                        //In this case, all the spline-related information is identical, so doesn't need to be recalculated.
                        if (resample)
                        {
                            Profiler.BeginSample("Spline Mesher: Sample Spline");
                            {
                                //Remap normalized (0-1) t-range to trimmed range
                                t = math.lerp(trimRange.x, trimRange.y, t);

                                t = math.clamp(t, 0.000001f, 0.999999f); //Ensure a tangent can always be derived

                                spline.Evaluate(t, out origin, out tangent, out up);
                                //SplineCache.Evaluate(splineContainer, splineIndex, t, out origin, out tangent, out up);
                                //splineSamples++;

                                //Recalculate tangent, required for a correct value on scaled spline containers
                                //float3 prevPosition = spline.EvaluatePosition(t - 0.001f);
                                //tangent = origin - prevPosition;

                                forward = math.normalize(tangent);
                                right = math.cross(up, forward);
                                rotation = quaternion.LookRotation(forward, up);

                                if (settings.deforming.ignoreKnotRotation && settings.deforming.rollAngle == 0)
                                {
                                    rotation = RollCorrectedRotation(forward);
                                    right = math.rotate(rotation, math.right());
                                }

                                if (settings.deforming.rollAngle != 0f || hasRollData)
                                {
                                    //Aligned conforming will completely override this rotation, so skip the calculations
                                    if ((settings.conforming.enable && settings.conforming.align) == false)
                                    {
                                        Profiler.BeginSample("Spline Mesher: Calculate roll rotation");
                                        {
                                            float rollInterpolator = settings.deforming.rollMode == Settings.Deforming.RollMode.PerSegment ? (center / splineLength) : t;

                                            float rollFrequency = settings.deforming.rollFrequency > 0 ? settings.deforming.rollFrequency * (rollInterpolator * splineLength) : 1f;
                                            float rollAngle = settings.deforming.rollAngle;

                                            float rollValue = rollAngle * rollFrequency;

                                            if (hasRollData)
                                            {
                                                if (rollData[splineIndex].Count > 0)
                                                {
                                                    rollValue += rollData[splineIndex].Evaluate(spline,
                                                        spline.ConvertIndexUnit(rollInterpolator, PathIndexUnit.Normalized, settings.deforming.rollPathIndexUnit),
                                                        settings.deforming.rollPathIndexUnit, FloatInterpolator);
                                                }
                                            }

                                            //Not needed, only a tiny bit of skewing
                                            //forward = math.normalize(spline.EvaluateTangent(rollInterpolator));

                                            //rotation = Quaternion.AngleAxis(-rollValue, forward) * rotation;
                                            rotation = math.mul(quaternion.AxisAngle(forward, -rollValue * Mathf.Deg2Rad), rotation);

                                            //Recalculate vectors, particularly for the curve offset functionality later on
                                            right = math.mul(rotation, math.right());
                                            up = math.mul(rotation, math.up());
                                        }
                                        Profiler.EndSample();
                                    }
                                }
                            }
                            Profiler.EndSample();

                            splineScale = new float3(1f);
                            if (hasScaleData)
                            {
                                //Important to not attempt to sample empty data, as this results in a scale of (0,0,0)
                                if (scaleData[splineIndex].Count > 0)
                                {
                                    splineScale = scaleData[splineIndex].Evaluate(spline,
                                        spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, settings.deforming.scalePathIndexUnit),
                                        settings.deforming.scalePathIndexUnit, Float3Interpolator);
                                }
                            }

                            //Counter scale of spline container transform
                            //splineScale /= containerScale;
                            splineScale.x *= settings.deforming.scale.x;
                            splineScale.y *= settings.deforming.scale.y;

#if SM_WORLD_SPACE_TRANSFORM
                            //Counter scale again to allow for non-uniform scaling of the spline
                            splineScale.x /= containerScale.x;
                            splineScale.y /= containerScale.y;
#endif

                            //Never scale the Z-axis, as this affects distribution of the vertices
                            splineScale.z = 0f;

                            //Update
                            splinePoint = origin;
                            normalRotation = rotation;
                        }

                        //Outside of resampling scope, since the color may be different over the vertical length of the mesh
                        Profiler.BeginSample("Spline Mesher: Sample Vertex Colors");
                        {
                            vertexColor = currentSource.HasSourceVertexColor ? currentSource.SourceColors[v] : Color.clear;

                            float vcSamplePos = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, settings.color.pathIndexUnit);

                            if (hasRedColorData)
                            {
                                if (redVertexColor[splineIndex].Count > 0)
                                {
                                    vertexColor.r = redVertexColor[splineIndex].Evaluate(spline, vcSamplePos, settings.color.pathIndexUnit,
                                        new SplineMesher.VertexColorChannel.LerpVertexColorData(vertexColor.r));

                                    setVertexColor = true;
                                }
                            }
                            if (hasGreenColorData)
                            {
                                if (greenVertexColor[splineIndex].Count > 0)
                                {
                                    vertexColor.g = greenVertexColor[splineIndex].Evaluate(spline, vcSamplePos, settings.color.pathIndexUnit,
                                        new SplineMesher.VertexColorChannel.LerpVertexColorData(vertexColor.g));

                                    setVertexColor = true;
                                }
                            }
                            if (hasBlueColorData)
                            {
                                if (blueVertexColor[splineIndex].Count > 0)
                                {
                                    vertexColor.b = blueVertexColor[splineIndex].Evaluate(spline, vcSamplePos, settings.color.pathIndexUnit,
                                        new SplineMesher.VertexColorChannel.LerpVertexColorData(vertexColor.b));

                                    setVertexColor = true;
                                }
                            }
                            if (hasAlphaColorData)
                            {
                                if (alphaVertexColor[splineIndex].Count > 0)
                                {
                                    vertexColor.a = alphaVertexColor[splineIndex].Evaluate(spline, vcSamplePos, settings.color.pathIndexUnit,
                                        new SplineMesher.VertexColorChannel.LerpVertexColorData(vertexColor.a));

                                    setVertexColor = true;
                                }
                            }
                        }
                        Profiler.EndSample();

                        if (settings.conforming.enable)
                        {
                            Profiler.BeginSample("Spline Mesher: Conform to colliders");

                            //Vertex position in spline's local space to world-space
                            float3 positionWS = math.transform(splineLocalToWorld, splinePoint);

                            if (PerformConforming(positionWS, settings.conforming, currentSource.MeshHeight, out float3 hitPosition, out float3 hitNormal))
                            {
                                //Convert information from world-space back to spline's local space
                                hitPosition = splineContainer.transform.InverseTransformPoint(hitPosition);
                                hitNormal = splineContainer.transform.InverseTransformVector(hitNormal);

                                splinePoint.y = hitPosition.y;

                                quaternion hitRotation = quaternion.LookRotationSafe(tangent, hitNormal);

                                //Copy normal of surface, to be used for deforming
                                if (settings.conforming.align)
                                {
                                    rotation = hitRotation;
                                }

                                if (settings.conforming.blendNormal)
                                {
                                    normalRotation = hitRotation;
                                }
                            }
                            Profiler.EndSample();
                        }

                        splinePoint += right * settings.deforming.curveOffset.x;
                        splinePoint.y += settings.deforming.curveOffset.y;

                        Profiler.BeginSample("Spline Mesher: Transform Vertices");

                        float3 vertexPositionLocal = (float3)currentSource.SourceVertices[v] + (math.forward() * settings.distribution.spacing);
                        vertexPositionLocal.x += settings.deforming.pivotOffset.x;
                        vertexPositionLocal.y += settings.deforming.pivotOffset.y;

                        //Transform vertex to spline point and rotation (spline's world-space)
#if SM_WORLD_SPACE_TRANSFORM //See note at top
                        splinePoint = math.mul(splineLocalToWorld, new float4(splinePoint, 1.0f)).xyz;
                        //rotation = splineContainer.transform.rotation * rotation;
#endif

                        float3 position = splinePoint + math.rotate(rotation, vertexPositionLocal * splineScale);

#if !SM_WORLD_SPACE_TRANSFORM
                        //Transform position from spline's local-space into world-space
                        float3 vertexPosition = math.mul(splineLocalToWorld, new float4(position, 1.0f)).xyz;
#else
                        //Already in world-space
                        float3 vertexPosition = position;
#endif

                        //Make that the local-space position of the mesh filter
                        vertexPosition = math.mul(worldToLocalMatrix, new float4(vertexPosition, 1.0f)).xyz;

                        //Also transform the normal
                        float3 vertexNormal = math.rotate(normalRotation, currentSource.SourceNormals[v]);

                        if (currentSource.HasTangents)
                        {
                            float4 sourceTangent = new float4(currentSource.SourceTangents[v]);

                            float3 vertexTangent = math.rotate(normalRotation, sourceTangent.xyz);

                            tangents.Add(new float4(vertexTangent, 1.0f));
                        }

                        Profiler.EndSample();

                        //Extend bounds as it expands
                        boundsMin = math.min(position, boundsMin);
                        boundsMax = math.max(position, boundsMax);

                        //Assign vertex attributes
                        vertices.Add(vertexPosition);

                        normals.Add(vertexNormal);

                        if (currentSource.HasUV)
                        {
                            Vector4 uv = currentSource.SourceUv0[v];

                            if (settings.uv.stretchMode == Settings.UV.StretchMode.U) uv.x = t;
                            if (settings.uv.stretchMode == Settings.UV.StretchMode.V) uv.y = t;

                            uv = (uv * settings.uv.scale) + settings.uv.offset;

                            if (settings.mesh.storeGradientsInUV)
                            {
                                //Normalized Distance
                                uv.z = t;
                                //Normalized Height
                                uv.w = (float)math.abs(vertexPositionLocal.y / (currentSource.MeshHeight * splineScale.y));
                            }

                            uv0.Add(uv);
                        }
                        if (setVertexColor) colors.Add(vertexColor);

#if SM_ADDITIONAL_DATA
                        uv2.Add(new Vector4(
                            (float)i/segments, //Segment ID
                            (float)(v * (i+1))/(segments * sourceVertexCount), //Vertex ID
                            ((float)splineIndex/splineCount), //Spline ID
                            noise.cnoise(new float2(v+ distance * splineLength, i + vertexPositionLocal.y * meshHeight)) //Noise
                                ));
#endif
                    }

                    for (int submeshIndex = 0; submeshIndex < currentSource.SubMeshCount; submeshIndex++)
                    {
                        var triCount = currentSource.SourceTriangles[submeshIndex].Length;
                        int x = triangles[currentSource.SubMeshesStartIndex + submeshIndex].Count;
                        for (int v = 0; v < triCount; v++)
                        {
                            var value = currentSource.SourceTriangles[submeshIndex][v] + currentVertexCount;
                            var index = x + v;
                            triangles[currentSource.SubMeshesStartIndex + submeshIndex].Insert(index, value);
                        }
                    }

                    segmentOffset += currentSource.SegmentLenght;
                    currentVertexCount += currentSource.SourceVertexCount;
                }

                var splineVertCount = vertices.Count;
                vertexCount += splineVertCount * submeshCount;

                //Debug.Log($"Estimated spline samples: {segments * sourceVertexCount}. Actual spline samples performed: {splineSamples}. Improvement: {100f/(1f/((float)(segments * sourceVertexCount) / (float)splineSamples) * 100f)*100f}%");

                Profiler.BeginSample($"Spline Mesher: Set mesh data for spline #{splineIndex}");

                var splineMesh = new Mesh();
                splineMesh.indexFormat = splineVertCount >= 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;
                splineMesh.subMeshCount = submeshCount;
                splineMesh.SetVertices(vertices, 0, splineVertCount, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);
                splineMesh.SetNormals(normals, 0, splineVertCount, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);

                if (tangents.Count > 0) splineMesh.SetTangents(tangents);
                if (uv0.Count > 0) splineMesh.SetUVs(0, uv0);
#if SM_ADDITIONAL_DATA
                splineMesh.SetUVs(2, uv2);
#endif
                if (setVertexColor) splineMesh.SetColors(colors);

                for (int submeshIndex = 0; submeshIndex < submeshCount; submeshIndex++)
                {
                    splineMesh.SetIndices(triangles[submeshIndex], MeshTopology.Triangles, submeshIndex, false);

                    CombineInstance combineInstance = new CombineInstance()
                    {
                        mesh = splineMesh,
                        subMeshIndex = submeshIndex
                    };

                    combineInstances.Add(combineInstance);
                }

                Profiler.EndSample();
                //Debug.Log($"Generated mesh for spline #{splineIndex} with {submeshCount} submeshs");

                foreach (var source in sources)
                {
                    source.RecalculateMeshLength(settings.deforming.scale.z, settings);
                }
            }

            Profiler.EndSample();

            Profiler.BeginSample("Spline Mesher: Composite Output Mesh");

            outputMesh.indexFormat = vertexCount >= 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;

            //Note: Warning about Instance X being null is attributed to a Spline having a length of 0. Therefor it was counted, but no mesh was created for it.
            //Bug: if submeshes aren't merged, a submesh is created for each spline
            var mergeSubmeshes = submeshCount == 1 && !settings.mesh.separateSubmeshes;
            outputMesh.CombineMeshes(combineInstances.ToArray(), mergeSubmeshes, false);

            //Debug.Log($"Combined {splineCount} spline meshes from {sourceMesh.name} with {outputMesh.subMeshCount} submeshes");

            outputMesh.UploadMeshData(!settings.mesh.keepReadable);
            outputMesh.bounds.SetMinMax(boundsMin, boundsMax);

            outputMesh.name = $"{meshSources[0].name} Spline";

            Profiler.EndSample();
            //Test, to verify if normals were rotated correctly
            //outputMesh.RecalculateNormals();

            return outputMesh;
        }

        public static bool PerformConforming(float3 positionWS, Settings.Conforming settings, float objectHeight, out float3 hitPosition, out float3 hitNormal)
        {
            var validHit = false;

            float dist = math.max(objectHeight + settings.seekDistance, 1f);

            hitPosition = float3.zero;
            hitNormal = float3.zero;
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(positionWS + (math.up() * dist), -math.up(), out hit, dist * 2f, settings.layerMask, QueryTriggerInteraction.Ignore))
            {
                validHit = true;

                if (settings.terrainOnly)
                {
                    validHit = hit.collider.GetType() == typeof(TerrainCollider); ;

                    if (validHit == false) return false;
                }

                hitPosition = hit.point;
                hitNormal = hit.normal;
            }

            return validHit;
        }

        /// <summary>
        /// Apply generic transforms to the mesh
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rotation">Euler rotation</param>
        /// <param name="flipX"></param>
        /// <param name="flipY"></param>
        /// <returns></returns>
        public static Mesh TransformMesh(Mesh input, Vector3 rotation, bool flipX, bool flipY)
        {
            var rotationAmount = math.abs(math.length(rotation));

            if (rotationAmount > 0.01f || flipX || flipY)
            {
                Vector3[] outputVertices = input.vertices;
                int vertexCount = outputVertices.Length;
                Vector3[] outputNormals = input.normals;
                int[] outputTriangles = input.triangles;
                int triCount = outputTriangles.Length;

                Bounds outputBounds = input.bounds;
                if (rotationAmount > 0.01f)
                {
                    (rotation.x, rotation.z) = (rotation.z, rotation.x);

                    outputBounds = new Bounds();

                    Quaternion m_meshRotation = Quaternion.Euler(rotation);
                    for (int i = 0; i < vertexCount; i++)
                    {
                        outputVertices[i] = math.rotate(m_meshRotation, outputVertices[i]);

                        outputBounds.Encapsulate(outputVertices[i]);

                        outputNormals[i] = math.rotate(m_meshRotation, outputNormals[i]);
                    }
                }

                if (flipX || flipY)
                {
                    //Reverse triangle order if negatively scaled
                    var triangleCount = triCount / 3;
                    for (int j = 0; j < triangleCount; j++)
                    {
                        (outputTriangles[j * 3], outputTriangles[j * 3 + 1]) = (outputTriangles[j * 3 + 1], outputTriangles[j * 3]);
                    }

                    //Rotate normals
                    Quaternion m_meshRotation = Quaternion.Euler(flipY ? 180f : 0f, flipX ? 180f : 0f, 0f);
                    for (int i = 0; i < vertexCount; i++)
                    {
                        outputNormals[i] = math.rotate(m_meshRotation, outputNormals[i]);
                    }
                }

                Mesh output = new Mesh();
                output.name = input.name;
                output.SetVertices(outputVertices, 0, vertexCount, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);

                output.subMeshCount = input.subMeshCount;
                for (int i = 0; i < input.subMeshCount; i++)
                {
                    output.SetTriangles(input.GetTriangles(i), i);
                }

                //output.RecalculateBounds();
                output.bounds = outputBounds;

                output.uv = input.uv;
                output.uv2 = input.uv2;
                output.normals = outputNormals;
                output.colors = input.colors;
                output.tangents = input.tangents;
                output.subMeshCount = input.subMeshCount;

                //Copy readable state
                output.UploadMeshData(input.isReadable);

                return output;
            }

            return input;
        }

        public static quaternion RollCorrectedRotation(float3 forward)
        {
            float3 euler = Quaternion.LookRotation(forward, math.up()).eulerAngles;
            return quaternion.AxisAngle(math.up(), euler.y * Mathf.Deg2Rad);
        }

        private static readonly Vector2[] corners = new[]
        {
            new Vector2(-0.5f, -0.5f), //Bottom-left
            new Vector2(-0.5f, 0.5f), //Top-left
            new Vector2(0.5f, 0.5f),  //Top-right
            new Vector2(0.5f, -0.5f), //Bottom-right
            new Vector2(-0.5f, -0.5f), //Bottom-right
        };

        /// <summary>
        /// Creates a cube mesh from the input mesh's bounds.
        /// </summary>
        /// <param name="sourceMesh"></param>
        /// <param name="subdivisions">Number of edge loops across the length</param>
        /// <param name="caps">Create two triangles at the front and back</param>
        /// <returns></returns>
        public static Mesh CreateBoundsMesh(Mesh sourceMesh, int subdivisions = 0, bool caps = false)
        {
            Bounds m_bounds = sourceMesh.bounds;

            Mesh boundsMesh = new Mesh();
            boundsMesh.name = $"{sourceMesh.name} Bounds";

            Vector3 scale = m_bounds.size;
            Vector3 offset = m_bounds.center;

            int edges = 4;
            subdivisions = Mathf.Max(0, subdivisions);
            int lengthSegments = subdivisions + 1;

            int xCount = edges + 1;
            int zCount = lengthSegments + 1;
            int numVertices = xCount * zCount;

            List<Vector3> mVertices = new List<Vector3>();
            List<int> mTriangles = new List<int>();

            float scaleZ = scale.z / lengthSegments;

            for (int z = 0; z < zCount; z++)
            {
                //Move clockwise to position vertices in each corner around the edge loop
                for (int x = 0; x < xCount; x++)
                {
                    Vector3 vertex;

                    vertex.x = (corners[x].x * scale.x) + offset.x;
                    vertex.y = (corners[x].y * scale.y) + offset.y;
                    vertex.z = z * scaleZ - (scale.z * 0.5f) + offset.z;

                    mVertices.Add(vertex);
                }

                if (z < zCount - 1) //Stop at 2nd last row
                {
                    for (int x = 0; x < edges; x++)
                    {
                        mTriangles.Insert(0, (z * xCount) + x);
                        mTriangles.Insert(1, ((z + 1) * xCount) + x);
                        mTriangles.Insert(2, (z * xCount) + x + 1);

                        mTriangles.Insert(3, ((z + 1) * xCount) + x);
                        mTriangles.Insert(4, ((z + 1) * xCount) + x + 1);
                        mTriangles.Insert(5, (z * xCount) + x + 1);
                    }
                }
            }

            if (caps)
            {
                //Back quad

                mTriangles.Add(1);
                mTriangles.Add(2);
                mTriangles.Add(0);

                mTriangles.Add(2);
                mTriangles.Add(3);
                mTriangles.Add(0);

                //Front quad
                mTriangles.Add(numVertices - 4);
                mTriangles.Add(numVertices - 5);
                mTriangles.Add(numVertices - 3);

                mTriangles.Add(numVertices - 2);
                mTriangles.Add(numVertices - 3);
                mTriangles.Add(numVertices - 5);

            }

            boundsMesh.SetVertices(mVertices, 0, numVertices, MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            boundsMesh.subMeshCount = 1;
            boundsMesh.SetIndices(mTriangles, MeshTopology.Triangles, 0, false);
            boundsMesh.RecalculateNormals();
            boundsMesh.bounds = m_bounds;

            return boundsMesh;
        }
#endif
    }
}