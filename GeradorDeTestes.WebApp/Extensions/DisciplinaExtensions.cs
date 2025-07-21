using GeradorDeTestes.Dominio.ModuloDisciplina;

public static class DisciplinaExtensions
{
    public static Disciplina ParaEntidade(this FormularioDisciplinaViewModel vm)
    {
        return new Disciplina(vm.Nome);
    }

    public static DetalhesDisciplinaViewModel ParaDetalhesVM(this Disciplina d)
    {
        return new()
        {
            Id = d.Id,
            Nome = d.Nome,
            Materias = d.Materias.Select(m => new MateriaResumoViewModel
            {
                Nome = m.Nome,
                Serie = m.Serie
            }).ToList(),
            Testes = d.Testes.Select(t => new TesteResumoViewModel
            {
                Titulo = t.Titulo,
                NomeMateria = t.Materias.FirstOrDefault()?.Nome ?? "N/A",
                QuantidadeQuestoes = t.QuantidadeQuestoes
            }).ToList()
        };
    }
}
