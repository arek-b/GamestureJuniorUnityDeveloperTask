using System.Collections;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource = null;

        private void Awake()
        {
            TryGetComponent(out audioSource);
        }

        public void PlayClip(AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}