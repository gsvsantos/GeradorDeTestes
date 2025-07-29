using Dapper;
using GeradorDeTestes.Dominio.ModuloDisciplina;
using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloTeste;
using System.Data;

namespace GeradorDeTestes.Infraestrutura.ORM.Dapper.ModuloDisciplina;

public class RepositorioDisciplinaDapper : IRepositorioDisciplina
{
    private readonly IDbConnection dbConnection;

    public RepositorioDisciplinaDapper(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    #region SQL queries
    private const string SqlCadastrar =
        @"INSERT INTO 
            ""Disciplinas""
        (
	        ""Id"",
	        ""Nome""
        )
        VALUES
        (
	        @ID,
	        @NOME
        );";

    private const string SqlEditar =
        @"UPDATE
            ""Disciplinas""
        SET
	        ""Nome"" = @NOME
        WHERE
            ""Id"" = @ID;";

    private const string SqlExcluir =
        @"DELETE FROM
            ""Disciplinas""
        WHERE
            ""Id"" = @ID;";

    private const string SqlSelecionarTodos =
        @"SELECT 
            ""Id"",
            ""Nome""
        FROM 
            ""Disciplinas"";";

    private const string SqlSelecionarPorId =
        @"SELECT
            ""Id"",
            ""Nome"" 
        FROM
            ""Disciplinas""
        WHERE
            ""Id"" = @ID;";

    private const string SqlSelecionarMateriasDaDisciplina =
        @"SELECT
            ""Id"", 
            ""Nome"", 
            ""Serie"",
            ""DisciplinaId"" 
        FROM 
            ""Materias""
        WHERE
            ""DisciplinaId"" = @DISCIPLINAID;";

    private const string SqlSelecionarTestesDaDisciplina =
        @"SELECT 
	        ""Id"", 
	        ""Titulo"", 
	        ""DisciplinaId"", 
	        ""Serie"", 
	        ""EhProvao"", 
	        ""QuantidadeQuestoes"", 
	        ""Finalizado"", 
	        ""DataCriacao""
        FROM 
	        ""Testes""
        WHERE
            ""DisciplinaId"" = @DISCIPLINAID;";
    #endregion

    public void CadastrarRegistro(Disciplina novaDisciplina)
    {
        if (novaDisciplina.Id == Guid.Empty)
            novaDisciplina.Id = Guid.NewGuid();

        dbConnection.Execute(SqlCadastrar, new
        {
            ID = novaDisciplina.Id,
            NOME = novaDisciplina.Nome
        });
    }

    public bool EditarRegistro(Guid id, Disciplina disciplinaEditada)
    {
        int linhasAfetadas = dbConnection.Execute(SqlEditar, new
        {
            ID = id,
            NOME = disciplinaEditada.Nome
        });

        return linhasAfetadas >= 1;
    }

    public bool ExcluirRegistro(Guid id)
    {
        int linhasAfetadas = dbConnection.Execute(SqlExcluir, new
        {
            ID = id
        });

        return linhasAfetadas >= 1;
    }

    public Disciplina? SelecionarRegistroPorId(Guid id)
    {
        Disciplina disciplina = dbConnection.QueryFirstOrDefault<Disciplina>(SqlSelecionarPorId,
            new { ID = id })!;

        if (disciplina is null)
            return null;

        disciplina.Materias = SelecionarMateriasDaDisciplina(disciplina.Id);
        disciplina.Testes = SelecionarTestesDaDisciplina(disciplina.Id);

        return disciplina;
    }

    public List<Disciplina> SelecionarRegistros()
    {
        List<Disciplina> disciplinas = dbConnection.Query<Disciplina>(SqlSelecionarTodos).ToList();

        foreach (Disciplina d in disciplinas)
        {
            d.Materias = SelecionarMateriasDaDisciplina(d.Id);
        }

        return disciplinas;
    }

    private List<Materia> SelecionarMateriasDaDisciplina(Guid id)
    {
        return dbConnection.Query<Materia>(SqlSelecionarMateriasDaDisciplina,
            new { DISCIPLINAID = id }).ToList();
    }

    private List<Teste> SelecionarTestesDaDisciplina(Guid id)
    {
        List<Teste> testes = dbConnection.Query<Teste>(SqlSelecionarTestesDaDisciplina,
            new { DISCIPLINAID = id }).ToList();

        foreach (Teste t in testes)
        {
            t.Materias = SelecionarMateriasDoTeste(t.Id);
        }

        return testes;
    }

    private List<Materia> SelecionarMateriasDoTeste(Guid id)
    {
        const string sqlMateriasTestes =
            @"SELECT
                ""MateriasId"", 
                ""TestesId""
            FROM 
                ""MateriaTeste""
            WHERE
                ""TestesId"" = @TESTEID;";

        return dbConnection.Query<Materia>(sqlMateriasTestes,
            new { TESTEID = id }).ToList();
    }
}
