# Testing Atlas

The following command will run all tests, and then generate an html report showing hit miss analysis.

```bash
dotnet build; \
coverlet web.Tests/bin/Debug/net6.0/web.Tests.dll --target "dotnet" --targetargs "test  --no-build" --format cobertura --exclude-by-file "**/Migrations/*"; \
reportgenerator -reports:coverage.cobertura.xml -targetdir:coverage/ -reporttypes:html
```

Tests require a few global tools:

```bash
dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
```
