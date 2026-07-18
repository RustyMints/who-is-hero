using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject chipPrefab;
    [SerializeField] private GameObject flashPrefab;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    [SerializeField] private TextMeshProUGUI currentSouls;
    private SkillManager skills;

    private float lastHealth;
    private GameObject currentFlash;

    void Start()
    {
        if(playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;

        skills = SkillManager.instance;

        lastHealth = playerStats.GetMaxHealthValue();
    }

    private void UpdateHealthUI()
    {
        float currentHealth = playerStats.currentHealth;
        float maxHealth = playerStats.GetMaxHealthValue();

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

        CoroutineRunner runner = chip.GetComponent<CoroutineRunner>();
        if (runner == null)
            runner = chip.AddComponent<CoroutineRunner>();
        runner.RunCoroutine(ChipAnimation(chip, cg));
    }

    private System.Collections.IEnumerator ChipAnimation(GameObject chip, CanvasGroup cg)
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

        RectTransform fillRT = slider.fillRect;
        RectTransform fillAreaRT = fillRT.parent as RectTransform;
        if (fillAreaRT == null) return;

        float flashWidth = 30f;
        float barWidth = fillAreaRT.rect.width;
        float targetX = barWidth * newRatio - flashWidth / 2f;

        if (currentFlash != null)
        {
            RectTransform existingFlashRT = currentFlash.GetComponent<RectTransform>();
            if (existingFlashRT != null)
            {
                existingFlashRT.anchoredPosition = new Vector2(targetX, fillRT.anchoredPosition.y);
            }
            CanvasGroup existingFg = currentFlash.GetComponent<CanvasGroup>();
            if (existingFg != null)
            {
                existingFg.alpha = 1;
            }
            return;
        }

        GameObject flash = Instantiate(flashPrefab);
        CanvasGroup fg = flash.GetComponent<CanvasGroup>();
        if (fg == null)
            fg = flash.AddComponent<CanvasGroup>();

        Canvas flashCanvas = flash.GetComponent<Canvas>();
        if (flashCanvas == null)
            flashCanvas = flash.AddComponent<Canvas>();
        flashCanvas.overrideSorting = true;
        flashCanvas.sortingOrder = 10;

        RectTransform flashRT = flash.GetComponent<RectTransform>();
        if (flashRT == null) return;

        float fullWidth = fillAreaRT.rect.width;

        flash.transform.SetParent(fillAreaRT);
        flash.transform.localScale = Vector3.one;

        float flashHeight = fillRT.rect.height * 2.5f;

        flashRT.anchorMin = new Vector2(0, 0.5f);
        flashRT.anchorMax = new Vector2(0, 0.5f);
        flashRT.pivot = new Vector2(0, 0.5f);
        flashRT.sizeDelta = new Vector2(flashWidth, flashHeight);
        flashRT.anchoredPosition = new Vector2(targetX, fillRT.anchoredPosition.y);

        fg.alpha = 1;

        currentFlash = flash;

        CoroutineRunner runner = flash.GetComponent<CoroutineRunner>();
        if (runner == null)
            runner = flash.AddComponent<CoroutineRunner>();
        runner.RunCoroutine(FlashAnimation(flash, fg));
    }

    private System.Collections.IEnumerator FlashAnimation(GameObject flash, CanvasGroup fg)
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

        if (currentFlash == flash)
            currentFlash = null;

        Destroy(flash);
    }

    void Update()
    {
        currentSouls.text = PlayerManager.instance.GetCurrency().ToString("#,#");

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            SetCooldownOf(dashImage);

        if(Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);

        if(Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlock)
            SetCooldownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackholeUnlocked)
            SetCooldownOf(blackholeImage);

        if(Input.GetKeyDown (KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image,float _cooldown)
    {
        if(_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }

}