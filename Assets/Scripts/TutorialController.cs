
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Text")]
    public TMP_Text tutorialTitle;
    public TMP_Text tutorialText;

    [Header("Navigation Buttons")]
    public Button previousButton;
    public Button nextButton;
    public GameObject startRunButton;

    private int currentPage = 0;

    private readonly string[] pageTitles =
    {
        "WELCOME TO THE MILE TRIALS",
        "MOVEMENT",
        "STAMINA",
        "EMERGENCY RECOVERY",
        "GET BACK UP"
    };

    private readonly string[] pageContents =
    {
        "Ester is running three challenging miles.\n\n" +
        "Each mile brings new obstacles and greater challenges.\n\n" +
        "Your goal is to make it to the finish!",

        "Press SPACE to jump over obstacles.\n\n" +
        "Time your jumps carefully. Hitting an obstacle will end your run!",

        "Running and jumping consume stamina.\n\n" +
        "Collect WATER BOTTLES to restore stamina.\n\n" +
        "Keep an eye on your stamina bar!",

        "When your stamina reaches 25% or lower, " +
        "you can use Emergency Recovery.\n\n" +
        "Press E to begin the recovery challenge.\n\n" +
        "You only get one charge per mile, " +
        "but you can earn another through special pickups!",

        "If Ester trips, a Break Free challenge begins.\n\n" +
        "Mash SPACE to get back up before time runs out.\n\n" +
        "Good luck. Make every mile count!"
    };

    private void OnEnable()
    {
        currentPage = 0;
        UpdatePage();
    }

    public void NextPage()
    {
        if (currentPage < pageTitles.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        if (tutorialTitle != null)
            tutorialTitle.text = pageTitles[currentPage];

        if (tutorialText != null)
            tutorialText.text = pageContents[currentPage];

        if (previousButton != null)
            previousButton.gameObject.SetActive(currentPage > 0);

        if (nextButton != null)
            nextButton.gameObject.SetActive(
                currentPage < pageTitles.Length - 1
            );

        if (startRunButton != null)
            startRunButton.SetActive(
                currentPage == pageTitles.Length - 1
            );
    }
}
