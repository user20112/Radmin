using UnityEngine;

public class RadminAudioHandler : MonoBehaviour
{
    private Radmin radmin;

    public AudioSource generalSource;
    public AudioSource carrySource;
    public AudioSource throwSource;

    [Header("Sounds")]
    public AudioClip throwSound;
    public AudioClip noticeSound;
    public AudioClip grabSound;

    private void Awake()
    {
        radmin = GetComponent<Radmin>();
        radmin.OnStartFollow.AddListener((x) => OnStartFollow(x));
        radmin.OnStartThrow.AddListener((x) => OnStartThrow(x));
        radmin.OnEndThrow.AddListener((x) => OnEndThrow(x));

        radmin.OnStartCarry.AddListener((x) => OnStartCarry(x));
        radmin.OnEndCarry.AddListener((x) => OnEndCarry(x));
    }

    public void OnStartFollow(int num)
    {
        carrySource.Stop();
        generalSource.PlayOneShot(noticeSound);
    }

    public void OnStartThrow(int num)
    {
        generalSource.PlayOneShot(throwSound);
        throwSource.Play();
    }

    public void OnEndThrow(int num)
    {
        if(radmin.Objective != null)
            generalSource.PlayOneShot(grabSound);
    }

    public void OnStartCarry(int num)
    {
        carrySource.Play();
    }

    public void OnEndCarry(int num)
    {
        carrySource.Stop();
    }
}
