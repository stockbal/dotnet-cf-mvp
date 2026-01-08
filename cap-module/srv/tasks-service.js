const cds = require("@sap/cds");
const { Task, Tasks, quickTask } = require("#cds-models/TasksService");

module.exports = class TasksService extends cds.ApplicationService {
  async init() {
    this.on(quickTask, async (req) => {
      await INSERT.into(Tasks).entries({
        name: `New Task [${
          (
            await SELECT.one.from(Tasks).columns("count(*) as count")
          ).count
        }]`,
        delay: req.data.delay,
        status: "NEW",
      });
    });
    return super.init();
  }
};
