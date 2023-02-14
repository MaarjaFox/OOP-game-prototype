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

    public void ShowAreaButton()
    {
        fireballAttack.gameObject.SetActive(true);
    }

    public void HideAreaButton()
    {
        fireballAttack.gameObject.SetActive(false);
    }
    

    public void DisplayEndText(string loserName)
    {
        battleOverScreen.gameObject.SetActive(true);
        battleOverScreen.text = "Battle over! " + loserName + " lost!";
    }

    private IEnumerator ClearLogOutput()
    {
        yield return new WaitForSeconds(10f);
        logOutput = "";
        textRenderer.text = logOutput;
    }
}
