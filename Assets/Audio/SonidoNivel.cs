using UnityEngine;

// suena solo cuando el objeto se activa (loop de ambiente,
// fuego, cascada) o cuando el juego prende el objeto al resolver algo (ascensor, puertas).
// No toca ningún script del juego.
[RequireComponent(typeof(AudioSource))]
public class SonidoNivel : MonoBehaviour
{
    [Tooltip("En loops, arranca en un punto al azar para que varias copias no suenen iguales")]
    public bool inicioAleatorio = true;

    AudioSource src;

    void OnEnable()
    {
        if (src == null) src = GetComponent<AudioSource>();
        if (src.clip == null) return;
        if (src.loop && inicioAleatorio) src.time = Random.Range(0f, src.clip.length * 0.95f);
        src.Play();
    }
}
