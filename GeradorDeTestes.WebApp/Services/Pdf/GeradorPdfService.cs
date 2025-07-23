using GeradorDeTestes.Dominio.ModuloMateria;
using GeradorDeTestes.Dominio.ModuloQuestao;
using GeradorDeTestes.Dominio.ModuloTeste;
using QuestPDF.Fluent;

public class GeradorPdfService
{
    public byte[] GerarPdfTeste(Teste teste)
    {
        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text($"Teste: {teste.Titulo}").FontSize(20).SemiBold();
                page.Content().Column(column =>
                {
                    column.Item().Text($"Disciplina: {teste.Disciplina.Nome}");

                    foreach (Materia materia in teste.Materias)
                    {
                        column.Item().Text($"\nMatéria: {materia.Nome}").FontSize(16).SemiBold();

                        int numeroQuestao = 1;

                        foreach (Questao questao in materia.Questoes)
                        {
                            if (teste.Questoes.All(q => q.Id != questao.Id))
                                continue;

                            column.Item().Text($"{numeroQuestao++}. {questao.Enunciado}");
                            foreach (Alternativa alt in questao.Alternativas)
                            {
                                column.Item().Text($" - {alt.Texto}");
                            }
                            column.Item().Text("");
                        }
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GerarPdfGabarito(Teste teste)
    {
        Document document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text($"Gabarito: {teste.Titulo}").FontSize(20).SemiBold();
                page.Content().Column(column =>
                {
                    column.Item().Text($"Disciplina: {teste.Disciplina.Nome}");

                    foreach (Materia materia in teste.Materias)
                    {
                        column.Item().Text($"\nMatéria: {materia.Nome}").FontSize(16).SemiBold();

                        int numeroQuestao = 1;
                        foreach (Questao questao in materia.Questoes)
                        {
                            if (teste.Questoes.All(q => q.Id != questao.Id))
                                continue;

                            column.Item().Text($"{numeroQuestao++}. {questao.Enunciado}");
                            foreach (Alternativa alt in questao.Alternativas)
                            {
                                string prefixo = alt.EstaCorreta ? "✔" : " ";
                                column.Item().Text($" - {alt.Texto} {prefixo} ");
                            }
                            column.Item().Text("");
                        }
                    }
                });
            });
        });

        return document.GeneratePdf();
    }
}
