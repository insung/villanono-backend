FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 진단 로그를 비활성화해서 문제 회피
# ENV COMPlus_EnableDiagnostics=0
# ENV COMPlus_EnableEventLog=0

# 1. 프로젝트 파일만 복사 (프로젝트 파일 이름을 실제 파일 이름에 맞게 수정)
COPY ["villanono-backend.csproj", "./"]

# 2. Restore - csproj 파일을 기반으로 패키지 복원
RUN dotnet restore "villanono-backend.csproj" 

# 3. 나머지 소스 코드 복사
COPY . ./

# 4. ARM 타깃으로 publish (예시: linux-arm64, self-contained=false로 프레임워크 종속 배포)
RUN dotnet publish -c Release -o out --self-contained false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "villanono-backend.dll"]