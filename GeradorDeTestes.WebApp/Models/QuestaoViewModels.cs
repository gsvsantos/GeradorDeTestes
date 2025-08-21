using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using GeradorDeTestes.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.WebApp.Models;

public class FormularioQuestaoViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Digite o Enunciado.")]
    [DisplayName("Enunciado")]
    public string Enunciado { get; set; }

    [Required(ErrorMessage = "Escolha uma Matéria.")]
    [DisplayName("Matéria")]
    public Guid MateriaId { get; set; }
    public List<SelectListItem> Materias { get; set; } = new List<SelectListItem>();
}

public class CadastrarQuestaoViewModel : FormularioQuestaoViewModel
{
    public CadastrarQuestaoViewModel() { }
    public CadastrarQuestaoViewModel(List<Materia> materias) : this()
    {
        foreach (Materia m in materias)
        {
            string serieFormatada = (int)m.Serie >= 10 ? m.Serie.GetDisplayName()[..8] : m.Serie.GetDisplayName()[..7];

            Materias.Add(new()
            {
                Text = $"{m.Nome} - {serieFormatada}",
                Value = m.Id.ToString()
            });
        }
    }
}

public class VisualizarQuestoesViewModel
{
    public List<DetalhesQuestaoViewModel> Registros { get; set; } = new List<DetalhesQuestaoViewModel>();

    public VisualizarQuestoesViewModel(List<Questao> questoes)
    {
        foreach (Questao q in questoes)
        {
            Registros.Add(q.ParaDetalhesVM());
        }
    }
}

public class EditarQuestaoViewModel : FormularioQuestaoViewModel
{
    public EditarQuestaoViewModel() { }
    public EditarQuestaoViewModel(Questao questao, List<Materia> materias) : this()
    {
        Id = questao.Id;
        Enunciado = questao.Enunciado;
        MateriaId = questao.Materia != null ? questao.Materia.Id : Guid.Empty;
        foreach (Materia m in materias)
        {
            Materias.Add(new()
            {
                Text = m.Nome,
                Value = m.Id.ToString()
            });
        }
    }
}

public class ExcluirQuestaoViewModel : FormularioQuestaoViewModel
{
    public ExcluirQuestaoViewModel() { }
    public ExcluirQuestaoViewModel(Guid id, string enunciado) : this()
    {
        Id = id;
        Enunciado = enunciado;
    }
}

public class GerenciarAlternativasViewModel
{
    public DetalhesQuestaoViewModel Questao { get; set; }
    public List<AlternativaQuestaoViewModel> Alternativas { get; set; } = new List<AlternativaQuestaoViewModel>();
    public string TextoAlternativa { get; set; }

    public GerenciarAlternativasViewModel() { }
    public GerenciarAlternativasViewModel(Questao questao, List<Alternativa> alternativas) : this()
    {
        Questao = questao.ParaDetalhesVM();
        foreach (Alternativa a in alternativas)
        {
            Alternativas.Add(new(
                a.Id,
                a.Texto,
                a.EstaCorreta));
        }
    }
}

public class AdicionarAlternativaViewModel
{
    [Required(ErrorMessage = "Digite o texto da alternativa.")]
    [StringLength(200, ErrorMessage = "A alternativa deve ter no máximo 200 caracteres.")]
    public string TextoAlternativa { get; set; }
}

public class DetalhesQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Enunciado { get; set; }
    public string NomeMateria { get; set; }
    public string NomeDisciplina { get; set; }
    public List<AlternativaQuestaoViewModel> Alternativas { get; set; } = new List<AlternativaQuestaoViewModel>();
    public List<TesteQuestaoViewModel> Testes { get; set; } = new List<TesteQuestaoViewModel>();

    public DetalhesQuestaoViewModel(Guid id, string enunciado, string nomeMateria, string nomeDisciplina, List<Alternativa> alternativas, List<Teste> testes)
    {
        Id = id;
        Enunciado = enunciado;
        NomeMateria = nomeMateria;
        NomeDisciplina = nomeDisciplina;
        foreach (Alternativa a in alternativas)
        {
            Alternativas.Add(new(
                a.Id,
                a.Texto,
                a.EstaCorreta));
        }
        foreach (Teste t in testes)
        {
            Testes.Add(new(
                t.Id,
                t.Titulo,
                t.Serie.GetDisplayName()));
        }
    }
}

