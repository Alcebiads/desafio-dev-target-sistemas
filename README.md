# Target Sistemas

Aplicação de console em C# (.NET 10) com um menu que reúne os 3 exercícios do teste.

## Como executar

```
cd TargetSistemas
dotnet run
```

Ou abrir `TargetSistemas.slnx` no Visual Studio e executar (Ctrl+F5).

## Estrutura

| Arquivo | Responsabilidade |
|---|---|
| `Program.cs` | Menu principal |
| `Comissoes.cs` | Exercício 1 - comissão por vendedor |
| `Estoque.cs` | Exercício 2 - movimentações de estoque |
| `Juros.cs` | Exercício 3 - juros por atraso |
| `Dados/vendas.json` | Dados do exercício 1 |
| `Dados/estoque.json` | Dados do exercício 2 |

## Decisões e premissas

### Exercício 1 - Comissões
- A regra é aplicada **venda a venda**, e depois somada por vendedor.
- Faixas: abaixo de R$ 100,00 = 0%; de R$ 100,00 até R$ 499,99 = 1%; a partir de R$ 500,00 = 5%.
- O enunciado não cita o valor exato de R$ 100,00. Foi considerado 1% (só *abaixo* de 100 não gera comissão).
- Valores monetários usam `decimal`. O arredondamento é feito apenas na exibição.

### Exercício 2 - Estoque
- Os produtos são lidos de `Dados/estoque.json` e ficam em memória durante a execução.
- Cada movimentação recebe um **ID sequencial único**, um **tipo** (Entrada/Saída) e uma **descrição**
  (informada pelo usuário; se vazia, usa "Entrada de mercadoria" ou "Saída de mercadoria").
- Ao final de cada movimentação é exibido o **estoque final** do produto.
- Validações: produto inexistente, quantidade inválida e **saída maior que o estoque disponível**
  (o estoque nunca fica negativo).
- Há também listagem de produtos e histórico de movimentações.
- Os dados não são gravados de volta no JSON (não foi pedido); o estado vale enquanto o programa está aberto.

### Exercício 3 - Juros
- Taxa de **2,5% ao dia**, com **juros simples**: `juros = valor x 0,025 x dias de atraso`.
- Os dias de atraso são contados de `DateTime.Today` em relação ao vencimento.
- Se a conta não está vencida (vencimento hoje ou futuro), não há juros.
- Arredondamento comercial para 2 casas (`MidpointRounding.AwayFromZero`).
- Entrada de data no formato `dd/MM/aaaa`; valor aceita vírgula decimal (cultura pt-BR).
