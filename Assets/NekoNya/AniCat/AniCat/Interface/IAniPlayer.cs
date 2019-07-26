using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Nekonya.AniCat
{
    public interface IAniPlayer
    {
        IAniPlayer SetAniJson(string json_str);

        IAniPlayer SetPlayableAsset(PlayableAsset asset);

        IAniPlayer SetStagePrefab(GameObject prefab);

        IAniPlayer SetBinding(string trackName, UnityEngine.Object obj);

        IAniPlayer SetPosition(Vector3 position);

        IAniPlayer Build();

        IAniPlayer Play();

        void Destory();

        IAniPlayer OnPlayFinish(Action<IAniPlayer> callback);

        IAniPlayer OnReady(Action<IAniPlayer, string, string, string,string> callback);

        IAniPlayer OnReady(Action<IAniPlayer, Model.AniTimelineModel.AniTrackInfo> callback);

    }
}

