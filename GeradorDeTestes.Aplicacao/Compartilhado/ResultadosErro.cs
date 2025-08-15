using FluentResults;

namespace GeradorDeTestes.Aplicacao.Compartilhado;
public abstract class ResultadosErro
{
    public static Error RegistroDuplicadoErro(string mensagemErro)
    {
        return new Error("Registro Duplicado")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "RegistroDuplicado");
    }

    public static Error RegistroVinculadoErro(string mensagemErro)
    {
        return new Error("Registro Vinculado")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "RegistroVinculado");
    }

    public static Error RegistroNaoEncontradoErro(Guid id)
    {
        return new Error("Registro Não Encontrado")
            .CausedBy("Não foi possível obter o registro ID: " + id.ToString())
            .WithMetadata("TipoErro", "RegistroNaoEncontrado");
    }

    public static Error NenhumRegistroGeradoErro(string mensagemErro)
    {
        return new Error("Nenhum Registro Gerado")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "NenhumRegistroGerado");
    }

    public static Error ExcecaoInternaErro(Exception ex)
    {
        return new Error("Ocorreu um erro interno no servidor.")
            .CausedBy(ex)
            .WithMetadata("TipoErro", "ExcecaoInterna");
    }

    public static Error TextoAlternativaObrigatorioErro(string mensagemErro)
    {
        return new Error("Texto Alternativa Obrigatório")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "TextoAlternativaObrigatorio");
    }

    public static Error AlternativaDuplicadaErro(string mensagemErro)
    {
        return new Error("Alternativa Duplicada")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "AlternativaDuplicada");
    }

    public static Error AlternativasErro(string mensagemErro)
    {
        return new Error("Problema Nas Alternativas")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "AlternativasErro");
    }

    public static Error AlternativaNaoEncontradaErro(Guid idAlternativa)
    {
        return new Error("Alternativa Não Encontrada")
            .CausedBy("Não foi possível obter a alternativa ID: " + idAlternativa.ToString())
            .WithMetadata("TipoErro", "AlternativaNaoEncontrada");
    }

    public static Error DisciplinaESerieSemMateriasErro(string mensagemErro)
    {
        return new Error("Disciplina E Série Sem Matérias")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "DisciplinaESerieSemMaterias");
    }

    public static Error MateriaSemQuestoesErro(string mensagemErro)
    {
        return new Error("Matéria Sem Questões")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "MateriaSemQuestoes");
    }

    public static Error QuantidadeQuestoesErro(string mensagemErro)
    {
        return new Error("Problema Na Quantidade de Questões")
            .CausedBy(mensagemErro)
            .WithMetadata("TipoErro", "QuantidadeQuestoes");
    }
}
