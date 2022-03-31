# Testing Atlas

The following command will run all tests, and then generate an html report showing hit miss analysis.

```bash
dotnet restore
npm run test:dev
```


Tests require a few global tools:

```bash
dotnet tool install -g coverlet.console
dotnet tool install -g dotnet-reportgenerator-globaltool
```
