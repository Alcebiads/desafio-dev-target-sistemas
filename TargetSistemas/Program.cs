using TargetSistemas;

// Garante acentos e símbolos corretos no console do Windows.
Console.OutputEncoding = System.Text.Encoding.UTF8;

bool executando = true;

while (executando)
{
    Console.Clear();
    Console.WriteLine("=== SISTEMA DE CONTROLE INTERNO ===");
    Console.WriteLine("1 - Comissões");
    Console.WriteLine("2 - Estoque");
    Console.WriteLine("3 - Juros");
    Console.WriteLine("0 - Sair");
    Console.Write("Escolha uma opção: ");

    string? entrada = Console.ReadLine();

    if (!int.TryParse(entrada, out int opcao))
    {
        Console.WriteLine("Digite apenas números.");
        Pausar();
        continue;
    }

    switch (opcao)
    {
        case 1:
            Comissoes.Executar();
            Pausar();
            break;
        case 2:
            // O Estoque tem seu próprio submenu (com pausas internas),
            // por isso não pausa aqui ao voltar.
            Estoque.Executar();
            break;
        case 3:
            Juros.Executar();
            Pausar();
            break;
        case 0:
            executando = false;
            break;
        default:
            Console.WriteLine("Opção inválida.");
            Pausar();
            break;
    }
}

static void Pausar()
{
    Console.WriteLine();
    Console.WriteLine("Pressione qualquer tecla para continuar...");
    Console.ReadKey();
}
