using System.Collections;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    public TextMeshProUGUI[] textComponent;
    public float letterDelay = 0.05f;

    public DoorOpen missionDoor;

    [TextArea]
    public string fullText;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        missionDoor.isLocked = true;
        foreach (var t in textComponent)
        {
            t.gameObject.SetActive(false);
        }

        ShowCurrentText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                // Text sofort vollständig anzeigen
                StopCoroutine(typingCoroutine);
                textComponent[currentIndex].text = fullText;
                isTyping = false;
            }
            else
            {
                // nächstes Textelement
                currentIndex++;

                if (currentIndex < textComponent.Length)
                {
                    textComponent[currentIndex -1].gameObject.SetActive(false);
                    ShowCurrentText();
                }
                else
                {
                    missionDoor.isLocked = false;
                }
            }
        }
    }

    void ShowCurrentText()
    {
        textComponent[currentIndex].gameObject.SetActive(true);
        fullText = textComponent[currentIndex].text;
        typingCoroutine = StartCoroutine(TypeText(textComponent[currentIndex]));
    }

    IEnumerator TypeText(TextMeshProUGUI targetText)
    {
        isTyping = true;
        targetText.text = "";

        foreach (char letter in fullText)
        {
            targetText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;
    }
}