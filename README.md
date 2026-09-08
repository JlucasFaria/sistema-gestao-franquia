# Sistema de Gestão de Franquias

API REST para gestão de uma rede de franquias. Contempla o cadastro da franqueadora e das
unidades franqueadas, o catálogo de produtos e serviços, fornecedores, controle de estoque
por unidade, registro de vendas, apuração de royalties, chamados de suporte e relatórios
gerenciais.

Trabalho da disciplina de Desenvolvimento Back-end.

## Stack

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 com SQLite
- Autenticação JWT (Bearer)
- Swagger / OpenAPI (Swashbuckle)
- BCrypt para hash de senhas
- xUnit para os testes automatizados

## Pré-requisitos

SDK do .NET 9.0. Confira a versão instalada com:

```bash
dotnet --version
```

## Como executar

```bash
git clone <url-do-repositorio>
cd sistema-gestao-franquias
dotnet restore
dotnet run --project src/Franquias.Api
```

A API sobe em `http://localhost:5270` (perfil `http`) ou em `https://localhost:7262`
(perfil `https`). A documentação interativa fica em `/swagger`.

Para compilar e rodar os testes:

```bash
dotnet build
dotnet test
```

## Estrutura do projeto

```
sistema-gestao-franquias/
├── Franquias.sln
└── src/
    └── Franquias.Api/
        ├── Common/           utilitários, validações e tipos compartilhados
        ├── Configurations/   mapeamentos das entidades (Fluent API)
        ├── Controllers/      endpoints da API
        ├── DTOs/             objetos de entrada e saída
        ├── Data/             contexto do EF Core e carga inicial de dados
        ├── Entities/         entidades de domínio
        ├── Repositories/     acesso a dados
        └── Services/         regras de negócio
```

## Configuração

Os parâmetros ficam em `src/Franquias.Api/appsettings.json`:

| Chave | Descrição |
| --- | --- |
| `ConnectionStrings:ConexaoPadrao` | Caminho do arquivo SQLite. |
| `Jwt:Emissor` | Emissor do token. |
| `Jwt:Audiencia` | Público-alvo do token. |
| `Jwt:ChaveSecreta` | Chave de assinatura HMAC-SHA256 (mínimo de 32 caracteres). |
| `Jwt:ExpiracaoEmMinutos` | Tempo de validade do token. |

A `ChaveSecreta` é definida apenas em `appsettings.Development.json` e propositalmente fica
vazia em `appsettings.json`, para que nenhum segredo de produção seja versionado. Fora do
ambiente de desenvolvimento, informe o valor por variável de ambiente:

```bash
export Jwt__ChaveSecreta="sua-chave-secreta"
```

O arquivo do banco (`franquias.db`) é gerado localmente e não é versionado.
