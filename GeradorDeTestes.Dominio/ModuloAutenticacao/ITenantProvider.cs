﻿namespace GeradorDeTestes.Dominio.ModuloAutenticacao;
public interface ITenantProvider
{
    Guid? UsuarioId { get; }
}
