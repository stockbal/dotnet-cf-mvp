const cds = require("@sap/cds");

module.exports = class ProxyService extends cds.ApplicationService {
  init() {
    this.on("executeHello", async () => {
      const extSrv = await cds.connect.to("DOTNET_API");
      return extSrv.get("/HelloWorld");
    });

    return super.init();
  }
};
