using UnityEngine;

namespace Nekonya.AniCatEditor
{
    public static class AniComponentCustomI18N
    {

        #region Ani Director

        public static string AniDirector_PlayableDirectorNotAsset
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    default:
                        return "Playable Director 's Playable Asset not fund, \nplease set up this asset, or create a new asset";
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return "“Playable Director”中的“Playable Asset”项为空, \n请为其指定资源，或新建资源";
                }
            }
        }


        public static string AniDirector_Btn_SaveTimelineData
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    default:
                        return "Save Timeline data";
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return "保存Timeline辅助数据";
                }
            }
        }

        public static string AniDirector_SaveTimelineData_SavePanel_title
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    default:
                        return "Save Timeline data";
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return "保存Timeline辅助数据";
                }
            }
        }

        public static string AniDirector_SaveTimelineData_IsCoverFile
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    default:
                        return "The Path will saved is exists, do you want to cover it?";
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return "将要保存的文件路径已存在，是否覆盖该文件？";
                }
            }
        }

        public static string AniDirector_SaveTimelineData_BtnCoverFile
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    default:
                        return "override it";
                    case SystemLanguage.Chinese:
                    case SystemLanguage.ChineseSimplified:
                        return "覆盖文件";
                }
            }
        }

        #endregion


    }
}

