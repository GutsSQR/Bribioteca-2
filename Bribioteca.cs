using System;
using System.Collections.Generic;
using System.Linq;

struct PeriodoEmprestimo
{
    public DateTime DataEmprestimo;
    public DateTime? DataDevolucao;

    public PeriodoEmprestimo(DateTime data)
    {
        DataEmprestimo = data;
        DataDevolucao = null;
    }
}

class Pessoa
{
    public string Nome { get; set; }
}

class Usuario : Pessoa
{
    public string Matricula { get; set; }
    public List<Emprestimo> Emprestimos = new List<Emprestimo>();
}

class Livro
{
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Codigodolivro { get; set; }

    private int quantidadeDisponivel;

    public int QuantidadeDisponivel
    {
        get => quantidadeDisponivel;
        set => quantidadeDisponivel = value < 0 ? 0 : value;
    }

    public Livro(string titulo, string autor, string Codigodolivro, int quantidade)
    {
        Titulo = titulo;
        Autor = autor;
        Codigodolivro = codigodolivro;
        QuantidadeDisponivel = quantidade;
    }
}

class Emprestimo
{
    public Livro Livro { get; private set; }
    public Usuario Usuario { get; private set; }
    public PeriodoEmprestimo Periodo { get; private set; }

    public bool Ativo => !Periodo.DataDevolucao.HasValue;

    public Emprestimo(Livro livro, Usuario usuario)
    {
        Livro = livro;
        Usuario = usuario;
        Periodo = new PeriodoEmprestimo(DateTime.Now);
    }

    public void Finalizar()
    {
        Periodo.DataDevolucao = DateTime.Now;
        Livro.QuantidadeDisponivel++;
    }
}

class Biblioteca
{
    private List<Livro> livros = new List<Livro>();
    private List<Usuario> usuarios = new List<Usuario>();
    private List<Emprestimo> emprestimos = new List<Emprestimo>();

    public void CadastrarLivro()
    {
        Console.Write("Título: ");
        string titulo = Console.ReadLine();
        Console.Write("Autor: ");
        string autor = Console.ReadLine();
        Console.Write("Codigo do livro: ");
        string codigodolivro = Console.ReadLine();
        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine());

        livros.Add(new Livro(titulo, autor, codigodolivro, quantidade));
        Console.WriteLine("Livro cadastrado com sucesso!\n");
    }

    public void CadastrarUsuario()
    {
        Console.Write("Nome: ");
        string nome = Console.ReadLine();
        Console.Write("Matrícula: ");
        string matricula = Console.ReadLine();

        usuarios.Add(new Usuario { Nome = nome, Matricula = matricula });
        Console.WriteLine("Usuário cadastrado com sucesso!\n");
    }

    public void RegistrarEmprestimo()
    {
        Console.Write("Matrícula do usuário: ");
        string matricula = Console.ReadLine();
        Usuario usuario = usuarios.FirstOrDefault(u => u.Matricula == matricula);
        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado.\n");
            return;
        }

        Console.Write("Codigodolivro do livro: ");
        string codigodolivro = Console.ReadLine();
        Livro livro = livros.FirstOrDefault(l => l.Codigodolivro == Codigodolivro);
        if (livro == null || livro.QuantidadeDisponivel <= 0)
        {
            Console.WriteLine("Livro não disponível.\n");
            return;
        }

        livro.QuantidadeDisponivel--;
        var emprestimo = new Emprestimo(livro, usuario);
        usuario.Emprestimos.Add(emprestimo);
        emprestimos.Add(emprestimo);

        Console.WriteLine("Empréstimo registrado com sucesso!\n");
    }

    public void RegistrarDevolucao()
    {
        Console.Write("Matrícula do usuário: ");
        string matricula = Console.ReadLine();
        Usuario usuario = usuarios.FirstOrDefault(u => u.Matricula == matricula);
        if (usuario == null)
        {
            Console.WriteLine("Usuário não encontrado.\n");
            return;
        }

        var emprestimosAtivos = usuario.Emprestimos.Where(e => e.Ativo).ToList();
        if (!emprestimosAtivos.Any())
        {
            Console.WriteLine("Nenhum empréstimo ativo encontrado para este usuário.\n");
            return;
        }

        Console.WriteLine("Livros emprestados:");
        for (int i = 0; i < emprestimosAtivos.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {emprestimosAtivos[i].Livro.Titulo}");
        }

        Console.Write("Escolha o número do livro para devolver: ");
        int escolha = int.Parse(Console.ReadLine());
        if (escolha < 1 || escolha > emprestimosAtivos.Count)
        {
            Console.WriteLine("Opção inválida.\n");
            return;
        }

        emprestimosAtivos[escolha - 1].Finalizar();
        Console.WriteLine("Devolução registrada com sucesso!\n");
    }

    public void ExibirRelatorios()
    {
        Console.WriteLine("\nLivros Disponíveis:");
        foreach (var livro in livros.Where(l => l.QuantidadeDisponivel > 0))
        {
            Console.WriteLine($"{livro.Titulo} - {livro.QuantidadeDisponivel} disponível(is)");
        }

        Console.WriteLine("\nLivros Emprestados:");
        foreach (var emprestimo in emprestimos.Where(e => e.Ativo))
        {
            Console.WriteLine($"{emprestimo.Livro.Titulo} emprestado para {emprestimo.Usuario.Nome}");
        }

        Console.WriteLine("\nUsuários com Livros:");
        foreach (var usuario in usuarios)
        {
            var ativos = usuario.Emprestimos.Where(e => e.Ativo).ToList();
            if (ativos.Any())
            {
                Console.WriteLine($"{usuario.Nome} tem {ativos.Count} livro(s) emprestado(s)");
            }
        }

        Console.WriteLine();
    }

    public void Menu()
    {
        int opcao;
        do
        {
            Console.WriteLine("===== MENU BIBLIOTECA =====");
            Console.WriteLine("1. Cadastrar Livro");
            Console.WriteLine("2. Cadastrar Usuário");
            Console.WriteLine("3. Registrar Empréstimo");
            Console.WriteLine("4. Registrar Devolução");
            Console.WriteLine("5. Exibir Relatórios");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha uma opção: ");
            opcao = int.Parse(Console.ReadLine());
            Console.WriteLine();

            switch (opcao)
            {
                case 1: CadastrarLivro(); break;
                case 2: CadastrarUsuario(); break;
                case 3: RegistrarEmprestimo(); break;
                case 4: RegistrarDevolucao(); break;
                case 5: ExibirRelatorios(); break;
                case 0: Console.WriteLine("Encerrando sistema..."); break;
                default: Console.WriteLine("Opção inválida.\n"); break;
            }
        } while (opcao != 0);
    }
}
