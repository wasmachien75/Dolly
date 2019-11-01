dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

%USERPROFILE%\.nuget\packages\reportgenerator\4.3.5\tools\net47\ReportGenerator.exe -reports:"coverage.cobertura.xml" -targetdir:"./coverage"

del coverage.cobertura.xml