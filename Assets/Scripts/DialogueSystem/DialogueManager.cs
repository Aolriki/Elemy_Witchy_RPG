using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters
{
    public enum CharacterName
    {
        None,
        Beatriny,
        Little_Talker,
        Scared_Boy,
        Academia_Carriage,
        Sign,
    }
}

[System.Serializable]
public struct CharacterDialogueData
{
    public CharacterName characterName;
    public Vector3 position;

    public CharacterDialogueData (CharacterName characterName, Vector3 position)
    {
        this.characterName = characterName;
        this.position = position;
    }
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitInputActions();
    }

    [HideInInspector] public bool isDialogueActive;
    [HideInInspector] public System.Action OnDialogueEnd;
    [HideInInspector] public System.Action OnDialogueStart; // Arthur Aqui

    [Header("Settings")]
    public DialogueBalloon dialogueBalloonPrefab;
    private DialogueBalloon currentDialogueBalloon;
    private SODialogue currentDialogue;
    public int currentSentenceIndex;
    public Sentence currentSentence;
    private List<CharacterDialogueData> currentCharactersData = new();
    private bool choosingResponse;

    [Header("Inputs")]
    public PlayerInputActions playerControls;
    private InputAction nextSentence;

    private void InitInputActions()
    {
        playerControls = new PlayerInputActions();
        nextSentence = playerControls.UI.NextDialogue;
    }

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs();
    }

    public void EnableInputs()
    {
        nextSentence = playerControls.UI.NextDialogue;
        nextSentence.Enable();
    }

    public void DisableInputs()
    {
        nextSentence?.Disable();
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (nextSentence.WasPressedThisFrame())
        {
            //AudioManager.Instance.PlayEffect("GenericButton");
            NextSentence();
        }
    }

    //the first character name must be the Dialogue Character Folder Name
    public void ShowCharacterDialogue(List<CharacterDialogueData> characterDatas)
    {
        //get the SOCharacterProfile from the resources folder
        SOCharacterProfile character =
            Resources.Load<SOCharacterProfile>($"Dialogues/CharacterProfiles/{characterDatas[0].characterName.ToString()}");

        string dialogueName = "";

        dialogueName = character.characterDialogues[character.currentDialogueIndex];
        if (character.currentDialogueIndex < character.characterDialogues.Length - 1)
        {
            character.currentDialogueIndex++;
        }
        else character.currentDialogueIndex = 0;

        //we need to get the character current Dialogue from the character
        //Dialogues folder
        currentDialogue =
            Resources.Load<SODialogue>($"Dialogues/{characterDatas[0].characterName}/{dialogueName}");

        currentSentenceIndex = 0;

        currentCharactersData.Clear();

        currentCharactersData = new List<CharacterDialogueData>(characterDatas);

        SetDialogueUI();
    }

    //the first character name must be the Dialogue Character Folder Name
    public void ShowDialogueByName(string dialogueName, List<CharacterDialogueData> characterDatas)
    {
        //get the SOCharacterProfile from the resources folder
        currentDialogue =
            Resources.Load<SODialogue>($"Dialogues/{characterDatas[0].characterName}/{dialogueName}");

        currentCharactersData.Clear();

        currentCharactersData = new List<CharacterDialogueData>(characterDatas);

        currentSentenceIndex = 0;

        SetDialogueUI();
    }

    private void SetDialogueUI()
    {
        if(!isDialogueActive)
        {
            currentDialogueBalloon = Instantiate(dialogueBalloonPrefab);
            isDialogueActive = true;
            OnDialogueStart?.Invoke(); // Arthur aqui
        }
        

        currentSentence = currentDialogue.sentences[currentSentenceIndex];
        currentSentence.sentenceText = ProcessText(currentSentence.sentenceText);

        Vector3 balloonPosition = new();

        foreach(CharacterDialogueData characterDialogueData in currentCharactersData)
        {
            if(characterDialogueData.characterName == currentSentence.talker)
            {
                balloonPosition = characterDialogueData.position;
                break;
            }
        }

        currentDialogueBalloon.transform.position = balloonPosition;

        SetDialogueInfo();

        currentDialogueBalloon.UpdateText(currentSentence.sentenceText);
    }

    //Normally we use this code to set the characters name, image, etc
    //by searching at the characters list and finding the character info!
    //So we won't be using this code for now
    private void SetDialogueInfo()
    {
        //this other code was used to add customizations, as background
        //and textures to that talker character

        //SetTalkerInfo()
    }


    //this code can process signs in the text and converts it to variables
    private string ProcessText(string text)
    {
        return text;
    }

    public void NextSentence()
    {
        //if the sentence typing is over
        if (currentDialogueBalloon.sentenceTMP.text == currentSentence.sentenceText)
        {
            //if the dialogue still has sentences
            if (currentSentenceIndex < currentDialogue.sentences.Length - 1)
            {
                currentSentenceIndex++;
                SetDialogueUI();
            }
            else //the dialogue doesn't have any sentence left
            {
                //the dialogue doesn't has any response
                if (currentDialogue.responses.Length == 0) 
                    EndDialogue();

                else if (!choosingResponse)
                    GenerateResponses();
            }
        }
        else //makes the sentence completed so the player can skip
        {
            currentDialogueBalloon.FullSentence();
        }
    }

    public void EndDialogue()
    {
        currentSentenceIndex = 0;
        Destroy(currentDialogueBalloon.gameObject);
        isDialogueActive = false;
        OnDialogueEnd?.Invoke();
    }

    public void GenerateResponses()
    {

    }

}
