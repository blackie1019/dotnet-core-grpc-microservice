# Do before RUN dotnet publish -c Release -o out
FROM microsoft/dotnet:2.1-aspnetcore-runtime 
WORKDIR /app

COPY /out .

ENV ASPNETCORE_URLS http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "MockSite.Web.dll"]

# FROM microsoft/dotnet:2.1-sdk AS build
# RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
# RUN apt-get install -y nodejs

# WORKDIR /app
# COPY *.csproj ./
# RUN dotnet restore

# COPY . ./
# RUN npm install
# RUN npm rebuild node-sass

# RUN dotnet publish -c Release -o out --no-restore


# FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
# WORKDIR /app

# ENV ASPNETCORE_URLS http://+:5001
# EXPOSE 5001

# COPY --from=build /app/out .
# ENTRYPOINT ["dotnet", "Skyline.Sports.Application.Backoffice.Web.dll"]`