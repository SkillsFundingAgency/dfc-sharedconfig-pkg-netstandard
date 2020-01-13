# dfc-sharedconfig-pkg-netstandard

## Introduction

Nuget package for reading and setting shared configuration between projects

## Getting Started
This is a self-contained Visual Studio 2019 solution containing a main project and unit/integration test projects

### Installing

Clone the project and open the solution in Visual Studio 2019.

## List of dependencies

|Item	|Purpose|
|-------|-------|
|Azure Table Storage | Document storage |
|Shared Config |Service to retrieve and store shared configuration documents|

## Local Config Files

Once you have cloned the public repo you need to rename the appsettings files by removing the -template part from the configuration file names listed below.

| Location | Repo Filename | Rename to |
|-------|-------|-------|
| Dfc.SharedConfig.IntegrationTests | appsettings-template.json | appsettings.json |

## Deployments

This package can be used as part of a larger solution that needs to save shared configuration data into table storage.
Using this package will provide a shared central location for various consuming applications to reference shared configuration data eg a common Azure search index name.

To use this package you will need to supply configuration settings from the hosting app.
For a example of how to do this please take a look at the Integration Test that is part of this solution.

To use Azure table storage, you can make the following call:

```
 var services = new ServiceCollection().AddAzureTableSharedConfigService(settings).BuildServiceProvider();
 ```

 To use File storage, you can make the following call:

 ```
 var services = new ServiceCollection().AddFileStorageSharedConfigService(settings).BuildServiceProvider();
 ```