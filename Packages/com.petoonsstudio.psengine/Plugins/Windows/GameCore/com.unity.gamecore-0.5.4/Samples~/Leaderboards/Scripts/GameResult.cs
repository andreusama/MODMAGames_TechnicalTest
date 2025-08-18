using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LeaderboardSample
{
    [Serializable]
    public struct GameResult
    {
        public string Environment;
        public uint DistanceTraveled;
        public uint GotLostCount;
        public uint ItemsFoundCount;

        public string ToEventJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
