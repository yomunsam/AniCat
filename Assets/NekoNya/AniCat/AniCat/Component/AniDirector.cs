//鸣谢：https://blog.csdn.net/linxinfa/article/details/79557744
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine.Timeline;
using System;

namespace Nekonya.AniCat
{
    [DisallowMultipleComponent]
    [AddComponentMenu(AniMenuI18N.ComponentMenu_Base + "/" + AniMenuI18N.ComponentMenu_AniDirector)]
    [RequireComponent(typeof(UnityEngine.Playables.PlayableDirector))]
    public class AniDirector : MonoBehaviour
    {
        public AniStage mAniStage;

        private PlayableDirector mPlayableDirector;
        private PlayableAsset mPlayableAsset;
        private Action mOnStopCallback;
        private Action mOnFinishCallback;

        //private Action 

        private Dictionary<string, PlayableBinding> mBindings = new Dictionary<string, PlayableBinding>();
        private Dictionary<string, Dictionary<string, PlayableAsset>> mClips = new Dictionary<string, Dictionary<string, PlayableAsset>>();

        public void InitAni(PlayableAsset aniAsset)
        {
            mPlayableAsset = aniAsset;
            mPlayableDirector.playableAsset = mPlayableAsset;

            foreach(var item in mPlayableAsset.outputs)
            {
                var trackName = item.streamName;
                mBindings.Add(trackName, item);

                var track = item.sourceObject as TrackAsset;
                if (track != null)
                {
                    var cliplist = track.GetClips();
                    foreach (var clip in cliplist)
                    {
                        if (!mClips.ContainsKey(trackName))
                        {
                            mClips.Add(trackName, new Dictionary<string, PlayableAsset>());
                        }
                        var name2Clips = mClips[trackName];
                        if (!name2Clips.ContainsKey(clip.displayName))
                        {
                            name2Clips.Add(clip.displayName, clip.asset as PlayableAsset);
                        }
                    }
                }
                
            }


            mPlayableDirector.stopped += E_OnStop;

        }

        public void SetBinding(string trackName,UnityEngine.Object obj)
        {
            if (mBindings.ContainsKey(trackName))
            {
                //干掉现在的
                var cur_obj = mPlayableDirector.GetGenericBinding(mBindings[trackName].sourceObject);
                if(cur_obj != null)
                {
                    
                    var cur_go = ((Component)cur_obj).gameObject;
                    if(cur_go != null)
                    {
                        Destroy(cur_go);
                    }
                    
                    UnityEngine.Object.Destroy(cur_obj);
                }

                mPlayableDirector.SetGenericBinding(mBindings[trackName].sourceObject, obj);
            }
            else
            {
                Debug.LogWarning("[AniCat]Bind Object to director error: not found such trackName:" + trackName);
            }
        }



        public void Play()
        {
            mPlayableDirector.Play();
            
        }

        


        public void OnStop(Action callback)
        {
            mOnStopCallback += callback;
        }

        public void OnFinish(Action callback)
        {
            mOnFinishCallback += callback;
        }


        private void Awake()
        {
            mPlayableDirector = gameObject.GetComponent<PlayableDirector>();
            mPlayableDirector.playOnAwake = false;

            
        }


        private void Start()
        {
            
            
        }


        private void OnDestroy()
        {
            if(mPlayableDirector!= null)
            {
                mPlayableDirector.stopped -= E_OnStop;
            }
        }


        private void E_OnStop(PlayableDirector director)
        {
            mOnStopCallback?.Invoke();

            
            if (director.time >= director.duration || director.time == 0)
            {
                mOnFinishCallback?.Invoke();
            }
        }



    }
}

