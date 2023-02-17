using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text textRenderer;
    [SerializeField] private TMP_Text gameState;
    [SerializeField] private TMP_Text hoveredHex;
    [SerializeField] private TMP_Text battleOverScreen;
    [SerializeField] private Button rangedAttack;
    [SerializeField] private Button fireballAttack;
    [SerializeField] private Button poisonAttack;
    [SerializeField] private Button riskAttack;
    [SerializeField] private Button tankAttack;
    [SerializeField] private Button mageAction;
    [SerializeField] private Button mageHeal;


    private string logOutput = "";

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UIManager>();
            }

            return _instance;
        }

    }

    void Start()
    {
        HideRangedAttackButton();
        HideAreaButton();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddLogText(string text)
    {
        logOutput += "\n" + text;
        textRenderer.text = logOutput;
        StartCoroutine(ClearLogOutput());
    }

    public void SetHexText(string text)
    {
        hoveredHex.text = "Hex: " + text;
    }
    public void SetStateText(string text)
    {
        gameState.text = "State: " + text;
    }

    public void ShowRangedAttackButton()
    {
        rangedAttack.gameObject.SetActive(true);
    }

    public void HideRangedAttackButton()
    {
        rangedAttack.gameObject.SetActive(false);
    }
    public void ShowPoisonButton()
    {
        poisonAttack.gameObject.SetActive(true);
    }

    public void HidePoisonButton()
    {
        poisonAttack.gameObject.SetActive(false);
    }
    public void ShowAreaButton()
    {
        fireballAttack.gameObject.SetActive(true);
    }

    public void HideAreaButton()
    {
        fireballAttack.gameObject.SetActive(false);
    }

    public void ShowRiskButton()
    {
        riskAttack.gameObject.SetActive(true);
    }

    public void HideRiskButton()
    {
       riskAttack.gameObject.SetActive(false);

    }
    public void ShowTankButton()
    {
        tankAttack.gameObject.SetActive(true);
    }

    public void HideTankButton()
    {
        tankAttack.gameObject.SetActive(false);
    }

    public void ShowMageButton()
    {
        mageAction.gameObject.SetActive(true);
    }

    public void HideMageButton()
    {
        mageAction.gameObject.SetActive(false);
    }
    public void ShowHealButton()
    {
        mageHeal.gameObject.SetActive(true);
    }

    public void HideHealButton()
    {
        mageHeal.gameObject.SetActive(false);
    }
    public void DisplayEndText(string loserName)
    {
        battleOverScreen.gameObject.SetActive(true);
        battleOverScreen.text = "Battle over! " + loserName + " lost!";
    }

    private IEnumerator ClearLogOutput()
    {
        yield return new WaitForSeconds(20f);
        logOutput = "";
        textRenderer.text = logOutput;
    }
}
