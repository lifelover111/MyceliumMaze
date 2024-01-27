using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerUIController : MonoBehaviour
{
    Player player;

    [SerializeField] Transform HPBar;
    [SerializeField] Transform SanityBar;
    [SerializeField] Transform ConcentrationBar;
    [SerializeField] Transform SporeCounter;
    [SerializeField] Transform FlaskCounter;


    GameObject healthLine;
    GameObject sanityLine;
    GameObject concentrationLine;

    Image concentrationLineImg;

    TMP_Text sporeCounterText;
    TMP_Text flaskCounterText;

    Coroutine sporeCountCoroutine;

    void Awake()
    {
        player = GetComponent<Player>();
        healthLine = HPBar.GetChild(0).gameObject;
        sanityLine = SanityBar.GetChild(0).gameObject;
        concentrationLine = ConcentrationBar.GetChild(0).gameObject;
        concentrationLineImg = concentrationLine.GetComponent<Image>();
        sporeCounterText = SporeCounter.GetComponentInChildren<TMP_Text>();
        flaskCounterText = FlaskCounter.GetComponentInChildren<TMP_Text>();

        player.OnSporeCountChanged += () => { 
            if(sporeCountCoroutine is not null)
            {
                StopCoroutine(sporeCountCoroutine);
            }
            sporeCountCoroutine = StartCoroutine(SporeCountChangeCoroutine());
        };
        player.OnFlaskCountChanged += () => { flaskCounterText.text = player.numFlasks.ToString(); };
    }

    void Update()
    {
        healthLine.transform.localScale = Vector3.Lerp(healthLine.transform.localScale, new Vector3(player.health / player.maxHealth, 1, 1), 10 * Time.deltaTime);
        sanityLine.transform.localScale = Vector3.Lerp(sanityLine.transform.localScale, new Vector3(player.sanity / player.maxSanity, 1, 1), 10 * Time.deltaTime);

        concentrationLine.transform.localScale = Vector3.Lerp(concentrationLine.transform.localScale, new Vector3(player.GetConcentration() / player.GetMaxConcentration(), 1, 1), 10 * Time.deltaTime);
        concentrationLineImg.color = new Color(0.8f, 1 - player.GetConcentration() / player.GetMaxConcentration(), 1 - player.GetConcentration() / player.GetMaxConcentration());
    }


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
}
