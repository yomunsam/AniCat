using Nekonya.AniCat;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine;
using Nekonya.AniCat.Model;
using System.Collections.Generic;
using System.IO;


namespace Nekonya.AniCatEditor.Component
{
    [CustomEditor(typeof(AniDirector))]
    public class AniDirectorCustom : Editor
    {
        private AniDirector mAniDirector;
        private PlayableDirector mDirector;


        private void OnEnable()
        {
            mAniDirector = (AniDirector)target;
            mDirector = mAniDirector.gameObject.GetComponent<PlayableDirector>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (mDirector != null)
            {
                if(mDirector.playableAsset == null)
                {
                    GUILayout.Label(AniComponentCustomI18N.AniDirector_PlayableDirectorNotAsset);
                }
                else
                {
                    //save timeline
                    if(GUILayout.Button(AniComponentCustomI18N.AniDirector_Btn_SaveTimelineData))
                    {
                        var path = EditorUtility.SaveFilePanelInProject(AniComponentCustomI18N.AniDirector_SaveTimelineData_SavePanel_title, mDirector.playableAsset.name, "json", "喵");
                        if (!string.IsNullOrEmpty(path))
                        {
                            SaveTimeLine(path);

                        }
                    }

                    //save stage
                    if(mAniDirector.mAniStage != null)
                    {
                        if(GUILayout.Button("Save Stage to Prefab"))
                        {
                            SaveStage2Prefab();
                        }
                    }

                }
            }

        }

        private void SaveTimeLine(string json_path)
        {
            Debug.Log("尝试保存TimeLine的编辑内容到：" + json_path);
            var model = new AniTimelineModel();

            var lists = new List<AniTimelineModel.AniTrackInfo>();
            var asset = mDirector.playableAsset;
            foreach (var item in asset.outputs)
            {
                if (item.sourceObject != null)
                {
                    var trackInfo = new AniTimelineModel.AniTrackInfo();
                    trackInfo.TrackName = item.streamName;
                    trackInfo.TrackType = item.outputTargetType.ToString();
                    var bind_obj = mDirector.GetGenericBinding(item.sourceObject);
                    if (bind_obj != null)
                    {
                        //trackInfo.GameObjectName = bind_obj.name;
                        UnityEngine.Component bind_component;
                        try
                        {
                            bind_component = (UnityEngine.Component)bind_obj;
                            if (bind_component != null && bind_component.gameObject != null)
                            {
                                trackInfo.GameObjectName = bind_component.gameObject.name;
                                //stage判定
                                if (mAniDirector.mAniStage != null)
                                {
                                    //Debug.Log("喵1");
                                    var parent_stage = bind_component.gameObject.GetComponentInParent<AniStage>();
                                    if (parent_stage != null && mAniDirector.mAniStage == parent_stage)
                                    {
                                        //取stage相对路径
                                        //Debug.Log("喵2");


                                        var trans_path = GetTransformPath(bind_component.transform, parent_stage.transform);
                                        //Debug.Log("喵3：" + trans_path);
                                        trackInfo.TransformPath = trans_path;

                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        
                        
                    }


                    lists.Add(trackInfo);
                }
            }
            //Debug.Log("count:" + lists.Count);
            model.Tracks = lists.ToArray();
            var json_str = JsonUtility.ToJson(model);
            //Debug.Log("json:\n" + json_str);

            var io_path = Path.GetFullPath(json_path);
            File.WriteAllText(io_path, json_str);
            //Debug.Log("写出路径:" + io_path);
            //if (File.Exists(io_path))
            //{
            //    if(EditorUtility.DisplayDialog("File already exists", AniComponentCustomI18N.AniDirector_SaveTimelineData_IsCoverFile, AniComponentCustomI18N.AniDirector_SaveTimelineData_BtnCoverFile, "cancel"))
            //    {
            //        File.WriteAllText(io_path, json_str);
            //    }
            //}
            //else
            //{
            //    File.WriteAllText(io_path, json_str);
            //}

        }

        private string GetTransformPath(Transform cur_trans, Transform util_trans, string cur_path = null)
        {
            if(cur_trans.parent == null)
            {
                return cur_path;
            }
            else
            {
                //是否到置顶层级
                if(cur_trans == util_trans)
                {
                    return cur_path;
                }
                else
                {
                    //记录当前层级
                    string new_path;
                    if (string.IsNullOrEmpty(cur_path))
                    {
                        new_path = cur_trans.name;
                    }
                    else
                    {
                        new_path = cur_trans.name + "/" + cur_path;
                    }
                    return GetTransformPath(cur_trans.parent, util_trans, new_path);
                }
            }
        }


        private void SaveStage2Prefab()
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(mAniDirector.mAniStage.gameObject))
            {
                PrefabUtility.ApplyPrefabInstance(mAniDirector.mAniStage.gameObject, InteractionMode.UserAction);
            }
            else
            {
                //弹窗保存
                var path = EditorUtility.SaveFilePanelInProject("save prefab", mDirector.playableAsset.name, "prefab", "喵");
                if (!string.IsNullOrEmpty(path))
                {
                    var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(mAniDirector.mAniStage.gameObject, path, InteractionMode.UserAction);
                    Selection.activeObject = prefab;
                }
            }

        }


    }
}

