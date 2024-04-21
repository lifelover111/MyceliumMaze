using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    PlayerManager player;

    private Transform HPBar;
    private Transform SanityBar;
    private Transform ConcentrationBar;
    private Transform SporeCounter;
    private Transform FlaskCounter;
    private Transform ActiveItemCooldownIndicator;
    private Transform ActiveItemCooldownIndicatorFrame;
    private Transform CooldownShadow;
    private Transform InteractButton;

    public PurchaseUI purchaseWindow;


    public Image activeItemIconRenderer;


    GameObject healthLine;
    GameObject sanityLine;
    GameObject concentrationLine;

    Image concentrationLineImg;

    TMP_Text sporeCounterText;
    TMP_Text flaskCounterText;

    Coroutine sporeCountCoroutine;


    public GameObject menuCanvas;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonfire") && Input.GetKeyDown(KeyCode.E))
        {
            ShowMenuCanvas();
        }
    }

    public void ShowMenuCanvas()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
    }
    void Awake()
    {
        OnStart();

        player.OnSporeCountChanged += () => {
            if (sporeCountCoroutine is not null)
            {
                StopCoroutine(sporeCountCoroutine);
            }
            sporeCountCoroutine = StartCoroutine(SporeCountChangeCoroutine());
        };
        player.playerStatsManager.OnFlaskCountChanged += () => { flaskCounterText.text = player.playerStatsManager.healingFlasksCount.ToString(); };
    }

    public void OnStart()
    {
        GetCanvas();

        player = GetComponent<PlayerManager>();
        healthLine = HPBar.GetChild(0).gameObject;
        sanityLine = SanityBar.GetChild(0).gameObject;
        concentrationLine = ConcentrationBar.GetChild(0).gameObject;
        concentrationLineImg = concentrationLine.GetComponent<Image>();
        flaskCounterText = FlaskCounter.GetComponentInChildren<TMP_Text>();
        sporeCounterText = SporeCounter.GetComponentInChildren<TMP_Text>();

        flaskCounterText.text = player.playerStatsManager.healingFlasksCount.ToString();
        sporeCounterText.text = player.sporeCount.ToString();
        healthLine.transform.localScale = new Vector3(player.playerStatsManager.Health / player.playerStatsManager.MaxHealth, 1, 1);
        sanityLine.transform.localScale = new Vector3(player.playerStatsManager.Sanity / player.playerStatsManager.MaxSanity, 1, 1);
        concentrationLine.transform.localScale = new Vector3(player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1, 1);
        concentrationLineImg.color = new Color(0.8f, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration);



        if (player.itemManager.activeItem is not null)
            activeItemIconRenderer.sprite = player.itemManager.activeItem.icon;
    }

    void Update()
    {
        healthLine.transform.localScale = Vector3.Lerp(healthLine.transform.localScale, new Vector3(player.playerStatsManager.Health / player.playerStatsManager.MaxHealth, 1, 1), 10 * Time.deltaTime);
        sanityLine.transform.localScale = Vector3.Lerp(sanityLine.transform.localScale, new Vector3(player.playerStatsManager.Sanity / player.playerStatsManager.MaxSanity, 1, 1), 10 * Time.deltaTime);

        concentrationLine.transform.localScale = Vector3.Lerp(concentrationLine.transform.localScale, new Vector3(player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1, 1), 10 * Time.deltaTime);
        concentrationLineImg.color = new Color(0.8f, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration);

        InteractButton.gameObject.SetActive(player.CanInteract && !PlayerInputManager.instance.uiIsOpen);


        if (player.itemManager.activeItem is not null)
        {
            if (player.itemManager.activeItem.cooldownUnits > 0)
            {
                ActiveItemCooldownIndicator.transform.localScale = new Vector3(ActiveItemCooldownIndicator.transform.localScale.x, (float)((float)player.itemManager.activeItem.currentCooldown) / (float)player.itemManager.activeItem.cooldownUnits, ActiveItemCooldownIndicator.transform.localScale.z);
                ActiveItemCooldownIndicator.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);
                ActiveItemCooldownIndicatorFrame.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);
                CooldownShadow.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);
            }
            else
            {
                ActiveItemCooldownIndicator.gameObject.SetActive(false);
                ActiveItemCooldownIndicatorFrame.gameObject.SetActive(false);
                CooldownShadow.gameObject.SetActive(false);
            }
        }
    }

    
    IEnumerator SporeCountChangeCoroutine()
    {
        int spores = int.Parse(sporeCounterText.text);
        while (spores != player.sporeCount)
        {
            if (spores < player.sporeCount)
            {
                spores += player.sporeCount * 0.99f - spores <= 100 ? 1 : Mathf.RoundToInt((player.sporeCount - spores) * Time.deltaTime);
                sporeCounterText.text = spores.ToString();
            }
            else if (spores > player.sporeCount)
            {
                spores -= spores * 0.99f - player.sporeCount <= 100 ? 1 : Mathf.RoundToInt((spores - player.sporeCount) * Time.deltaTime);
                sporeCounterText.text = spores.ToString();
            }
            yield return null;
        }

        sporeCountCoroutine = null;
    }

    private void GetCanvas()
    {
        HPBar = PlayerCanvas.instance.HPBar;
        SanityBar = PlayerCanvas.instance.SanityBar;
        ConcentrationBar = PlayerCanvas.instance.ConcentrationBar;
        SporeCounter = PlayerCanvas.instance.SporeCounter;
        FlaskCounter = PlayerCanvas.instance.FlaskCounter;
        ActiveItemCooldownIndicator = PlayerCanvas.instance.ActiveItemCooldownIndicator;
        ActiveItemCooldownIndicatorFrame = PlayerCanvas.instance.ActiveItemCooldownIndicatorFrame;
        CooldownShadow = PlayerCanvas.instance.CooldownShadow;
        InteractButton = PlayerCanvas.instance.InteractButton;
        purchaseWindow = PlayerCanvas.instance.purchaseWindow;
        activeItemIconRenderer = PlayerCanvas.instance.activeItemIconRenderer;
        menuCanvas = PlayerCanvas.instance.menuCanvas;
    }

}
