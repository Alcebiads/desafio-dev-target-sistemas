using System.Globalization;
using System.Text.Json;

namespace TargetSistemas
{
    // Modelos que representam o JSON (as propriedades do JSON estão em minúsculo,
    // por isso a leitura usa PropertyNameCaseInsensitive).
    public record Venda(string Vendedor, decimal Valor);
    public record DadosVendas(List<Venda> Vendas);

    public static class Comissoes
    {
        // Regras de comissão (aplicadas VENDA A VENDA, não sobre o total do vendedor):
        //   valor <  R$ 100,00            -> sem comissão
        //   R$ 100,00 <= valor < R$ 500,00 -> 1%
        //   valor >= R$ 500,00            -> 5%
        // Premissa: o enunciado não cita explicitamente o valor exatamente R$ 100,00;
        // foi tratado como 1% ("abaixo de 100 não gera" => 100 já gera).
        private const decimal LimiteSemComissao = 100m;
        private const decimal LimiteComissaoMaior = 500m;
        private const decimal PercentualBaixo = 0.01m;
        private const decimal PercentualAlto = 0.05m;

        private static readonly CultureInfo PtBr = new("pt-BR");

        public static void Executar()
        {
            Console.WriteLine("=== COMISSÃO POR VENDEDOR ===");
            Console.WriteLine();

            DadosVendas? dados = CarregarVendas();

            if (dados is null || dados.Vendas.Count == 0)
            {
                Console.WriteLine("Nenhuma venda encontrada.");
                return;
            }

            var resumo = dados.Vendas
                .GroupBy(v => v.Vendedor)
                .Select(g => new
                {
                    Vendedor = g.Key,
                    QtdVendas = g.Count(),
                    TotalVendido = g.Sum(v => v.Valor),
                    Comissao = g.Sum(v => CalcularComissao(v.Valor))
                })
                .ToList();

            Console.WriteLine($"{"Vendedor",-18}{"Vendas",8}{"Total vendido",18}{"Comissão",16}");
            Console.WriteLine(new string('-', 60));

            foreach (var r in resumo)
            {
                Console.WriteLine(
                    $"{r.Vendedor,-18}{r.QtdVendas,8}" +
                    $"{r.TotalVendido.ToString("C2", PtBr),18}" +
                    $"{r.Comissao.ToString("C2", PtBr),16}");
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"TOTAL",-18}{resumo.Sum(r => r.QtdVendas),8}" +
                              $"{resumo.Sum(r => r.TotalVendido).ToString("C2", PtBr),18}" +
                              $"{resumo.Sum(r => r.Comissao).ToString("C2", PtBr),16}");
        }

        // Calcula a comissão de UMA venda. A soma fica sem arredondar; só a exibição formata.
        public static decimal CalcularComissao(decimal valor)
        {
            if (valor < LimiteSemComissao)
                return 0m;

            if (valor < LimiteComissaoMaior)
                return valor * PercentualBaixo;

            return valor * PercentualAlto;
        }

        private static DadosVendas? CarregarVendas()
        {
            string caminho = Path.Combine(AppContext.BaseDirectory, "Dados", "vendas.json");

            if (!File.Exists(caminho))
            {
                Console.WriteLine($"Arquivo não encontrado: {caminho}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(caminho);
                var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<DadosVendas>(json, opcoes);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON inválido: {ex.Message}");
                return null;
            }
        }
    }
}
