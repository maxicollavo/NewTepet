using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask mirrorMask;
    public LayerMask winnerMask;
    public LineRenderer line;
    public float maxDistance = 128;
    public int maxBounces = 128;

    [SerializeField] private float surfaceOffset = 0.01f;

    [Header("State")]
    readonly List<Vector3> _linePoints = new();
    public bool hasWon { get; private set; }
    private int mirrorCounter;

    // sonido gemas
    // Cuando el rayo empieza a tocar una gema suena un destello, y mientras la toca suena un aura en loop.
    readonly HashSet<GameObject> _gemasTocadas = new();
    readonly HashSet<GameObject> _gemasAntes = new();
    readonly Dictionary<GameObject, AudioSource> _auras = new();
    readonly Dictionary<GameObject, float> _ultimoDestello = new();
    AudioClip _clipDestello, _clipAura;
    float _inicio;

    private void Start()
    {
        _clipDestello = Resources.Load<AudioClip>("Sounds/Niveles/Destello gema");
        _clipAura = Resources.Load<AudioClip>("Sounds/Niveles/AuraGema");
        _inicio = Time.time;
    }

    private void Update()
    {
        Physics.SyncTransforms();
        _gemasTocadas.Clear();

        mirrorCounter = 0;

        _linePoints.Clear();
        _linePoints.Add(transform.position);

        ShootLaser(transform.position, transform.forward, maxBounces);

        line.positionCount = _linePoints.Count;
        line.SetPositions(_linePoints.ToArray());

        ActualizarSonidoGemas();
    }

    void ActualizarSonidoGemas()
    {
        bool cargando = Time.time - _inicio < 1f; // al cargar el nivel no suena el destello
        foreach (var gema in _gemasTocadas)
        {
            var aura = AuraDe(gema);
            if (aura == null) continue;
            if (!_gemasAntes.Contains(gema))
            {
                if (!aura.isPlaying) { aura.time = Random.Range(0f, aura.clip.length * 0.9f); aura.Play(); }
                _ultimoDestello.TryGetValue(gema, out float t);
                if (!cargando && _clipDestello != null && Time.time - t > 0.35f)
                {
                    aura.PlayOneShot(_clipDestello, 0.8f);
                    _ultimoDestello[gema] = Time.time;
                }
            }
        }
        foreach (var gema in _gemasAntes)
            if (!_gemasTocadas.Contains(gema) && _auras.TryGetValue(gema, out var a) && a != null) a.Stop();
        _gemasAntes.Clear();
        _gemasAntes.UnionWith(_gemasTocadas);
    }

    AudioSource AuraDe(GameObject gema)
    {
        if (_clipAura == null || gema == null) return null;
        if (_auras.TryGetValue(gema, out var a) && a != null) return a;
        a = gema.AddComponent<AudioSource>();
        a.clip = _clipAura; a.loop = true; a.playOnAwake = false; a.volume = 0.25f;
        a.spatialBlend = 1f; a.rolloffMode = AudioRolloffMode.Linear; a.minDistance = 2f; a.maxDistance = 18f; a.dopplerLevel = 0f;
        _auras[gema] = a;
        return a;
    }

    void ShootLaser(Vector3 pos, Vector3 dir, int bounceLimit)
    {
        if (bounceLimit <= 0)
            return;

        dir = dir.normalized;

        int hitMask = mirrorMask.value | winnerMask.value;

        if (!Physics.Raycast(pos, dir, out RaycastHit hit, maxDistance, hitMask))
        {
            _linePoints.Add(pos + dir * maxDistance);
            return;
        }

        _linePoints.Add(hit.point);

        GameObject target = hit.collider.gameObject;

        if (IsObjectMirror(target))
        {
            mirrorCounter++;
            _gemasTocadas.Add(target);

            Vector3 reflectedDir = Vector3.Reflect(dir, hit.normal).normalized;
            Vector3 newStartPos = hit.point + reflectedDir * surfaceOffset;

            ShootLaser(newStartPos, reflectedDir, bounceLimit - 1);

            return;
        }

        if (IsObjectTargetWinner(target))
        {
            if (mirrorCounter == 3 && !hasWon)
            {
                hasWon = true;
            }

            return;
        }
    }

    bool IsObjectMirror(GameObject target)
    {
        return (mirrorMask.value & (1 << target.layer)) != 0;
    }

    bool IsObjectTargetWinner(GameObject target)
    {
        return (winnerMask.value & (1 << target.layer)) != 0;
    }
}