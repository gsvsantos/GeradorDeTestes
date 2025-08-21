﻿using GeradorDeTestes.Dominio.ModuloAutenticacao;
using System.ComponentModel.DataAnnotations;

namespace GeradorDeTestes.WebApp.Models;


public class RegistroViewModel
{
    [Required(ErrorMessage = "O campo \"Email\" é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo \"Email\" precisa ser um endereço de email válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O campo \"Senha\" é obrigatório.")]
    public string? Senha { get; set; }

    [Required(ErrorMessage = "O campo \"Confirmação da Senha\" é obrigatório.")]
    [Compare(nameof(Senha), ErrorMessage = "O campo \"Confirmação da Senha\" não é igual à senha.")]
    public string? ConfirmarSenha { get; set; }

    [Required(ErrorMessage = "O campo \"Tipo de Usuário\" é obrigatório.")]
    public TipoUsuario Tipo { get; set; }
}

public class LoginViewModel
{
    [Required(ErrorMessage = "O campo \"Email\" é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo \"Email\" precisa ser um endereço de email válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "O campo \"Senha\" é obrigatório.")]
    public string? Senha { get; set; }
}
