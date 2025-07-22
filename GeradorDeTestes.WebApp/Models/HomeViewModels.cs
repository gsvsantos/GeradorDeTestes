using GeradorDeTestes.Dominio.ModuloTeste;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeradorDeTestes.WebApp.Models;

public class HomeViewModel
{
    public List<DetalhesUltimosTestes> UltimosTestes { get; set; } = new List<DetalhesUltimosTestes>();

    public HomeViewModel(List<Teste> testes)
    {
        foreach (Teste t in testes)
        {
            UltimosTestes.Add(new()
            {
                Id = t.Id,
                Titulo = t.Titulo,
                DataCriacao = t.DataCriacao,
                NomeDisciplina = t.Disciplina.Nome,
                Materias = t.Materias.Select(m => new SelectListItem()
                {
                    Text = m.Nome,
                    Value = m.Id.ToString()
                }).ToList(),
                EhProvao = t.EhProvao,
                QuantidadeQuestoes = t.QuantidadeQuestoes
            });
        }
    }
}

public class DetalhesUltimosTestes
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public DateTime DataCriacao { get; set; }
    public string NomeDisciplina { get; set; }
    public List<SelectListItem> Materias { get; set; } = new List<SelectListItem>();
    public bool EhProvao { get; set; }
    public int QuantidadeQuestoes { get; set; }
}