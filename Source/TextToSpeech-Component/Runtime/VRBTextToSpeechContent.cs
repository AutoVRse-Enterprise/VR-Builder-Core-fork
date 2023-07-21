using System.IO;
using UnityEngine;
using VRBuilder.Core.Configuration;

namespace VRBuilder.TextToSpeech.Audio
{
    /// <summary>
    ///     /// Utility implementation of the <see cref="IVRBTextToSpeechContent"/> interface that provides a default <see cref="IsCached"/> getter.
    /// </summary>
    public abstract class VRBTextToSpeechContent : IVRBTextToSpeechContent
    {
        public abstract string Text { get; set; }
        public abstract string EnglishText { get; set; }
        public abstract string HindiText { get; set; }
        public abstract string TamilText { get; set; }

        /// <inheritdoc/>
        public bool IsCached
        {
            get
            {
                TextToSpeechConfiguration ttsConfiguration = RuntimeConfigurator.Configuration.GetTextToSpeechConfiguration();

                bool IsLanguageCached(string text, string languageCode)
                {
                    if (text == null)
                        return true;

                    string filename = ttsConfiguration.GetUniqueTextToSpeechFilenameForLanguage(text, languageCode);
                    string filePath = $"{ttsConfiguration.StreamingAssetCacheDirectoryName}/{filename}";
                    return File.Exists(Path.Combine(Application.streamingAssetsPath, filePath));
                }

                return IsLanguageCached(EnglishText, "en") &&
                       IsLanguageCached(HindiText, "hi") &&
                       IsLanguageCached(TamilText, "ta");

            }
        }
    }
}
