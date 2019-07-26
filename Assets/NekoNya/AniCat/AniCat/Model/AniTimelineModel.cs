using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nekonya.AniCat.Model
{
    /// <summary>
    /// 记录Timeline与编辑场景的绑定关系
    /// </summary>
    [System.Serializable]
    public class AniTimelineModel
    {
        public AniTrackInfo[] Tracks;


        [System.Serializable]
        public struct AniTrackInfo
        {
            public string GameObjectName;
            public string TransformPath;
            public string TrackName;
            public string TrackType;
        }

    }

}


