using System;
using System.Threading.Tasks;
using VRBuilder.Core.IO;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using static UnityEditor.PlayerSettings.Switch;
#endif

namespace VRBuilder.TextToSpeech
{
    /// <summary>
    /// The disk based provider for text to speech, which is using the streaming assets folder.
    /// On the first step we check if the application has files provided on delivery.
    /// If there is no compatible file found, will download the file from the given
    /// fallback TextToSpeechProvider.
    /// </summary>
    public class FileTextToSpeechProvider : IVRBTextToSpeechProvider
    {
        protected TextToSpeechConfiguration Configuration;

        public FileTextToSpeechProvider(TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<AudioClip> ConvertTextToSpeech(string text)
        {
            string filename = Configuration.GetUniqueTextToSpeechFilename(text);
            string filePath = GetPathToFile(filename);
            AudioClip audioClip = null;

            if (await IsFileCached(filePath))
            {
                byte[] bytes = await GetCachedFile(filePath);
                float[] sound = TextToSpeechUtils.ShortsInByteArrayToFloats(bytes);

                audioClip = AudioClip.Create(text, channels: 1, frequency: 48000, lengthSamples: sound.Length, stream: false);
                audioClip.SetData(sound, 0);
            }
            else
            {
                Debug.Log($"No audio cached for TTS string. Audio will be generated in real time.");
                audioClip = await TextToSpeechProviderFactory.Instance.CreateProvider().ConvertTextToSpeech(text);                
            }

            if (audioClip == null)
            {
                throw new CouldNotLoadAudioFileException($"AudioClip is null for text '{text}'");
            }

            return audioClip;
        }

        public async Task<AudioClip> ConvertTextToSpeechForLanguage(string text, string language)
        {
            string filename = Configuration.GetUniqueTextToSpeechFilenameForLanguage(text,language);
            string filePath = GetPathToFile(filename);
            AudioClip audioClip = null;

            if (await IsFileCached(filePath))
            {
                byte[] bytes = await GetCachedFile(filePath);
                float[] sound = TextToSpeechUtils.ShortsInByteArrayToFloats(bytes);

                audioClip = AudioClip.Create(text, channels: 1, frequency: 48000, lengthSamples: sound.Length, stream: false);
                audioClip.SetData(sound, 0);
            }
            else
            {
                Debug.Log($"No audio cached for TTS string. Audio will be generated in real time.");
                IVRBTextToSpeechProvider vRBTextToSpeechProvider = (IVRBTextToSpeechProvider)TextToSpeechProviderFactory.Instance.CreateProvider();
                audioClip = await vRBTextToSpeechProvider.ConvertTextToSpeechForLanguage(text, language);
            }

            if (audioClip == null)
            {
                throw new CouldNotLoadAudioFileException($"AudioClip is null for text '{text}'");
            }

            return audioClip;
        }

        public async Task<AudioClip[]> ConvertMultipleTextToSpeech(string[] texts, string[] languages)
        {
            string[] filenames = new string[5];
            string[] filePaths = new string[5];
            AudioClip[] audioClips = new AudioClip[5];
            for (int i = 0; i < languages.Length; i++)
            {
                filenames[i] = Configuration.GetUniqueTextToSpeechFilenameForLanguage(texts[i], languages[i]);
                filePaths[i] = GetPathToFile(filenames[i]);
                audioClips[i] = null;

                if (await IsFileCached(filePaths[i]))
                {
                    byte[] bytes = await GetCachedFile(filePaths[i]);
                    float[] sound = TextToSpeechUtils.ShortsInByteArrayToFloats(bytes);

                    audioClips[i] = AudioClip.Create(texts[i], channels: 1, frequency: 48000, lengthSamples: sound.Length, stream: false);
                    audioClips[i].SetData(sound, 0);
                }
                else
                {
                    Debug.Log($"No audio cached for TTS string. Audio will be generated in real time.");
                    IVRBTextToSpeechProvider vRBTextToSpeechProvider = (IVRBTextToSpeechProvider)TextToSpeechProviderFactory.Instance.CreateProvider();
                    audioClips = await vRBTextToSpeechProvider.ConvertMultipleTextToSpeech(texts, languages);
                }

                if (audioClips[i] == null)
                {
                    throw new CouldNotLoadAudioFileException($"AudioClip is null for text '{texts[i]}'");
                }
            }
            

            return audioClips;

        }

        /// <inheritdoc/>
        public void SetConfig(TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Returns the relative location were the file is cached.
        /// </summary>
        protected virtual string GetPathToFile(string filename)
        {
            string directory = $"{Configuration.StreamingAssetCacheDirectoryName}/{filename}";
            return directory;
        }

        /// <summary>
        /// Retrieves a cached file.
        /// </summary>
        /// <param name="filePath">Relative path where the cached file is stored.</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        protected virtual async Task<byte[]> GetCachedFile(string filePath)
        {
            if (Application.isPlaying)
            {
                return await FileManager.Read(filePath);
            }
            else
            {
                return File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, filePath));
            }
        }

        /// <summary>
        /// Returns true is a file is cached in given relative <paramref name="filePath"/>.
        /// </summary>
        protected virtual async Task<bool> IsFileCached(string filePath)
        {
            if(Application.isPlaying)
            {
                return await FileManager.Exists(filePath);
            }
            else
            {
                return File.Exists(Path.Combine(Application.streamingAssetsPath, filePath));
            }
        }

        public class CouldNotLoadAudioFileException : Exception
        {
            public CouldNotLoadAudioFileException(string msg) : base(msg)
            {
            }

            public CouldNotLoadAudioFileException(string msg, Exception ex) : base(msg, ex)
            {
            }
        }
    }
}
