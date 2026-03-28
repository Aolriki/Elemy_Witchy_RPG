using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueCharacter : Interactable
{
    public CharacterName characterName;

    [SerializeField] private string specificDialogueName;

    [SerializeField] private bool shouldRepeatMonologues = true;
    [SerializeField] private List<string> monologueSentences;

    [SerializeField] private Transform graphic;

    [SerializeField] private Transform dialogueBalloonPosition;

    [SerializeField] private DialogueBalloon monologueBalloonPrefab;
    private DialogueBalloon currentMonologueBalloon;
    private int currentMonologueSentenceIndex = 0;
    private Coroutine monologueCoroutine;
    private bool monologueEnded = false;

    protected bool dialogueActive = false;
    [SerializeField] private bool isAlive = true;

    public UnityEvent OnEndDialogue;
    protected void Update()
    {
        if (Player.instance == null || !canInteract) return;

        Vector3 playerPos = Player.instance.transform.position;
        Vector3 myPos = transform.position;

        // Se o player está à direita
        if (isAlive == false)
        {
            return;
        }    
        if (playerPos.x > myPos.x)
        {
            graphic.localScale = Vector3.one; // Olha pra direita
        }
        else
        {
            graphic.localScale = new Vector3(-1f, 1f, 1f); // Olha pra esquerda
        }
    }

    protected virtual void EndDialogue()
    {
        if (dialogueActive)
            dialogueActive = false;
        DialogueManager.instance.OnDialogueEnd -= EndDialogue;
        PlayerController.instance.Possess();

        canInteract = true;
        OnCanInteract();

        if (!monologueEnded && monologueSentences.Count > 0)
              Monologue();

        OnEndDialogue?.Invoke();
    }

    public override void Interact()
    {

        if (dialogueActive) return;

        if (currentMonologueBalloon != null)
        {
            EndMonologue();
        }

        canInteract = false;
        OnCantInteract();

        DialogueManager.instance.OnDialogueEnd += EndDialogue;

        CharacterDialogueData NPC = new (characterName, dialogueBalloonPosition.position);
        CharacterDialogueData player = new (CharacterName.Beatriny, Player.instance.playerDialoguePosition.position);

        List<CharacterDialogueData> charactersDataList = new();
        charactersDataList.Add(NPC);
        charactersDataList.Add(player);

        if (specificDialogueName != null)
        {
            DialogueManager.instance.ShowDialogueByName(specificDialogueName, charactersDataList);
        }
        else //show character dialogues
        {
            DialogueManager.instance.ShowCharacterDialogue(charactersDataList);
        }

        PlayerController.instance.Unpossess();
            
        dialogueActive = true;
    }

    public override void OnCanInteract()
    {
        if(characterName != CharacterName.None)
        {
            base.OnCanInteract();
        }

        //there's monologue sentences, so we need to show it
        if(!monologueEnded && monologueSentences.Count > 0)
        {
            Monologue();
        }
       
    }

    public override void OnCantInteract()
    {
        base.OnCantInteract();

        if(currentMonologueBalloon != null)
        {
            EndMonologue();
        }
    }

    private void Monologue()
    {
        if (currentMonologueBalloon == null)
        {
            currentMonologueBalloon = Instantiate(monologueBalloonPrefab, dialogueBalloonPosition.position, Quaternion.identity,  transform);

            currentMonologueBalloon.OnTypingOverEvent.AddListener(OnMonologueSentenceOver);
        }

        currentMonologueBalloon.UpdateText(monologueSentences[currentMonologueSentenceIndex]);
    }

    private void OnMonologueSentenceOver()
    {
        monologueCoroutine = StartCoroutine(OnMonologueSentenceOverCoroutine());
    }

    private IEnumerator OnMonologueSentenceOverCoroutine()
    {
        yield return new WaitForSeconds(1);

        if (currentMonologueSentenceIndex < monologueSentences.Count - 1)
        {
            currentMonologueSentenceIndex++;
            Monologue();
        }
        else
        {
            if (shouldRepeatMonologues)
            {
                currentMonologueSentenceIndex = 0;
                Monologue();
            }
            else
            {
                monologueEnded = true;
                EndMonologue();
            }
        }
    }

    private void EndMonologue()
    {
        Debug.Log("EndMonologue");

        currentMonologueBalloon.OnTypingOverEvent.RemoveAllListeners();

        if (monologueCoroutine != null)
        {
            StopCoroutine(monologueCoroutine);
            monologueCoroutine = null;
        }

        Destroy(currentMonologueBalloon.gameObject);
        currentMonologueBalloon = null;
    }
}
