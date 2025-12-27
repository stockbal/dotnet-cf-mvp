# .NET Core demo project for MTA deployment to Cloud Foundry

This branch showcases a background polling service in .NET which calls an endpoint in the CAP application to reserve open tasks.
The CAP application sends a custom metric - the number of tasks with status `NEW` - to the [SAP Application Autoscaler](https://help.sap.com/docs/application-autoscaler/application-autoscaler/defining-custom-metric?locale=en-US) instance to increase the number of .NET instances according the tasks in the *queue*.

## Remarks

- The downscaling needs to be revisited as it could happen that an instance is removed whilst still processing a task.
