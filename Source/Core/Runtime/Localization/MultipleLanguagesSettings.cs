

using UnityEngine;
using VRBuilder.Core.Runtime.Utils;

namespace VRBuilder.Core.Internationalization
{
    /// <summary>
    /// Language settings for VR Builder.
    /// </summary>

    [CreateAssetMenu(fileName = "MultipleLanguageSettings", menuName = "AutoVRse/VRseBuilder/MultipleLanguageSettings", order = 1)]


    public class MultipleLanguagesSettings : SettingsObject<MultipleLanguagesSettings>
    {
        /// <summary>
        /// Languages which should be used.
        /// </summary>
        public string ApplicationLanguage = "En";

        /// <summary>
        /// Returns the currently active language, will be stored for one session.
        /// </summary>
        public string[] ActiveLanguages;

        /// <summary>
        /// Returns the active or default language.
        /// </summary>
        public string[] ActiveOrDefaultLanguage
        {
            get
            {
                Debug.Log("Active languages are" + ActiveLanguages.Length);
                if (ActiveLanguages == null)
                {
                    ActiveLanguages[0] = ApplicationLanguage;
                }

                return ActiveLanguages;
            }
        }
    }
}
