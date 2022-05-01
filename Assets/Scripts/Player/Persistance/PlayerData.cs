using System;
using System.Collections.Generic;
using static SuitColorSync;

namespace XReal.XTown.Persistance
{
    [Serializable]
    public struct PlayerData
    {
        public string id;
        public string name;
        public ColorEnum suitColor;
        public List<int> myFaces;
    }

}
