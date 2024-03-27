using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    PlayerManager player;

    [SerializeField] Transform HPBar;
    [SerializeField] Transform SanityBar;
    [SerializeField] Transform ConcentrationBar;
    [SerializeField] Transform SporeCounter;
    [SerializeField] Transform FlaskCounter;
    [SerializeField] Transform ActiveItemCooldownIndicator;
    [SerializeField] Transform ActiveItemCooldownIndicatorFrame;
    [SerializeField] Transform CooldownShadow;

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

    // Метод для активации объекта Canvas
    public void ShowMenuCanvas()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
    }
    void Awake()
    {
        player = GetComponent<PlayerManager>();
        healthLine = HPBar.GetChild(0).gameObject;
        sanityLine = SanityBar.GetChild(0).gameObject;
        concentrationLine = ConcentrationBar.GetChild(0).gameObject;
        concentrationLineImg = concentrationLine.GetComponent<Image>();
        flaskCounterText = FlaskCounter.GetComponentInChildren<TMP_Text>();
        /*
        player.OnSporeCountChanged += () => { 
            if(sporeCountCoroutine is not null)
            {
                StopCoroutine(sporeCountCoroutine);
            }
            sporeCountCoroutine = StartCoroutine(SporeCountChangeCoroutine());
        };*/
        player.playerStatsManager.OnFlaskCountChanged += () => { flaskCounterText.text = player.playerStatsManager.healingFlasksCount.ToString(); };
        flaskCounterText.text = player.playerStatsManager.healingFlasksCount.ToString();

    }

    void Update()
    {
        healthLine.transform.localScale = Vector3.Lerp(healthLine.transform.localScale, new Vector3(player.playerStatsManager.Health / player.playerStatsManager.MaxHealth, 1, 1), 10 * Time.deltaTime);
        sanityLine.transform.localScale = Vector3.Lerp(sanityLine.transform.localScale, new Vector3(player.playerStatsManager.Sanity / player.playerStatsManager.MaxSanity, 1, 1), 10 * Time.deltaTime);

        concentrationLine.transform.localScale = Vector3.Lerp(concentrationLine.transform.localScale, new Vector3(player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1, 1), 10 * Time.deltaTime);
        concentrationLineImg.color = new Color(0.8f, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration, 1 - player.playerStatsManager.Concentration / player.playerStatsManager.MaxConcentration);

        ActiveItemCooldownIndicator.transform.localScale = new Vector3(ActiveItemCooldownIndicator.transform.localScale.x, (float)((float)player.itemManager.activeItem.currentCooldown)/(float)player.itemManager.activeItem.cooldownUnits, ActiveItemCooldownIndicator.transform.localScale.z);
        ActiveItemCooldownIndicator.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);
        ActiveItemCooldownIndicatorFrame.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);
        CooldownShadow.gameObject.SetActive(player.itemManager.activeItem.currentCooldown / player.itemManager.activeItem.cooldownUnits != 1);

    }

    /*
    IEnumerator SporeCountChangeCoroutine()
    {
        int spores = int.Parse(sporeCounterText.text);
        while (spores != player.spores)
        {
            if (spores < player.spores)
            {
                spores += player.spores * 0.99f - spores <= 100 ? 1 : Mathf.RoundToInt((player.spores - spores) * Time.deltaTime);
                sporeCounterText.text = spores.ToString();
            }
            else if (spores > player.spores)
            {
                spores -= spores * 0.99f - player.spores <= 100 ? 1 : Mathf.RoundToInt((spores - player.spores) * Time.deltaTime);
                sporeCounterText.text = spores.ToString();
            }
            yield return null;
        }

        sporeCountCoroutine = null;
    }
    */
}
