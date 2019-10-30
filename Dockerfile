FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY . .
ENTRYPOINT ["dotnet", "TestPing.dll"]