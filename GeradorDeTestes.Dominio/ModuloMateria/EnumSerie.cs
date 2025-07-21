using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.Dominio.ModuloMateria;

public enum EnumSerie
{
    [Display(Name = "1º ano do Ensino Fundamental")]
    PrimeiroAnoFundamental = 1,

    [Display(Name = "2º ano do Ensino Fundamental")]
    SegundoAnoFundamental = 2,

    [Display(Name = "3º ano do Ensino Fundamental")]
    TerceiroAnoFundamental = 3,

    [Display(Name = "4º ano do Ensino Fundamental")]
    QuartoAnoFundamental = 4,

    [Display(Name = "5º ano do Ensino Fundamental")]
    QuintoAnoFundamental = 5,

    [Display(Name = "6º ano do Ensino Fundamental")]
    SextoAnoFundamental = 6,

    [Display(Name = "7º ano do Ensino Fundamental")]
    SetimoAnoFundamental = 7,

    [Display(Name = "8º ano do Ensino Fundamental")]
    OitavoAnoFundamental = 8,

    [Display(Name = "9º ano do Ensino Fundamental")]
    NonoAnoFundamental = 9,

    [Display(Name = "1ª série do Ensino Médio")]
    PrimeiraSerieMedio = 10,

    [Display(Name = "2ª série do Ensino Médio")]
    SegundaSerieMedio = 11,

    [Display(Name = "3ª série do Ensino Médio")]
    TerceiraSerieMedio = 12
}
