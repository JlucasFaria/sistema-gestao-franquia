# Sistema de Gestão de Franquias

API REST em C# para gestão de uma rede de franquias. Centraliza o cadastro da franqueadora
e das unidades franqueadas, o catálogo de produtos e serviços, fornecedores, o estoque de
cada unidade, as vendas, a apuração de royalties, os chamados de suporte e os relatórios
gerenciais, com autenticação e permissões por perfil.

Trabalho da disciplina de Desenvolvimento Back-end.

## Tecnologias

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 com SQLite (migrations)
- Autenticação JWT (Bearer) e autorização por políticas
- Swagger / OpenAPI (Swashbuckle)
- BCrypt para hash de senhas

## Requisitos

SDK do .NET 9.0. Confira a versão instalada com:

```bash
dotnet --version
```

Não é preciso instalar banco de dados: o SQLite é um arquivo criado pela própria aplicação.

## Como executar

```bash
git clone https://github.com/JlucasFaria/sistema-gestao-franquia.git
cd sistema-gestao-franquia
dotnet restore
dotnet run --project src/Franquias.Api
```

Na primeira execução a aplicação aplica as migrations, cria o banco `franquias.db` e
carrega os dados de exemplo. A API sobe em `http://localhost:5270`, e a documentação
interativa fica em <http://localhost:5270/swagger>.

Para usar HTTPS (`https://localhost:7262`):

```bash
dotnet run --project src/Franquias.Api --launch-profile https
```

Para recomeçar com um banco limpo, pare a API, apague o arquivo do banco e execute de novo:

```bash
rm src/Franquias.Api/franquias.db*
dotnet run --project src/Franquias.Api
```

No PowerShell, apague o banco com `Remove-Item src/Franquias.Api/franquias.db*`.

## Usuários de teste

| Perfil | E-mail | Senha |
| --- | --- | --- |
| Administrador | `admin@franquias.com.br` | `Admin@123` |
| Gestor de unidade | `gestor@franquias.com.br` | `Gestor@123` |
| Operador | `operador@franquias.com.br` | `Operador@123` |

O administrador vem das migrations; o gestor e o operador fazem parte dos dados de exemplo.

## Dados de exemplo

Um banco recém-criado recebe uma rede pronta para testes:

- 3 franqueados e 5 unidades: três em operação, uma em implantação e uma encerrada e
  inativa, cada uma com seu responsável;
- 11 produtos e serviços em 4 categorias, incluindo um item descontinuado;
- 3 fornecedores com produtos homologados, um deles inativo;
- estoque por unidade, com itens abaixo do mínimo;
- vendas diárias dos três últimos meses fechados e do mês corrente, entre confirmadas,
  pendentes e canceladas, com as movimentações de estoque correspondentes;
- cobranças de royalty dos três últimos meses, entre pagas, pendentes e atrasadas;
- 8 chamados de suporte em estágios e prioridades diferentes.

As datas são calculadas a partir do dia em que o banco é criado. A carga só acontece em
banco sem unidades e é controlada pela chave `DadosDeExemplo:Carregar`, ligada apenas em
`appsettings.Development.json`.

## Testando a API

**Swagger:** acesse `/swagger`, execute `POST /api/auth/login`, copie o `token` da resposta,
clique em **Authorize** e cole o token. As rotas com cadeado exigem autenticação, e cada uma
informa os códigos de resposta possíveis.

**Arquivo .http:** `src/Franquias.Api/Franquias.Api.http` traz requisições para todos os
endpoints, incluindo casos de erro (validação, recurso inexistente, conflito, regra de
negócio e acesso negado). Ele pode ser executado pelo Visual Studio 2022 ou pela extensão
REST Client do VS Code. Rode primeiro os três logins do início do arquivo: os tokens são
reaproveitados nas demais requisições.

## Estrutura do projeto

```
sistema-gestao-franquia/
├── Franquias.sln
└── src/
    └── Franquias.Api/
        ├── Common/           autenticação, consultas paginadas, exceções, middlewares e validações
        ├── Configurations/   mapeamentos das entidades (Fluent API)
        ├── Controllers/      endpoints da API
        ├── DTOs/             objetos de entrada e saída
        ├── Data/             contexto do EF Core, migrations, carga inicial e dados de exemplo
        ├── Entities/         entidades de domínio
        ├── Repositories/     acesso a dados
        ├── Services/         regras de negócio
        ├── Program.cs
        └── Franquias.Api.http
```

## Endpoints

Todas as listagens aceitam `pagina`, `tamanhoPagina` (até 100), `busca`, `ordenarPor` e
`decrescente`, além dos filtros próprios de cada recurso.

