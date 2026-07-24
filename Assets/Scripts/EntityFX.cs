using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class EntityFX : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    [Header("Screen shake FX")]
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHigeDamage;

    [Header("After image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem ChillFX;
    [SerializeField] private ParticleSystem ShockFX;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitfx;
    [SerializeField] private GameObject criticalHitFX;

    [Header("Bleed Particle")]
    [SerializeField] private GameObject bleedParticlePrefab;
    [SerializeField] private float bleedParticleInterval = 0.15f;

    [Space]
    [SerializeField] private ParticleSystem dustFX;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        player = PlayerManager.instance.player;
        screenShake = GetComponent<CinemachineImpulseSource>();
        originalMat = sr.material;

    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir,_shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;

            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, sr.sprite);
        }
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFx.Stop();
        ChillFX.Stop();
        ShockFX.Stop();
    }

    public void IgniteFxFor(float _seconds)
    {
        igniteFx.Play();

        InvokeRepeating("IgniteColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        ChillFX.Play();

        InvokeRepeating("ChillColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

   

    public void ShockFxFor(float _seconds)
    {
        ShockFX.Play();

        InvokeRepeating("ShockColorFx", 0, 0.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void BleedFxFor(float _seconds)
    {
        if (bleedParticlePrefab != null)
        {
            InvokeRepeating("SpawnBleedParticle", 0, bleedParticleInterval);
            Invoke("CancelBleedParticle", _seconds);
        }
    }

    private void SpawnBleedParticle()
    {
        if (bleedParticlePrefab == null)
            return;

        Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0);
        GameObject particle = Instantiate(bleedParticlePrefab, transform.position + spawnOffset, Quaternion.identity);
        Destroy(particle, 1f);
    }

    private void CancelBleedParticle()
    {
        CancelInvoke("SpawnBleedParticle");
    }


    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }


    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }


    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    
    public void CreateHitFX(Transform _target,bool _cirtical)
    {

        float zRotation = Random.Range(-90, 90);
        float xPotation = Random.Range(-0.5f, 0.5f);
        float yPotation = Random.Range(-0.5f, 0.5f);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);

        GameObject hitPrefab = hitfx;

        if (_cirtical)
        {
            hitPrefab = criticalHitFX;

            float yRoation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
                yRoation = 100;

            hitFxRotation = new Vector3(0,yRoation,zRotation);
        }

        GameObject newHitFX = Instantiate(hitPrefab, _target.position + new Vector3(xPotation, yPotation), Quaternion.identity);// _target); 打击特效追随目标

        newHitFX.transform.Rotate(hitFxRotation);

        Destroy(newHitFX, 0.5f);
        
    }

    public void PlayerDustFX()
    {
        if(dustFX != null)
            dustFX.Play();
        
        
    }
}
