using System;

namespace PetoonsStudio.PSEngine.Credits
{
    [Serializable]
    public struct Chapter
    {
        public Page[] Pages;
    }
    [Serializable]
    public struct Page
    {
        public string Title;
        public Section[] Sections;
    }
    [Serializable]
    public struct Section
    {
        public string Title;
        public Mention[] Mentions;
        public int Columns;
    }
    [Serializable]
    public struct Mention
    {
        public string Job;
        public string Name;
    }
}


