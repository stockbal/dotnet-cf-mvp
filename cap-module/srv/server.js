const cds = require("@sap/cds");
const { Tasks } = require("#cds-models/demo");
const axios = require("axios");
const https = require("https");
const fs = require("fs");

const VCAP_SERVICES = JSON.parse(process.env.VCAP_SERVICES);
const logger = cds.log("server");

cds.on("served", async (_) => {
  cds.spawn({ every: 60000 }, async () => {
    // send metric to application autoscaler
    const openTasks = await SELECT.one
      .from(Tasks)
      .where({ status: "NEW" })
      .columns("count(*) as count");

    // Reload certificate and key on each request to handle automatic rotation
    // Certificates are valid for 24 hours and regenerated 20 minutes before expiration
    const instanceCert = fs.readFileSync(process.env.CF_INSTANCE_CERT, "utf-8");
    const instanceKey = fs.readFileSync(process.env.CF_INSTANCE_KEY, "utf-8");

    // Get mtls_url from autoscaler service binding
    const mtlsUrl =
      VCAP_SERVICES.autoscaler[0].credentials.custom_metrics.mtls_url;
    const appId = process.env.DOTNET_APP_ID;

    try {
      await axios.post(
        `${mtlsUrl}/v1/apps/${appId}/metrics`,
        {
          instance_index: 0,
          metrics: [
            {
              name: "taskqueue",
              value: openTasks.count,
            },
          ],
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
          httpsAgent: new https.Agent({
            cert: instanceCert,
            key: instanceKey,
            rejectUnauthorized: true,
          }),
        }
      );
      logger.info("Successfully sent custom metric to autoscaler", {
        taskCount: openTasks.count,
      });
    } catch (error) {
      logger.error(`Error sending custom metric: ${error.message}`, {
        code: error.code,
        status: error.response?.status,
        stack: error.stack,
      });
    }
  });
});