| Módulo | Rotas | Acesso |
| --- | --- | --- |
| Autenticação | `POST /api/auth/login`, `POST /api/auth/registrar` | público / administrador |
| Usuários | `GET, POST /api/usuarios`, `GET, PUT /api/usuarios/{id}`, `PATCH /api/usuarios/{id}/ativar` e `/inativar` | gestor (consulta) / administrador |
| Perfis | `GET /api/perfis`, `GET /api/perfis/{id}`, `PUT /api/perfis/usuarios/{usuarioId}` | todos (consulta) / administrador |
| Franqueadoras | `GET, POST /api/franqueadoras`, `GET, PUT /api/franqueadoras/{id}` | todos (consulta) / administrador |
| Franqueados | `GET, POST /api/franqueados`, `GET, PUT /api/franqueados/{id}` | gestor (consulta) / administrador |
| Unidades | `GET, POST /api/unidades`, `GET, PUT, DELETE /api/unidades/{id}`, `PATCH /api/unidades/{id}/situacao`, `/percentual-royalty`, `/ativar` e `/inativar` | todos (consulta) / administrador |
| Responsáveis | `GET, POST /api/unidades/{unidadeId}/responsaveis`, `PUT, DELETE .../responsaveis/{id}` | todos (consulta) / gestor |
| Categorias | `GET, POST /api/categorias`, `GET, PUT, DELETE /api/categorias/{id}`, `PATCH .../ativar` e `/inativar` | todos (consulta) / administrador |
| Produtos e serviços | `GET, POST /api/produtos`, `GET, PUT, DELETE /api/produtos/{id}`, `PATCH /api/produtos/{id}/status`, `/ativar` e `/inativar` | todos (consulta) / administrador |
| Fornecedores | `GET, POST /api/fornecedores`, `GET, PUT, DELETE /api/fornecedores/{id}`, `PATCH .../ativar` e `/inativar` | gestor (consulta) / administrador |
| Produtos do fornecedor | `GET, POST /api/fornecedores/{id}/produtos`, `PUT, DELETE .../produtos/{produtoId}` | gestor (consulta) / administrador |
| Estoque | `GET /api/estoques`, `GET .../unidades/{u}/produtos/{p}`, `GET .../movimentacoes`, `POST .../entrada`, `POST .../saida`, `POST .../ajuste`, `PUT .../minimo` | operador / gestor (ajuste e mínimo) |
| Vendas | `GET, POST /api/vendas`, `GET /api/vendas/{id}`, `POST /api/vendas/{id}/confirmar`, `POST /api/vendas/{id}/cancelar` | operador / gestor (cancelamento) |
| Royalties | `GET, POST /api/royalties`, `GET /api/royalties/{id}`, `POST .../{id}/pagamento`, `POST .../atualizar-atrasos`, `GET .../unidades/{u}/resumo`, `GET .../resumo-por-unidade` | gestor (consulta) / administrador |
| Chamados | `GET, POST /api/chamados`, `GET /api/chamados/{id}`, `GET /api/chamados/em-aberto`, `POST .../{id}/interacoes`, `PATCH .../{id}/prioridade`, `PATCH .../{id}/status` | operador / gestor (prioridade e status) |
| Relatórios | `GET /api/relatorios/faturamento`, `/ranking-unidades`, `/royalties`, `/produtos-mais-vendidos`, `/estoque-critico`, `/chamados-por-status` | gestor / operador (estoque crítico) |

Os perfis são cumulativos: o administrador acessa tudo, e o gestor acessa também o que é
liberado ao operador.

## Regras de negócio

- Duas unidades, franqueadoras ou fornecedores não podem ter o mesmo CNPJ, e dois usuários
  não podem ter o mesmo e-mail. CPF e CNPJ têm os dígitos verificadores validados.
- Só a unidade ativa e em operação registra vendas. A unidade inativa também não abre
  chamados.
- Uma venda pertence a uma única unidade, tem pelo menos um item, e o total é calculado a
  partir das quantidades e preços dos itens.
- Confirmar a venda baixa o estoque dos produtos físicos numa única transação; se faltar
  saldo para algum item, nada é gravado. Cancelar uma venda confirmada estorna o estoque.
- O estoque nunca fica negativo, nem por venda nem por movimentação manual.
- O royalty é calculado sobre o faturamento das vendas confirmadas no período e o
  percentual configurado na unidade. O percentual fica gravado na cobrança, o período
  precisa estar encerrado e não pode se sobrepor a outra cobrança da mesma unidade.
- A cobrança só é quitada com o valor integral. Cobranças vencidas passam a atrasadas.
- O chamado encerrado não aceita mais mensagens nem mudanças.
- Registros importantes não são apagados: usuários, unidades, produtos e fornecedores são
  inativados (em unidades e produtos, o `DELETE` faz essa exclusão lógica), e categorias
  e fornecedores com vínculos não podem ser excluídos. A rede
  também nunca fica sem um administrador ativo.
- Cada perfil acessa apenas as operações permitidas a ele.
- Erros de validação e de regra de negócio retornam no formato `application/problem+json`,
  com mensagem explicativa e o código HTTP adequado: 400 (dados inválidos), 404 (não
  encontrado), 409 (conflito) e 422 (regra de negócio). Requisições sem token válido
  recebem 401, e as feitas por um perfil sem permissão recebem 403.

## Configuração

Os parâmetros ficam em `src/Franquias.Api/appsettings.json`:

| Chave | Descrição |
| --- | --- |
| `ConnectionStrings:ConexaoPadrao` | Caminho do arquivo SQLite. |
| `Jwt:Emissor` | Emissor do token. |
| `Jwt:Audiencia` | Público-alvo do token. |
| `Jwt:ChaveSecreta` | Chave de assinatura HMAC-SHA256 (mínimo de 32 caracteres). |
| `Jwt:ExpiracaoEmMinutos` | Tempo de validade do token. |
| `DadosDeExemplo:Carregar` | Carrega os dados de exemplo em um banco novo. |

A `ChaveSecreta` é definida apenas em `appsettings.Development.json` e propositalmente fica
vazia em `appsettings.json`, para que nenhum segredo de produção seja versionado. Fora do
ambiente de desenvolvimento, informe o valor por variável de ambiente:

```bash
export Jwt__ChaveSecreta="sua-chave-secreta"
```

O arquivo do banco (`franquias.db`) é gerado localmente e não é versionado.
