# Do before RUN dotnet publish -c Release -o out
FROM microsoft/dotnet:2.2-aspnetcore-runtime 
WORKDIR /app

COPY /out .

ENV ASPNETCORE_URLS http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "MockSite.Web.dll"]