# Architecture Microservices – Exercice 1

### Solution
Nom solution : PAS (Policy Administration System)

### Services
- Service Asset
  - Entité Fund
    - Propriétés : Name, Isin, Currency, Status, Navs
    - API : GetFundList, GetFund, CreateFund, PutFundNav
    - Publish message asychrone : FundNavUpdatedDomainEvent

=> Assemblies :
- PAS.Asset.Domain
- PAS.Asset.Application
- PAS.Asset.Infrastructure
- PAS.Asset.Api (Minimal API + Scalar)

Utilisation : Aspire, MediatR
