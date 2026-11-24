# Первый запуск бэкенда

Нужны Git, .NET SDK 10, Docker Engine и Docker Compose. После клонирования все команды выполняются из корня репозитория `underekat`

## Подготовка

Установить зависимости проекта и `dotnet ef`:

```bash
dotnet restore UnderEkat.slnx
dotnet tool restore
```

Создать локальный файл настроек Docker:

```bash
cp .env.example .env
```

В `.env` заполнить `POSTGRES_PASSWORD` своим паролем

## БД и подключение

```bash
docker compose up -d --wait
```

Docker создаст PostgreSQL: адрес `localhost:5432`, база и пользователь `underekat` без таблиц с данными

Сохранить строку подключения API, заменив POSTGRES_PASSWORD в команде его значением из файла .env:

```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=underekat;Username=underekat;Password=POSTGRES_PASSWORD" --project UnderEkat.Api.csproj
```

## Миграции

Применить миграции, которые уже есть в репозитории, чтобы создать таблицы:

```bash
dotnet ef database update --project UnderEkat.Api.csproj -- --environment Development
```

Применить миграции нужно при первом запуске после клонирования и при получении новых миграций из Git

## Импорт мест из файла

```bash
dotnet run --project UnderEkat.Api.csproj --launch-profile http -- import data/import/places.json
```

## Запуск API

```bash
dotnet run --project UnderEkat.Api.csproj --launch-profile http
```

При следующих запусках достаточно поднять БД и API. Настройки, миграции и импорт повторять каждый раз не нужно, данные сохраняются в Docker volume

## Изменение структуры БД

После изменения структуры хранения создать миграцию, проверить ее код и применить:

```bash
dotnet ef migrations add НазваниеИзменения --project UnderEkat.Api.csproj --output-dir Infrastructure/Migrations -- --environment Development
dotnet ef database update --project UnderEkat.Api.csproj -- --environment Development
```
