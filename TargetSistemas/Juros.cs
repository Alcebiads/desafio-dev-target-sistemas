using System.Globalization;

namespace TargetSistemas
{
    public static class Juros
    {
        // O enunciado não diz se os juros são simples ou compostos.
        // Adotei como padão JUROS SIMPLES: juros = valor * taxa * diasAtraso.
        private const decimal TaxaDiaria = 0.025m; // 2,5% ao dia

        private static readonly CultureInfo PtBr = new("pt-BR");

        public static void Executar()
        {
            Console.WriteLine("=== CÁLCULO DE JUROS POR ATRASO ===");

            decimal valor = LerValor();
            DateTime vencimento = LerData();

            int diasAtraso = (DateTime.Today - vencimento.Date).Days;

            if (diasAtraso <= 0)
            {
                Console.WriteLine();
                Console.WriteLine("A conta não está vencida. Não há juros a cobrar.");
                return;
            }

            decimal juros = CalcularJuros(valor, diasAtraso);
            decimal total = valor + juros;

            Console.WriteLine();
            Console.WriteLine($"Valor original : {valor.ToString("C2", PtBr)}");
            Console.WriteLine($"Vencimento     : {vencimento:dd/MM/yyyy}");
            Console.WriteLine($"Dias em atraso : {diasAtraso}");
            Console.WriteLine($"Juros ({TaxaDiaria.ToString("P1", PtBr)} ao dia): {juros.ToString("C2", PtBr)}");
            Console.WriteLine($"Total a pagar  : {total.ToString("C2", PtBr)}");
        }

        private static decimal CalcularJuros(decimal valor, int diasAtraso)
        {
            // AwayFromZero: 375,125 vira 375,13 (o padrão do .NET arredondaria para 375,12).
            return Math.Round(valor * TaxaDiaria * diasAtraso, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal LerValor()
        {
            while (true)
            {
                Console.Write("Informe o valor (ex: 1500,50): ");
                string? entrada = Console.ReadLine();

                if (decimal.TryParse(entrada, NumberStyles.Number, PtBr, out decimal valor) && valor > 0)
                    return valor;

                Console.WriteLine("Valor inválido. Digite um número maior que zero.");
            }
        }

        private static DateTime LerData()
        {
            while (true)
            {
                Console.Write("Informe a data de vencimento (dd/MM/aaaa): ");
                string? entrada = Console.ReadLine();

                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", PtBr, DateTimeStyles.None, out DateTime data))
                    return data;

                Console.WriteLine("Data inválida. Use o formato dd/MM/aaaa.");
            }
        }
    }
}
