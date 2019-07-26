using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Nekonya.AniCat;

namespace Nekonya.AniCatEditor
{
    public static class AniCatMenu
    { 
        [MenuItem(AniMenuI18N.MenuItem_GameObject_Base,true,100)]
        static bool Menu_Go_Base_Define() =>  true;


        [MenuItem(AniMenuI18N.MenuItem_GameObject_Base + "/" + AniMenuI18N.MenuItem_GameObject_CreateAniDirector,false,1)]
        static void Menu_Go_CreateAniDirector()
        {
            var go_name = "AniDirector";
            if(GameObject.Find(go_name) != null)
            {
                var i = 1;
                while(GameObject.Find("AniDirector" + i.ToString()) != null)
                {
                    i++;
                }
                go_name = "AniDirector" + i.ToString();
            }

            var go = new GameObject(go_name,new System.Type[] {
                typeof(PlayableDirector),
                typeof(AniDirector),
            });
            go.transform.position = Vector3.zero;

            var playable_director = go.GetComponent<PlayableDirector>();
            playable_director.playOnAwake = false;
            

            Selection.activeGameObject = go;
        }


        [MenuItem(AniMenuI18N.MenuItem_GameObject_Base + "/" + AniMenuI18N.MenuItem_GameObject_CreateAniStage, false, 2)]
        static void Menu_Go_CreateAniStage()
        {
            var go_name = "AniStage";
            if (GameObject.Find(go_name) != null)
            {
                var i = 1;
                while (GameObject.Find("AniStage" + i.ToString()) != null)
                {
                    i++;
                }
                go_name = "AniStage" + i.ToString();
            }

            var go = new GameObject(go_name, new System.Type[] {
                typeof(AniStage),
            });
            go.transform.position = Vector3.zero;

            var stage = go.GetComponent<AniStage>();
            
            if(Selection.activeGameObject != null)
            {
                var ani_director = Selection.activeGameObject.GetComponent<AniDirector>();
                if(ani_director != null)
                {
                    ani_director.mAniStage = stage;


                }
            }


            Selection.activeGameObject = go;
        }

    }
}

