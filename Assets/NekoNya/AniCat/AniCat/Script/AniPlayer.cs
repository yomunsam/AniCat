using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using System.Reflection;


namespace Nekonya.AniCat
{
    public class AniPlayer:IAniPlayer
    {
        private Model.AniTimelineModel mTimelineModel;
        private PlayableAsset mAsset;
        private GameObject mStagePrefab;
        private string mName;
        private Vector3 mPosition = Vector3.zero;

        private GameObject mPlayerBaseGo;
        private GameObject mStageGameObject;
        private AniDirector mAniDirector;

        private Action<IAniPlayer, Model.AniTimelineModel.AniTrackInfo> mCallback_OnReady1;
        private Action<IAniPlayer, Model.AniTimelineModel.AniTrackInfo> mCallback_OnReady_private; //只给这个类内部用，然后它会在另外两个之前调用（其实是发现之前写的方法不太严谨，又不太像细改的情况）
        private Action<IAniPlayer, string, string, string,string> mCallback_OnReady2;
        private Action<IAniPlayer> mCallback_OnPlayFinish;

        private bool mIsDestroy = false;

        public AniPlayer(string name)
        {
            mName = name;
        }

        /// <summary>
        /// Set Json Auxiliary file | 置入Json辅助文件
        /// </summary>
        /// <param name="json_str"></param>
        /// <returns></returns>
        public IAniPlayer SetAniJson(string json_str)
        {
            if (string.IsNullOrEmpty(json_str))
            {
                throw new ArgumentNullException();
            }
            mTimelineModel = JsonUtility.FromJson<Model.AniTimelineModel>(json_str);
            return this;
        }


        public IAniPlayer SetPlayableAsset(PlayableAsset asset)
        {
            mAsset = asset;
            return this;
        }

        public IAniPlayer SetStagePrefab(GameObject prefab)
        {
            mStagePrefab = prefab;
            return this;
        }

        public IAniPlayer SetPosition(Vector3 position)
        {
            mPosition = position;
            return this;
        }


        public IAniPlayer SetBinding(string trackName, UnityEngine.Object obj)
        {
            mAniDirector?.SetBinding(trackName, obj);
            return this;
        }


        public IAniPlayer Build()
        {
            if (mIsDestroy)
                return this;
            var go_player = new GameObject(mName, new Type[]
            {
                typeof(AniDirector),
                //typeof(PlayableDirector)
            });

            mPlayerBaseGo = go_player;
            mPlayerBaseGo.transform.position = mPosition;

            mAniDirector = mPlayerBaseGo.GetComponent<AniDirector>();


            //初始化Director
            mAniDirector.InitAni(mAsset);
            //事件注册
            mAniDirector.OnFinish(() =>
            {
                mCallback_OnPlayFinish?.Invoke(this);
            });


            //stage
            if (mStagePrefab != null)
            {
                mStageGameObject = UnityEngine.Object.Instantiate(mStagePrefab, mPlayerBaseGo.transform);
                var stageObj = mStageGameObject.GetComponent<AniStage>();
                if (stageObj == null)
                {
                    Debug.LogError("can not found \" AniStage\" component your stage prefab ");
                }
                mAniDirector.mAniStage = stageObj;
            }


            //如果需要的话，在player内部执行stage的对象替换
            if (mAniDirector.mAniStage != null)
            {
                
                this.mCallback_OnReady_private += ((player, trackInfo) =>
                {
                    
                    if(!string.IsNullOrEmpty(trackInfo.TransformPath))
                    {
                        //Debug.Log("尝试在stage中寻找：" + trackInfo.TransformPath);
                        //尝试在stage中寻找对应路径的Object
                        var trans = mStageGameObject.transform.Find(trackInfo.TransformPath);
                        if(trans != null)
                        {
                            //Debug.Log("    找到了", trans);
                            //Debug.Log("        寻找类型：" + trackInfo.TrackType);
                            //Debug.Log("尝试获取类型");
                            var type = typen(trackInfo.TrackType);
                            if(type != null)
                            {
                                var obj = trans.GetComponent(type);
                                if (obj != null)
                                {
                                    player.SetBinding(trackInfo.TrackName, obj);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("[AniCat] AniPlayer解析JSON辅助文件时获取类型失败:" + trackInfo.TrackType);
                            }
                            
                        }
                    }
                    
                });
            }


            //触发Ready方法
            if (mTimelineModel != null)
            {
                foreach(var item in mTimelineModel.Tracks)
                {
                    mCallback_OnReady_private?.Invoke(this,item);
                    mCallback_OnReady1?.Invoke(this, item);
                    mCallback_OnReady2?.Invoke(this, item.GameObjectName,item.TransformPath,item.TrackName,item.TrackType);
                }
            }


            return this;
        }

        public IAniPlayer Play()
        {
            if (mIsDestroy)
                return this;
            mAniDirector.Play();   
            return this;
        }


        
        public void Destory()
        {
            mIsDestroy = true;
            mTimelineModel = null;
            mAsset = null;
            mStagePrefab = null;
            mName = string.Empty;
            GameObject.Destroy(mPlayerBaseGo);
            mPlayerBaseGo = null;
            mStageGameObject = null;
            mAniDirector = null;
            mCallback_OnPlayFinish = null;
            mCallback_OnReady1 = null;
            mCallback_OnReady2 = null;
        }


        public IAniPlayer OnReady(Action<IAniPlayer,string,string,string,string> callback) //action五个参数：1. 本体的接口， 2. gameObjectName, 3.TransformPath 4.Timeline轨道名 5.轨道绑定类型
        {
            mCallback_OnReady2+=callback;
            return this;
        }

        public IAniPlayer OnReady(Action<IAniPlayer, Model.AniTimelineModel.AniTrackInfo> callback) //action两个参数：1. 本体的接口， 2. 轨道信息（就是上面那个方法中2,3,4的组合结构体）
        {
            mCallback_OnReady1+=callback;
            return this;
        }


        public IAniPlayer OnPlayFinish(Action<IAniPlayer> callback)
        {
            mCallback_OnPlayFinish+=callback;
            return this;
        }


        private Type typen(string typeName)
        {
            Type type = null;
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }
            return type;
        }


    }
}
