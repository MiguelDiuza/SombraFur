using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        // Opcional: configura el AudioSource por defecto
        _audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Reproduce un clip con PlayOneShot.
    /// </summary>
    /// <param name="clip">AudioClip a reproducir.</param>
    public void PlayClip(AudioClip clip)
    {
        if (clip != null)
            _audioSource.PlayOneShot(clip);
    }
}
