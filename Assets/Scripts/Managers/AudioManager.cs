using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinmumDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBgm;
    private int bgmIndex;

    private bool canPlaySFX;
    private Coroutine[] fadeCoroutines;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        fadeCoroutines = new Coroutine[sfx.Length];
        Invoke("AllowSFX", 1f);
    }

    private void Update()
    {
        if(!playBgm)
            stopAllBGM();
        else
        {
            if (bgmIndex < bgm.Length && bgm[bgmIndex] != null && !bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        if (canPlaySFX == false)
            return;

        if (_sfxIndex < 0 || _sfxIndex >= sfx.Length)
            return;

        if (sfx[_sfxIndex] == null)
            return;

        if (fadeCoroutines[_sfxIndex] != null)
        {
            StopCoroutine(fadeCoroutines[_sfxIndex]);
            fadeCoroutines[_sfxIndex] = null;
        }

        sfx[_sfxIndex].volume = 1f;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinmumDistance)
            return;

        sfx[_sfxIndex].pitch = Random.Range(0.85f, 1f);
        sfx[_sfxIndex].Play();
    }

    public void stopSFX(int _indx)
    {
        if (_indx < 0 || _indx >= sfx.Length)
            return;

        if (sfx[_indx] != null)
            sfx[_indx].Stop();
    }

    public void StopFXWithTime(int _indx)
    {
        if (_indx < 0 || _indx >= sfx.Length)
            return;

        if (sfx[_indx] == null)
            return;

        fadeCoroutines[_indx] = StartCoroutine(DecreaseVolume(sfx[_indx], _indx));
    }

    private IEnumerator DecreaseVolume(AudioSource _audio, int _index)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > 0.1f)
        {
            _audio.volume -= _audio.volume * 0.2f;
            yield return new WaitForSeconds(0.25f);

            if (_audio.volume <= 0.1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }

        fadeCoroutines[_index] = null;
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int _bgmIndex)
    {
        if (_bgmIndex < 0 || _bgmIndex >= bgm.Length)
            return;

        bgmIndex = _bgmIndex;

        stopAllBGM();
        if (bgm[bgmIndex] != null)
            bgm[bgmIndex].Play();
    }

    public void stopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null)
                bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;
}