public class AlternativaQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Texto { get; set; }
    public bool EstaCorreta { get; set; }

    public AlternativaQuestaoViewModel(Guid id, string texto, bool estaCorreta)
    {
        Texto = texto;
        Id = id;
        EstaCorreta = estaCorreta;
    }
}

public class TesteQuestaoViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Serie { get; set; }

    public TesteQuestaoViewModel(Guid id, string titulo, string serie)
    {
        Id = id;
        Titulo = titulo;
        Serie = serie;
    }
}

public class PrimeiraEtapaGerarQuestoesViewModel
{
    [Required(ErrorMessage = "O campo \"Matéria\" é obrigatório.")]
    public Guid MateriaId { get; set; }
    public List<SelectListItem> MateriasDisponiveis { get; set; } = new List<SelectListItem>();

    [Required(ErrorMessage = "O campo \"Quantidade de Questões\" é obrigatório.")]
    [Range(1, 100, ErrorMessage = "O campo \"Quantidade de Questões\" precisa conter um valor numérico entre 1 e 100.")]
    public int QuantidadeQuestoes { get; set; }

    public PrimeiraEtapaGerarQuestoesViewModel() { }

    public PrimeiraEtapaGerarQuestoesViewModel(List<Materia> materias) : this()
    {
        foreach (Materia m in materias)
        {
            string serieFormatada = (int)m.Serie >= 10 ? m.Serie.GetDisplayName()[..8] : m.Serie.GetDisplayName()[..7];

            MateriasDisponiveis.Add(new()
            {
                Text = $"{m.Nome} {serieFormatada} - {m.Disciplina.Nome}",
                Value = m.Id.ToString()
            });
        }
    }
}

public class SegundaEtapaGerarQuestoesViewModel
{
    public required Guid MateriaId { get; set; }
    public required string Materia { get; set; }

    public List<QuestaoGeradaViewModel> QuestoesGeradas { get; set; } = new List<QuestaoGeradaViewModel>();

    public SegundaEtapaGerarQuestoesViewModel() { }

    public SegundaEtapaGerarQuestoesViewModel(List<Questao> questoes) : this()
    {
        QuestoesGeradas = questoes
            .Select(QuestaoGeradaViewModel.ParaViewModel)
            .ToList();
    }

    public static List<Questao> ObterQuestoesGeradas(SegundaEtapaGerarQuestoesViewModel segundaEtapaVm, Materia materiaSelecionada, Guid? usuarioId)
    {
        List<Questao> questoes = new List<Questao>();

        foreach (QuestaoGeradaViewModel questaoVm in segundaEtapaVm.QuestoesGeradas)
        {
            Questao questao = new()
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId ?? Guid.Empty,
                Enunciado = questaoVm.Enunciado,
                Materia = materiaSelecionada
            };

            foreach (AlternativaQuestaoGeradaViewModel alternativaVm in questaoVm.AlternativasGeradas)
                questao.AderirAlternativa(new()
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId ?? Guid.Empty,
                    Texto = alternativaVm.Resposta,
                    EstaCorreta = alternativaVm.Correta
                });

            questoes.Add(questao);
        }

        return questoes;
    }

    public class QuestaoGeradaViewModel
    {
        public required string Enunciado { get; set; }
        public required List<AlternativaQuestaoGeradaViewModel> AlternativasGeradas { get; set; } = new List<AlternativaQuestaoGeradaViewModel>();

        public static QuestaoGeradaViewModel ParaViewModel(Questao questao)
        {
            return new QuestaoGeradaViewModel
            {
                Enunciado = questao.Enunciado,
                AlternativasGeradas = questao.Alternativas
                    .Select(AlternativaQuestaoGeradaViewModel.ParaViewModel)
                    .ToList()
            };
        }
    }

    public class AlternativaQuestaoGeradaViewModel
    {
        public required string Resposta { get; set; }
        public required bool Correta { get; set; }

        public static AlternativaQuestaoGeradaViewModel ParaViewModel(Alternativa alternativa)
        {
            return new AlternativaQuestaoGeradaViewModel
            {
                Resposta = alternativa.Texto,
                Correta = alternativa.EstaCorreta
            };
        }
    }
}
