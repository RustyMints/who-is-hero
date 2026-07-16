using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private GameObject flashPrefab;

    private Entity entity;
    private CharacterStarts myStats;
    private RectTransform myTransform;
    private Slider slider;
    private CanvasGroup canvasGroup;
    private Image fillImage;

    private float lastHealth;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<CharacterStarts>();
        canvasGroup = GetComponent<CanvasGroup>();
        fillImage = slider.fillRect.GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        lastHealth = myStats.GetMaxHealthValue();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        float currentHealth = myStats.currentHealth;
        float maxHealth = myStats.GetMaxHealthValue();

        float oldRatio = lastHealth / maxHealth;
        float newRatio = currentHealth / maxHealth;

        slider.maxValue = maxHealth;
        slider.value = currentHealth;

        if (currentHealth < lastHealth)
        {
            SpawnChip(oldRatio, newRatio);
            SpawnFlash(newRatio);
        }

        lastHealth = currentHealth;
    }

    private void SpawnChip(float oldRatio, float newRatio)
    {
        if (chipPrefab == null) return;
        if (slider == null) return;
        if (slider.fillRect == null) return;

        GameObject chip = Instantiate(chipPrefab);
        Image img = chip.GetComponent<Image>();
        if (img == null) img = chip.AddComponent<Image>();

        CanvasGroup cg = chip.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = chip.AddComponent<CanvasGroup>();

        Canvas chipCanvas = chip.GetComponent<Canvas>();
        if (chipCanvas == null)
            chipCanvas = chip.AddComponent<Canvas>();
        chipCanvas.overrideSorting = true;
        chipCanvas.sortingOrder = 10;

        RectTransform chipRT = img.rectTransform;
        RectTransform fillRT = slider.fillRect;
        RectTransform fillAreaRT = fillRT.parent as RectTransform;
        if (fillAreaRT == null) return;

        float fullWidth = fillAreaRT.rect.width;

        chip.transform.SetParent(fillAreaRT);
        chip.transform.localScale = Vector3.one;

        float chipWidth = fullWidth * (oldRatio - newRatio);
        float chipHeight = fillRT.rect.height;

        chipRT.anchorMin = new Vector2(0, 0.5f);
        chipRT.anchorMax = new Vector2(0, 0.5f);
        chipRT.pivot = new Vector2(0, 0.5f);
        chipRT.sizeDelta = new Vector2(chipWidth, chipHeight);
        chipRT.anchoredPosition = new Vector2(fillRT.anchoredPosition.x + fullWidth * newRatio, fillRT.anchoredPosition.y);

        cg.alpha = 1;

        StartCoroutine(ChipAnimation(chip, cg));
    }

    private IEnumerator ChipAnimation(GameObject chip, CanvasGroup cg)
    {
        RectTransform rt = chip.GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        float duration = 0.8f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            float easeProgress = Mathf.Pow(progress, 3);
            rt.anchoredPosition = new Vector2(startPos.x, startPos.y - 180f * easeProgress);

            float fadeProgress = Mathf.Pow(progress, 2);
            cg.alpha = 1 - fadeProgress;

            yield return null;
        }

        Destroy(chip);
    }

    private void SpawnFlash(float newRatio)
    {
        if (flashPrefab == null) return;
        if (slider == null) return;
        if (slider.fillRect == null) return;

        GameObject flash = Instantiate(flashPrefab);
        CanvasGroup fg = flash.GetComponent<CanvasGroup>();
        if (fg == null)
            fg = flash.AddComponent<CanvasGroup>();

        Canvas flashCanvas = flash.GetComponent<Canvas>();
        if (flashCanvas == null)
            flashCanvas = flash.AddComponent<Canvas>();
        flashCanvas.overrideSorting = true;
        flashCanvas.sortingOrder = 100;

        RectTransform flashRT = flash.GetComponent<RectTransform>();
        if (flashRT == null) return;

        RectTransform fillRT = slider.fillRect;
        RectTransform fillAreaRT = fillRT.parent as RectTransform;
        if (fillAreaRT == null) return;

        float fullWidth = fillAreaRT.rect.width;

        flash.transform.SetParent(fillAreaRT);
        flash.transform.localScale = Vector3.one;

        float flashWidth = 50f;
        float flashHeight = fillRT.rect.height * 3f;

        flashRT.anchorMin = new Vector2(0, 0.5f);
        flashRT.anchorMax = new Vector2(0, 0.5f);
        flashRT.pivot = new Vector2(0, 0.5f);
        flashRT.sizeDelta = new Vector2(flashWidth, flashHeight);

        float currentHealth = slider.value;
        float maxHealth = slider.maxValue;
        float ratio = currentHealth / maxHealth;
        float barWidth = fillAreaRT.rect.width;
        float targetX = barWidth * ratio - flashWidth / 2f;
        flashRT.anchoredPosition = new Vector2(targetX, fillRT.anchoredPosition.y);

        fg.alpha = 1;

        StartCoroutine(FlashAnimation(flash, fg));
    }

    private IEnumerator FlashAnimation(GameObject flash, CanvasGroup fg)
    {
        float duration = 0.3f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            float fadeProgress = Mathf.Pow(progress, 2);
            fg.alpha = 1 - fadeProgress;

            yield return null;
        }

        Destroy(flash);
    }

    private void FlipUI() => myTransform.Rotate(0, 180, 0);

    public void StartFadeOut(float duration = 0.5f)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float timer = 0;
        float startAlpha = canvasGroup.alpha;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}