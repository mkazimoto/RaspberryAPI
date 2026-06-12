# RaspberryAPI

API REST minimalista em ASP.NET Core 8 projetada para rodar em Raspberry Pi.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (para desenvolvimento)
- Raspberry Pi com Linux ARM (para deploy)

## Endpoints

| Método | Rota               | Descrição                          |
|--------|--------------------|------------------------------------|
| GET    | `/`                | Status e timestamp da API          |
| GET    | `/weatherforecast` | Previsão do tempo aleatória (demo) |

A API escuta em `http://0.0.0.0:5000`.

## Desenvolvimento

```bash
dotnet run
```

Acesse `http://localhost:5000`.

## Build e Publicação

### Build local

```bash
dotnet build
```

### Publicar para Raspberry Pi (linux-arm, self-contained)

```bash
dotnet publish RaspberryAPI.csproj -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o bin/publish/raspberry
```

Ou use a task configurada no VS Code: **publish-raspberry**.

## Deploy no Raspberry Pi

1. Certifique-se de que o diretório de destino existe no Pi:

```bash
ssh pi@raspberrypi "mkdir -p ~/api"
```

2. Copie os arquivos publicados:

```bash
scp -r bin\publish\raspberry\* pi@raspberrypi:~/api/
```

3. No Raspberry Pi, dê permissão de execução:

```bash
ssh pi@raspberrypi
chmod +x ~/api/RaspberryAPI
```

A API ficará acessível em `http://<ip-do-raspberry>:5000`.

## Rodar como Serviço (systemd)

Para que a API inicie automaticamente com o Raspberry Pi, configure um serviço systemd.

1. Crie o arquivo de serviço:

```bash
sudo nano /etc/systemd/system/raspberryapi.service
```

2. Cole o conteúdo abaixo (ajuste `User` e caminhos se necessário):

```ini
[Unit]
Description=RaspberryAPI - ASP.NET Core API
After=network.target

[Service]
WorkingDirectory=/home/pi/api
ExecStart=/home/pi/api/RaspberryAPI
Restart=always
RestartSec=10
KillSignal=SIGINT
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

3. Ative e inicie o serviço:

```bash
# Recarrega os arquivos de serviço
sudo systemctl daemon-reload

# Habilita para iniciar automaticamente no boot
sudo systemctl enable raspberryapi

# Inicia o serviço imediatamente
sudo systemctl start raspberryapi
```

4. Verifique o status e os logs:

```bash
sudo systemctl status raspberryapi

# Logs em tempo real
journalctl -u raspberryapi -f
```

## Estrutura do Projeto

```
RaspberryAPI/
├── Program.cs              # Configuração e endpoints da API
├── RaspberryAPI.csproj     # Definições do projeto
├── appsettings.json        # Configurações gerais
├── appsettings.Development.json
└── Properties/
    ├── launchSettings.json
    └── PublishProfiles/
        └── RaspberryPi.pubxml
```
