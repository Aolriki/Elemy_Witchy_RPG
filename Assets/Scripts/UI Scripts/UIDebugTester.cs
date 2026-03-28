using UnityEngine;

public class UIDebugTester : MonoBehaviour
{
    public DialogueBalloon balloon;

    public HealthBar healthBar;

    #region Hard Coded Test
    private string GetRandomSentence()
    {
        string newSentence = "newSentence";

        int randomIndex = 0;
        randomIndex = Random.Range(0, 5);

        switch (randomIndex)
        {
            case 0:
                newSentence = "Eu sou lindo! Apesar de narigudo, eu ainda me acho muito bonito! Eu não sei porque as mina não faz fila, na verdade!";
                break;
            case 1:
                newSentence = "Tenho Frases motivacionais, mas... Não sei se elas funcionam pra você tanto assim. Pra mim elas com certeza funcionam. Porque eu sou o bonzão mesmo!";
                break;
            case 2:
                newSentence = "Eu motivo a mim mesmo! Eu não preciso de você! Eu sou melhor que você!";
                break;
            case 3:
                newSentence = "Eu sou tipo um esquema de pirâmide! Imprimindo felicidade artificial infinita!";
                break;
            case 4:
                newSentence = "Achoo! *sniff*";
                break;
        }

        return newSentence;
    }
    private int GetRandomInt()
    {
        return Random.Range(3, 10);
    }
    private int GetRandomHealthInt()
    {
        return Random.Range(1, healthBar._maxHealth);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Y Pressed");
            int integer1 = GetRandomInt();
            healthBar.UpdateMaxHealth(integer1);
            int integer2 = GetRandomHealthInt();
            healthBar.UpdateHealth(integer2, integer1);
            Debug.Log(integer1 + " and " + integer2);


        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T Pressed");
        }

    }
}
