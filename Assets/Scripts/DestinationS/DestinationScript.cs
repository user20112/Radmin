using DG.Tweening;
using System;
using UnityEngine;

public class DestinationScript : MonoBehaviour
{
    private new Renderer renderer;
    private new AudioSource audio;

    [ColorUsage(true, true)]
    public Color originalColor;

    public Color captureColor;

    public RadminDestinationEnum DestinationType;

    public Vector3 capturePointOffset;

    [Space]
    [Header("Particle Systems")]
    public ParticleSystem captureParticle;

    public ParticleSystem storeParticle;
    public ParticleSystem smokeParticle;
    public ParticleSystem capsuleParticle;
    public EventHandler<CollectedEventArgs> ObjectCollected;

    [Space]
    [Header("Sounds")]
    public AudioClip suckSound;

    public AudioClip collectSound;
    public bool active = true;

    public virtual void Start()
    {
        audio = GetComponent<AudioSource>();
        renderer = GetComponent<Renderer>();
    }

    public static DestinationScript GetDestinationFromType(RadminDestinationEnum type)
    {
        DestinationScript[] Destinations = FindObjectsOfType(typeof(DestinationScript)) as DestinationScript[];
        foreach (DestinationScript Destination in Destinations)
            if (Destination.DestinationType == type)
                return Destination;
        return null;
    }

    public static DestinationScript GetPrefabFromDestinationType(RadminDestinationEnum type)
    {
        DestinationScript[] Destinations = FindObjectsOfType(typeof(DestinationScript)) as DestinationScript[];
        foreach (DestinationScript Destination in Destinations)
            if (Destination.DestinationType == type)
                return Destination;
        return null;
    }

    public Vector3 Point()
    {
        return transform.position + capturePointOffset;
    }

    public void StartCapture()
    {
        if (!active)
            return;
        audio.pitch = 1.5f;
        audio.PlayOneShot(suckSound);
        captureParticle.Play();
    }

    public void FinishCapture()
    {
        if (!active)
            return;
        audio.pitch = 1;
        audio.PlayOneShot(collectSound);
        storeParticle.Play();
        smokeParticle.Play();
        capsuleParticle.Play();
        renderer.material.DOColor(captureColor, "_EmissionColor", .2f).OnComplete(() => renderer.material.DOColor(originalColor, "_EmissionColor", .5f));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Point(), .2f);
    }

    public virtual void ItemCollected(CollectedEventArgs Args)
    {
        float time = 1.3f;
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => StartCapture());
        s.Append(Args.ObjectCollected.ObjectRenderer.material.DOColor(captureColor, "_EmissionColor", time));
        s.Join(Args.ObjectCollected.transform.DOMove(transform.position, time).SetEase(Ease.InQuint));
        s.Join(Args.ObjectCollected.transform.DOScale(0, time).SetEase(Ease.InQuint));
        s.AppendCallback(() => FinishCapture());
        Destroy(Args.ObjectCollected);
    }
}