
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nekonya.AniCat;
//using System.Threading.Tasks;

public class AniPlayerScene : MonoBehaviour
{
    public GameObject go_Replace; //在运行时替换成这个对象

    private  void Start()
    {
        
        
        

    }


    private void OnGUI()
    {
        if (GUILayout.Button("在空场景播放预先做好的完整动画"))
        {
            PlayAnim();
        }

        if (GUILayout.Button("播放动画，并把动画中的文件替换成自己的"))
        {
            PlayAnim2();
        }
    }

    private void PlayAnim() //在空场景播放预先做好的完整动画
    {
        //准备资源（AniCat本身不处理任何资源加载工作
        var json_text = Resources.Load<TextAsset>("AniDemo/Demo1/Demo1_json");
        var ani_asset = Resources.Load<UnityEngine.Playables.PlayableAsset>("AniDemo/Demo1/Demo1");
        var stage_prefab = Resources.Load<GameObject>("AniDemo/Demo1/Demo1_stage");

        //启动timeline动画
        new AniPlayer("demo1")
            .SetAniJson(json_text.text)     //设置动画辅助文件
            .SetPlayableAsset(ani_asset)    //设置timeline动画文件
            .SetStagePrefab(stage_prefab)   //设置stage(可以不要这个
            .OnPlayFinish(player =>
            {
                Debug.Log("播放结束");
                player.Destory(); //把动画场景干掉
            })
            .SetPosition(new Vector3(2,2,2))
            .Build()
            .Play();
    }


    private void PlayAnim2() //播放动画，并把动画中的cube实时替换掉
    {
        //准备资源（AniCat本身不处理任何资源加载工作
        var json_text = Resources.Load<TextAsset>("AniDemo/Demo1/Demo1_json");
        var ani_asset = Resources.Load<UnityEngine.Playables.PlayableAsset>("AniDemo/Demo1/Demo1");
        var stage_prefab = Resources.Load<GameObject>("AniDemo/Demo1/Demo1_stage");

        //启动timeline动画
        new AniPlayer("demo1")
            .SetAniJson(json_text.text)     //设置动画辅助文件
            .SetPlayableAsset(ani_asset)    //设置timeline动画文件
            .SetStagePrefab(stage_prefab)   //设置stage(可以不要这个
            .OnPlayFinish(player =>
            {
                Debug.Log("播放结束");
                player.Destory(); //把动画场景干掉
            })
            .OnReady((player, gameobjectName, path, trackName, trackType) =>
            {
                if(path == "GameObject/Cube")//就你了，别跑
                {
                    player.SetBinding(trackName, go_Replace.GetComponent<Animator>());
                    //好听，但是这个模型得换成我们解放军的
                }
            })
            .Build()//回调和设置的的操作应该在调用Build之前
            .Play();
    }


}
