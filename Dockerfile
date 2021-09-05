FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY /release/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "EndeavourDemo.dll"]
EXPOSE 80