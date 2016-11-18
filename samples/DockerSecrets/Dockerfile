FROM microsoft/dotnet:1.1.0-runtime
ENTRYPOINT ["dotnet", "DockerSecrets.dll"]
ARG source=.
WORKDIR /app
COPY $source .
