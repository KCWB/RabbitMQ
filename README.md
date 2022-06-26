# RabbitMQ

## Baixando a imagem Docker do RabbitMQ
```
docker pull rabbitmq:3-management
```

## Iniciando o RabbitMQ no terminal
```
docker run -p 8080:15672 -p 5672:5672 -p 25676:25676 rabbitmq:3-management
```

## Criando um novo projeto C# (Produtor ou Consumidor)
```
donet new console –name projeto-nome
dotnet add package RabbitMQ.Client - -version 5.2.0
dotnet  add package NewtonSoft.json
```
