
## ccompile a .NET console application into a single executable file
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true