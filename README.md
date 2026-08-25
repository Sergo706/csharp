# C# and .NET learning projects

This repository is my sandbox for learning C# and exploring the .NET ecosystem.
It contains small, experimental projects for practicing ASP.NET Core, Entity
Framework Core, authentication, API design, and related tools.

## Current projects

### Movies API

A minimal ASP.NET Core API that returns an in-memory list of movies and accepts
new movies through a `POST` endpoint. It also explores OpenAPI documentation,
CORS, and request logging.

### Todo API

A CRUD ASP.NET Core Web API based on Microsoft's Todo tutorial. It uses
controllers, DTOs, Entity Framework Core, and an in-memory database to manage
todo items.

### DocsParser

DocsParser is a document conversion website built as a small, self-contained
service. Its Nuxt/Vue frontend proxies requests to an ASP.NET Core backend,
which uses LibreOffice and other conversion tools.

It supports the following conversions:

| Source \ Target | PDF | DOCX | Markdown | HTML | PPTX | JPG |
| --------------- | :-: | :--: | :------: | :--: | :--: | :-: |
| PDF             |  —  |  ✓   |    ✓     |  ✓   |  ✓   |  ✓  |
| DOCX            |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| Markdown        |  ✓  |  ✓   |    —     |  ✓   |  —   |  —  |
| HTML            |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| CSV             |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| XLS             |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| XLSX            |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| ODS             |  ✓  |  —   |    —     |  —   |  —   |  —  |
| PPTX            |  ✓  |  —   |    ✓     |  —   |  —   |  —  |
| JPG             |  ✓  |  —   |    —     |  —   |  —   |  —  |

A check mark indicates a supported conversion. A dash indicates that the
conversion isn't currently supported.

The backend uses EF Core, ASP.NET Core Identity, and MySQL for account
management and per-user conversion history. It also includes Google and GitHub authentication, email confirmation, and rate limiting.

The application will be packaged with Docker and self-hosted behind Caddy and a
Cloudflare Tunnel, on a vm.

Currently in the following stage:
- Frontend still doesn't wired fully
- Infrastructure is still in a dev environment

---

More similiar will be added in the future
