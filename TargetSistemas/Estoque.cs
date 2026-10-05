using System.Text.Json;
using System.Text.Json.Serialization;

namespace TargetSistemas
{
    public class Produto
    {
        [JsonPropertyName("codigoProduto")]
        public int Codigo { get; set; }

        [JsonPropertyName("descricaoProduto")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("estoque")]
        public int Quantidade { get; set; }
    }

    public class DadosEstoque
    {
        [JsonPropertyName("estoque")]
        public List<Produto> Produtos { get; set; } = new();
    }

    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2
    }

    public class Movimentacao
    {
        public int Id { get; init; }
        public int CodigoProduto { get; init; }
        public TipoMovimentacao Tipo { get; init; }
        public string Descricao { get; init; } = string.Empty;
        public int Quantidade { get; init; }
        public int SaldoApos { get; init; }
        public DateTime DataHora { get; init; }

        public string TipoTexto => Tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída";
    }

    public static class Estoque
    {
        // Os dados ficam em memória enquanto o programa roda, assim o estoque
        // continua atualizado quando o usuário volta ao menu principal.
        private static List<Produto>? _produtos;
        private static readonly List<Movimentacao> _movimentacoes = new();

        // Identificador único: contador sequencial (1, 2, 3...).
        private static int _proximoId = 1;

        public static void Executar()
        {
            if (!CarregarProdutos())
                return;

            bool voltar = false;

            while (!voltar)
            {
                Console.Clear();
                Console.WriteLine("=== ESTOQUE ===");
                Console.WriteLine("1 - Lançar movimentação");
                Console.WriteLine("2 - Listar produtos");
                Console.WriteLine("3 - Histórico de movimentações");
                Console.WriteLine("0 - Voltar");
                Console.Write("Escolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine("Digite apenas números.");
                    Pausar();
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        LancarMovimentacao();
                        break;
                    case 2:
                        ListarProdutos();
                        break;
                    case 3:
                        ListarHistorico();
                        break;
                    case 0:
                        voltar = true;
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }

                if (!voltar)
                    Pausar();
            }
        }

        private static void LancarMovimentacao()
        {
            Console.WriteLine();
            ListarProdutos();
            Console.WriteLine();

            Produto? produto = LerProduto();
            if (produto is null)
                return;

            TipoMovimentacao? tipo = LerTipo();
            if (tipo is null)
                return;

            int? quantidade = LerQuantidade();
            if (quantidade is null)
                return;

            // Regra de negócio: não permite saída maior que o estoque disponível.
            if (tipo == TipoMovimentacao.Saida && quantidade > produto.Quantidade)
            {
                Console.WriteLine();
                Console.WriteLine($"Estoque insuficiente. Disponível: {produto.Quantidade}, solicitado: {quantidade}.");
                return;
            }

            string descricao = LerDescricao(tipo.Value);

            // Atualiza o saldo.
            produto.Quantidade += tipo == TipoMovimentacao.Entrada ? quantidade.Value : -quantidade.Value;

            var movimentacao = new Movimentacao
            {
                Id = _proximoId++,
                CodigoProduto = produto.Codigo,
                Tipo = tipo.Value,
                Descricao = descricao,
                Quantidade = quantidade.Value,
                SaldoApos = produto.Quantidade,
                DataHora = DateTime.Now
            };

            _movimentacoes.Add(movimentacao);

            Console.WriteLine();
            Console.WriteLine("Movimentação registrada com sucesso!");
            Console.WriteLine($"Nº da movimentação : {movimentacao.Id}");
            Console.WriteLine($"Tipo               : {movimentacao.TipoTexto}");
            Console.WriteLine($"Descrição          : {movimentacao.Descricao}");
            Console.WriteLine($"Produto            : {produto.Codigo} - {produto.Descricao}");
            Console.WriteLine($"Quantidade         : {movimentacao.Quantidade}");
            Console.WriteLine($"Estoque final      : {produto.Quantidade}");
        }

        private static void ListarProdutos()
        {
            Console.WriteLine($"{"Código",-8}{"Produto",-30}{"Estoque",8}");
            Console.WriteLine(new string('-', 46));

            foreach (var p in _produtos!)
                Console.WriteLine($"{p.Codigo,-8}{p.Descricao,-30}{p.Quantidade,8}");
        }

        private static void ListarHistorico()
        {
            Console.WriteLine();

            if (_movimentacoes.Count == 0)
            {
                Console.WriteLine("Nenhuma movimentação lançada ainda.");
                return;
            }

            Console.WriteLine($"{"Nº",-5}{"Data/Hora",-18}{"Produto",-9}{"Tipo",-9}{"Qtde",6}{"Saldo",8}  Descrição");
            Console.WriteLine(new string('-', 80));

            foreach (var m in _movimentacoes)
            {
                Console.WriteLine(
                    $"{m.Id,-5}{m.DataHora,-18:dd/MM/yy HH:mm}" +
                    $"{m.CodigoProduto,-9}{m.TipoTexto,-9}{m.Quantidade,6}{m.SaldoApos,8}  {m.Descricao}");
            }
        }

        private static Produto? LerProduto()
        {
            Console.Write("Código do produto (0 para cancelar): ");

            if (!int.TryParse(Console.ReadLine(), out int codigo))
            {
                Console.WriteLine("Código inválido.");
                return null;
            }

            if (codigo == 0)
                return null;

            Produto? produto = _produtos!.FirstOrDefault(p => p.Codigo == codigo);

            if (produto is null)
                Console.WriteLine("Produto não encontrado.");

            return produto;
        }

        private static TipoMovimentacao? LerTipo()
        {
            Console.Write("Tipo (1 - Entrada, 2 - Saída): ");

            if (int.TryParse(Console.ReadLine(), out int valor) && Enum.IsDefined(typeof(TipoMovimentacao), valor))
                return (TipoMovimentacao)valor;

            Console.WriteLine("Tipo inválido.");
            return null;
        }

        private static int? LerQuantidade()
        {
            Console.Write("Quantidade: ");

            if (int.TryParse(Console.ReadLine(), out int quantidade) && quantidade > 0)
                return quantidade;

            Console.WriteLine("Quantidade inválida. Informe um número inteiro maior que zero.");
            return null;
        }

        private static string LerDescricao(TipoMovimentacao tipo)
        {
            Console.Write("Descrição da movimentação (ex: Compra, Venda, Devolução; Enter para padrão): ");
            string? texto = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(texto))
                return texto.Trim();

            return tipo == TipoMovimentacao.Entrada ? "Entrada de mercadoria" : "Saída de mercadoria";
        }

        private static bool CarregarProdutos()
        {
            if (_produtos is not null)
                return true;

            string caminho = Path.Combine(AppContext.BaseDirectory, "Dados", "estoque.json");

            if (!File.Exists(caminho))
            {
                Console.WriteLine($"Arquivo não encontrado: {caminho}");
                return false;
            }

            try
            {
                string json = File.ReadAllText(caminho);
                DadosEstoque? dados = JsonSerializer.Deserialize<DadosEstoque>(json);
                _produtos = dados?.Produtos ?? new List<Produto>();
                return true;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON inválido: {ex.Message}");
                return false;
            }
        }

        private static void Pausar()
        {
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}