using System;
using System.Text;
using Unity.Profiling;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class MemoryProfilerViewer : MonoBehaviour
    {
        private string m_StatsText;
        private ProfilerRecorder m_TotalReservedMemoryRecorder;
        private ProfilerRecorder m_GCReservedMemoryRecorder;
        private ProfilerRecorder m_TextureMemoryRecorder;
        private ProfilerRecorder m_MeshMemoryRecorder;

        protected const string TOTAL_RESERVED_STAT_NAME = "Total Reserved Memory";
        protected const string GC_RESERVED_STAT_NAME = "GC Reserved Memory";
        protected const string TEXTURE_MEMORY_STAT_NAME = "Texture Memory";
        protected const string MESH_MEMORY_STAT_NAME = "Mesh Memory";

        protected virtual int StringBuilderCapacity => 500;
        protected virtual Rect TextAreaRect => new Rect(10, 30, 250, 70);

        protected enum Kinds
        {
            B = 0,
            KB = 1,
            MB = 2,
            GB = 3,
            TB = 4,
        }

        public void Show()
        {
            enabled = true;
        }

        public void Hide()
        {
            enabled = false;
        }

        protected virtual void OnEnable()
        {
            StartRecorders();
        }

        protected virtual void OnDisable()
        {
            DisposeRecorders();
        }

        protected virtual void StartRecorders()
        {
            m_TotalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, TOTAL_RESERVED_STAT_NAME);
            m_GCReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, GC_RESERVED_STAT_NAME);
            m_TextureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, TEXTURE_MEMORY_STAT_NAME);
            m_MeshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, MESH_MEMORY_STAT_NAME);
        }

        protected virtual void DisposeRecorders()
        {
            m_TotalReservedMemoryRecorder.Dispose();
            m_GCReservedMemoryRecorder.Dispose();
            m_TextureMemoryRecorder.Dispose();
            m_MeshMemoryRecorder.Dispose();
        }

        protected virtual void Update()
        {
            var sb = new StringBuilder(StringBuilderCapacity);
            sb.AppendLine(FormatLine(TOTAL_RESERVED_STAT_NAME, m_TotalReservedMemoryRecorder, Kinds.MB));
            sb.AppendLine(FormatLine(GC_RESERVED_STAT_NAME, m_GCReservedMemoryRecorder, Kinds.MB));
            sb.AppendLine(FormatLine(TEXTURE_MEMORY_STAT_NAME, m_TextureMemoryRecorder, Kinds.MB));
            sb.AppendLine(FormatLine(MESH_MEMORY_STAT_NAME, m_MeshMemoryRecorder, Kinds.MB));

            m_StatsText = sb.ToString();
        }

        protected string FormatLine(string name, ProfilerRecorder recorder, Kinds kind)
        {
            if (recorder.Valid)
                return $"{name}: {String.Format("{0:F3}", To(recorder.LastValue, kind))}{kind.ToString()}";
            return "";
        }

        protected void OnGUI()
        {
            GUI.TextArea(TextAreaRect, m_StatsText);
        }

        protected double To(double value, Kinds kind) => value / Math.Pow(1024, (int)kind);
    }
}
