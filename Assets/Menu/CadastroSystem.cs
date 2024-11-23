using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CadastroSystem : MonoBehaviour
{
   
    public InputField inputEmail;
    public InputField inputUsername;
    public InputField inputPassword;
    public Text feedbackText;
    public Button btnCadastrar;

    //   onde os dados serão armazenados 
    private string path = "cadastros.txt";

    void Start()
    {
        
        if (inputEmail == null || inputUsername == null || inputPassword == null || feedbackText == null || btnCadastrar == null)
        {
            Debug.LogError("Alguma referência não foi atribuída no Inspetor!");
            return; 
        }

        
        btnCadastrar.onClick.AddListener(CadastrarUsuario);
    }

    void CadastrarUsuario()
    {
        // Verifica se todos os campos de InputField estão atribuídos
        if (inputEmail == null || inputUsername == null || inputPassword == null || feedbackText == null)
        {
            feedbackText.text = "Erro: Referência não atribuída corretamente!";
            return;
        }

        string email = inputEmail.text;
        string username = inputUsername.text;
        string password = inputPassword.text;

        // Verifica se os campos não estão vazios
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Preencha todos os campos!";
            return;
        }

       
        if (UsuarioExistente(username))
        {
            feedbackText.text = "Nome de usuário já existe!";
            return;
        }

        //  ID único para o usuário
        string userId = GerarIdUnico();

        // Salva os dados do usuário no arquivo
        SalvarUsuario(userId, email, username, password);

        
        feedbackText.text = "Cadastro realizado com sucesso!";
    }

    bool UsuarioExistente(string username)
    {
        // Verifica se o arquivo existe 
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string[] dados = line.Split(';');
                if (dados.Length > 1 && dados[2] == username)  // Verifica se o nome de usuário já está registrado
                {
                    return true;
                }
            }
        }
        return false;
    }

    void SalvarUsuario(string userId, string email, string username, string password)
    {
        // Cria a string de dados do usuário
        string usuario = userId + ";" + email + ";" + username + ";" + password;

        // Salva os dados no arquivo de cadastro
        File.AppendAllText(path, usuario + "\n");
    }

    string GerarIdUnico()
    {
        int id = 1;
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            id = lines.Length + 1;
        }
        return id.ToString();
    }
}
