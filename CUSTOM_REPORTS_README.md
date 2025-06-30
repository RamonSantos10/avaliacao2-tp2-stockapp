# Sistema de Relatórios Personalizados - StockApp

## Descrição

O Sistema de Relatórios Personalizados permite aos usuários criar e visualizar relatórios customizados sobre vendas, inventário, performance de produtos e categorias.

## Funcionalidades Implementadas

### 1. Tipos de Relatórios Disponíveis

- **Relatório de Vendas**: Análise de vendas por produto com métricas de performance
- **Relatório de Inventário**: Status do estoque com identificação de produtos com estoque baixo
- **Relatório de Performance**: Score de performance baseado em preço, estoque e categoria
- **Relatório por Categoria**: Agrupamento de produtos por categoria com valores totais

### 2. Endpoints da API

#### Gerar Relatório Personalizado
```http
POST /api/customreports/generate
Content-Type: application/json

{
  "reportType": "sales",
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "categoryId": 1,
  "productId": null,
  "includeDetails": true,
  "groupBy": "category"
}
```

#### Relatórios Específicos
- `POST /api/customreports/sales` - Relatório de Vendas
- `POST /api/customreports/inventory` - Relatório de Inventário
- `POST /api/customreports/performance` - Relatório de Performance
- `POST /api/customreports/category` - Relatório por Categoria

#### Consultas
- `GET /api/customreports/history` - Histórico de relatórios gerados
- `GET /api/customreports/types` - Tipos de relatórios disponíveis
- `GET /api/customreports/test` - Endpoint de teste

### 3. Estrutura dos DTOs

#### ReportParametersDto
```csharp
public class ReportParametersDto
{
    public string ReportType { get; set; }        // Tipo do relatório
    public DateTime? StartDate { get; set; }      // Data inicial
    public DateTime? EndDate { get; set; }        // Data final
    public int? CategoryId { get; set; }          // ID da categoria
    public int? ProductId { get; set; }           // ID do produto
    public bool IncludeDetails { get; set; }      // Incluir detalhes
    public string GroupBy { get; set; }           // Agrupar por
}
```

#### CustomReportDto
```csharp
public class CustomReportDto
{
    public string Title { get; set; }             // Título do relatório
    public DateTime GeneratedAt { get; set; }     // Data de geração
    public string ReportType { get; set; }        // Tipo do relatório
    public List<ReportDataDto> Data { get; set; } // Dados do relatório
    public ReportSummaryDto Summary { get; set; }  // Resumo do relatório
}
```

### 4. Exemplos de Uso

#### Exemplo 1: Relatório de Vendas
```json
{
  "reportType": "sales",
  "includeDetails": true
}
```

#### Exemplo 2: Relatório de Inventário por Categoria
```json
{
  "reportType": "inventory",
  "categoryId": 1,
  "includeDetails": true
}
```

#### Exemplo 3: Relatório de Performance de Produto Específico
```json
{
  "reportType": "performance",
  "productId": 5
}
```

### 5. Configuração no Program.cs

O serviço foi registrado no container de dependências:

```csharp
// Registro do serviço de relatórios personalizados
builder.Services.AddScoped<ICustomReportService, CustomReportService>();
```

### 6. Testes Unitários

Foi criado o arquivo `CustomReportServiceTests.cs` com testes para:
- Geração de relatórios de vendas
- Geração de relatórios de inventário
- Geração de relatórios por categoria
- Obtenção de tipos de relatórios disponíveis
- Geração de relatórios padrão

### 7. Como Testar

1. **Via Swagger UI**: Acesse `https://localhost:7189/swagger` quando a aplicação estiver rodando
2. **Via Postman**: Use os endpoints documentados acima
3. **Endpoint de Teste**: Use `GET /api/customreports/test` para verificar se o sistema está funcionando

### 8. Métricas Incluídas nos Relatórios

#### Relatório de Vendas
- Total de vendas
- Número total de pedidos
- Valor médio por pedido

#### Relatório de Inventário
- Valor total do estoque
- Número de itens com estoque baixo
- Valor médio do estoque por produto

#### Relatório de Performance
- Score médio de performance
- Número de produtos com alto desempenho
- Categorização por níveis de performance

#### Relatório por Categoria
- Número total de categorias
- Valor médio por categoria
- Distribuição de produtos por categoria

### 9. Funcionalidades Avançadas

- **Histórico de Relatórios**: Todos os relatórios gerados são armazenados em memória
- **Filtros Flexíveis**: Suporte a filtros por data, categoria e produto
- **Múltiplos Formatos**: Dados estruturados com resumos e métricas adicionais
- **Extensibilidade**: Fácil adição de novos tipos de relatórios

### 10. Dependências

O sistema utiliza as seguintes dependências já existentes no projeto:
- AutoMapper (para mapeamento de objetos)
- Entity Framework Core (para acesso a dados)
- ASP.NET Core (para API)
- Repositórios existentes (IProductRepository, ICategoryRepository)

## Conclusão

O Sistema de Relatórios Personalizados foi implementado com sucesso, oferecendo uma solução completa e extensível para geração de relatórios no StockApp. O sistema segue as melhores práticas de arquitetura limpa e está totalmente integrado com a infraestrutura existente do projeto.