using System.Threading.Tasks;
using UnityEngine;

namespace VRBuilder.TextToSpeech
{
    /// <summary>
    /// TextToSpeechProvider allows to convert text to AudioClips.This an extension on ITexToSpeechProvider to support conversion of multiple languages together.
    /// </summary>
    public interface IVRBTextToSpeechProvider : ITextToSpeechProvider
    {

        /// <summary>
        /// Loads multiple the AudioClip file for the given text.
        /// </summary>
        Task<AudioClip[]> ConvertMultipleTextToSpeech(string[] texts, string[] languages);

        /// <summary>
        /// Loads the AudioClip file for the given text and language.
        /// </summary>
        Task<AudioClip> ConvertTextToSpeechForLanguage(string text, string language);
    }
}
