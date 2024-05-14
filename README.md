# .NET Core Demo project for MTA deployment to Cloud Foundry

This demo project shows a very simple ASP.NET Web API service that returns hello world. It is secured with XSUAA from SAP BTP and can be deployed via `mta.yaml` to SAP BTP Cloud Foundry Environment.  
It can be used as a template to build more complex multi-target that require a .NET module.

## Securing your application with XSUAA

There is no standard NuGet package available (e.g. `@sap/xssec` for CAP NodeJS) to easily connect to the defined security configuration in `xs-security.json`, so this has to be done manually in the project.

Have a look in the [Authentication](./dotnet-module/src/DemoService/Authentication/ConfigureJwtBearerOptions.cs) folder to see how this can be done.

| :exclamation:  The implementation is reverse engineered from [@sap/xssec](https://www.npmjs.com/package/@sap/xssec). Use at your own risk :smile:   :exclamation: |
|-----------------------------------------|

## Specifics for .NET Core MTA deployment

### MTA.yaml

A workaround for the builder is required, as `dotnet_core` is no longer a recognized and valid application type.

In this sample we use the `nodejs` module and overwrite the builder with a dummy `echo` command. As we use the source

```yaml
modules:
  - name: my-dotnet-service-module
    type: nodejs
    path: <path-to-folder-with-.net-solution>
    build-parameters:
      builder: custom
      commands:
        - echo ">> Building .NET module"
    ...
```

Additionally, the *online* build pack for .NET has been removed in Nov 16, 2023 (SAP note 3364781), so we need to specify a valid one in the `parameters` section of the module.  
The repository [dotnet-core-buildpack](https://github.com/cloudfoundry/dotnet-core-buildpack) in the Cloud Foundry org holds different releases. If you require a certain stack (i.e. concrete .NET Core version) you can use a specific release by adding the release tag after the repository url.

```yaml
    ...
    parameters:
      buildpack: https://github.com/cloudfoundry/dotnet-core-buildpack.git#v2.4.24
```

### Setting buildpack options

To define certain options for the used buildpack we need to create an additional file called `buildpack.yml` in the root path of the .NET module.

Here we can set the .NET version to be used:

```yaml
---
dotnet-core:
  sdk: 8.0
```

### Defining your main .NET project

If you have several projects in your module you need to specify your main project in a file `.deployment` at root level of the .NET module:

```ini
[config]
project = src/DemoService/DemoService.csproj
```

## Links

- [Cloud Foundry docs | .NET Buildpack](https://docs.cloudfoundry.org/buildpacks/dotnet-core/index.html)
