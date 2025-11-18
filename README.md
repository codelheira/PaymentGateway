## 🚀 Tecnologias Utilizadas

- Minimal API .NET com C#
- Docker para conteinerização
- Swagger para documentação da API
- AutoMapper para mapeamento de objetos
- Logger para rastreamento e auditoria

---

## 🧩 Arquitetura

- **Hexagonal Architecture (Ports & Adapters)**: separação clara entre domínio, aplicação e infraestrutura.
- **Microserviços**: este serviço podes ser usado como parte de uma plataforma maior, podendo se comunicar com outros serviços via filas (RabbitMQ, SQS, Kafka, etc...) ou APIs REST.
- **DDD, SOLID e Design Patterns**: Padrão Strategy e demais boas praticas aplicadas para garantir escalabilidade, manutenção e legibilidade do código.
- **Tracing**: Implementa Log Tracing para garantir rastreabilidade completa, acelerar o diagnóstico de problemas e aprimorar o monitoramento de desempenho e suporte a incidentes.
  
---

## 🔍 Log Tracing

- Implementação de Rastreamento Distribuído
- Geração de trace ID único por requisição
- Registro de eventos em cada camada da aplicação
- Suporte a correlação de logs entre serviços

---

## 📚 Funcionalidades

- Integração Flexível de Provedores de Pagamento
- Seleção Automática de Provedor
- Tolerância a Falhas de Provedor

---


## 🔐 Autenticação

> Este serviço não implementa autenticação por padrão, mas pode ser facilmente integrado com JWT ou OAuth2.

---

## 📦 Instalação e Execução

### Pré-requisitos

- Docker instalado
- .NET SDK (caso deseje rodar localmente sem Docker)

### Executando

1. Com Docker Compose
```bash
docker-compose up --build

# Para simular indisponibilidade, ajuste as variáveis de ambiente em docker-compose.yml ou exporte antes:
export PAYMENT_FASTPAY_AVAILABLE=false
```

2. Build e run local (opcional):
```bash
cd payment-gateway
dotnet build
dotnet run --project src/payment-gateway.Api/payment-gateway.Api.csproj

# Exemplo de requisição
# POST http://localhost:5000/payments 
# body: {"amount":120.50,"currency":"BRL"}
```

### Resposta Esperada
```json
{
  "id": "GUID",
  "externalId": "SP-12345",
  "status": "approved",
  "provider": "SecurePay", 
  "grossAmount": 120.50,
  "fee": 4.01,
  "netAmount": 116.49
}
```




