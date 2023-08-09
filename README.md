# AWSFacade
AWSFacade is a face of AWSSDK.


| Package |  Version | Downloads |
| ------- | ----- | ----- |
| `AWSFacade` | [![NuGet](https://img.shields.io/nuget/v/AWSFacade.svg)](https://nuget.org/packages/AWSFacade) | [![Nuget](https://img.shields.io/nuget/dt/AWSFacade.svg)](https://nuget.org/packages/AWSFacade) |


### Dependencies
.NET Standard 2.1

You can check supported frameworks here:

https://learn.microsoft.com/pt-br/dotnet/standard/net-standard?tabs=net-standard-2-1

### Instalation
This package is available through Nuget Packages: https://www.nuget.org/packages/AWSFacade


**Nuget**
```
Install-Package AWSFacade
```

**.NET CLI**
```
dotnet add package AWSFacade
```

# How to use
## Add Dependency Injection
### Basic
```csharp

using AWSFacade.SQS.Contracts;
using AWSFacade.SQS.Extensions;

services.AddSqsFacade(config =>
{
    config.MessageGroupId = "YOUR_GROUP_ID";
    config.QueueUrl = "YOUR_QUEUE_URL";
    config.RegionEndpoint = RegionEndpoint.SAEast1;
});

```
### With Identification Name
```csharp

using AWSFacade.SQS.Contracts;
using AWSFacade.SQS.Extensions;

services.AddSqsFacade("SQS_CUSTOMER", config =>
{
    config.MessageGroupId = "YOUR_GROUP_ID";
    config.QueueUrl = "YOUR_QUEUE_URL";
    config.RegionEndpoint = RegionEndpoint.SAEast1;
});

```
### Usage example A
```csharp

var sqsFacadeFactory = serviceProvider.GetService<ISqsFacadeFactory>();
var sqsFacade = sqsFacadeFactory!.Create();

```
### Usage example B
```csharp

public class Customer
{
    private readonly ISqsFacade _sqsFacade;
    public Customer(ISqsFacadeFactory sqsFacadeFactory)
    {
        _sqsFacade = sqsFacadeFactory.Create();
    }
}

```
### Usage example C
```csharp

public class Customer
{
    private readonly ISqsFacade _sqsFacade;
    public Customer(ISqsFacadeFactory sqsFacadeFactory)
    {
        _sqsFacade = sqsFacadeFactory.Create("SQS_CUSTOMER");
    }
}

```
