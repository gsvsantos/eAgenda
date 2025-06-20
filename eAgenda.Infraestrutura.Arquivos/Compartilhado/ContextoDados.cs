using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloDespesa;

namespace eAgenda.Infraestrutura.Arquivos.Compartilhado;

public class ContextoDados
{
    // Espaço para implementar as Listas (Ex: public List<Contato> Contatos { get; set; })
    private string pastaArmazenamento = string.Empty;
    private string arquivoArmazenamento = "dados-eAgenda.json";

    public List<Categoria> Categorias {  get; set; }
    public List<Despesa> Despesas { get; set; }

    public ContextoDados()
    {
        // Espaço para inicializar as Listas (Ex: Contatos = new List<Contato>();)
        Categorias = new List<Categoria>();
        Despesas = new List<Despesa>();
    }

    public void VerificarSistemaOperacional()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pastaArmazenamento = @"C:\temp";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            pastaArmazenamento = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "temp");
        }
    }

    public ContextoDados(bool carregarDados) : this()
    {
        if (carregarDados)
            Carregar();
    }

    public void Salvar()
    {
        VerificarSistemaOperacional();
        string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.WriteIndented = true;
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        string json = JsonSerializer.Serialize(this, jsonOptions);

        if (!Directory.Exists(pastaArmazenamento))
            Directory.CreateDirectory(pastaArmazenamento);

        File.WriteAllText(caminhoCompleto, json);
    }

    public void Carregar()
    {
        VerificarSistemaOperacional();
        string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

        if (!File.Exists(caminhoCompleto)) return;

        string json = File.ReadAllText(caminhoCompleto);

        if (string.IsNullOrWhiteSpace(json)) return;

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        ContextoDados contextoArmazenado = JsonSerializer.Deserialize<ContextoDados>(json, jsonOptions)!;

        if (contextoArmazenado == null)
        {
            return;
        }

        // Espaço para reatribuir as listas conforme o contexto carregado (Ex: Contatos = contextoArmazenado.Contatos;)

        Categorias = contextoArmazenado.Categorias;
        Despesas = contextoArmazenado.Despesas;
    }
}
