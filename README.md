# AWSFacade
AWSFacade is a face of AWSSDK.


| Package |  Version | Downloads |
| ------- | ----- | ----- |
| `AWSFacade` | [![NuGet](https://img.shields.io/nuget/v/AWSFacade.svg)](https://nuget.org/packages/AWSFacade) | [![Nuget](https://img.shields.io/nuget/dt/AWSFacade.svg)](https://nuget.org/packages/AWSFacade) |


### Dependencies
.NET 6.0

You can check supported frameworks here:

https://learn.microsoft.com/pt-br/dotnet/api/?view=net-6.0

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

## How to use
```csharp

string jsonString = "{ \"name\":\"product a\" }";
AWSFacade.PublishMessage(jsonString);

```
