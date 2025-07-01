using System.Text.Json;
using System.Text.Json.Serialization;
using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;

namespace eAgenda.Infraestrutura.Arquivos.Compartilhado;

public class ContextoDados
{
    public List<Contato> Contatos { get; set; }
    public List<Compromisso> Compromissos { get; set; }
    public List<Categoria> Categorias { get; set; }
    public List<Despesa> Despesas { get; set; }
    public List<Tarefa> Tarefas { get; set; }
    private string pastaArmazenamento = string.Empty;
    private readonly string arquivoArmazenamento = "dados-eAgenda.json";

    public ContextoDados()
    {
        Contatos = new List<Contato>();
        Compromissos = new List<Compromisso>();
        Categorias = new List<Categoria>();
        Despesas = new List<Despesa>();
        Tarefas = new List<Tarefa>();
    }
    public void VerificarSistemaOperacional()
    {
        pastaArmazenamento = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "eAgenda");
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

        if (!Directory.Exists(pastaArmazenamento))
            Directory.CreateDirectory(pastaArmazenamento);

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.WriteIndented = true;
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        string json = JsonSerializer.Serialize(this, jsonOptions);

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

        Contatos = contextoArmazenado.Contatos;
        Compromissos = contextoArmazenado.Compromissos;
        Categorias = contextoArmazenado.Categorias;
        Despesas = contextoArmazenado.Despesas;
        Tarefas = contextoArmazenado.Tarefas;
    }
}
