const cds = require("@sap/cds");
const { Task, Tasks, quickTask } = require("#cds-models/TasksService");

module.exports = class TasksService extends cds.ApplicationService {
  async init() {
    this.on(quickTask, async (req) => {
      const numberOfTasks =
        req.data.numberOfTasks <= 0 ? 1 : req.data.numberOfTasks;
      const currentTaskCount = (
        await SELECT.one.from(Tasks).columns("count(*) as count")
      ).count;
      if (req.data.numberOfTasks > 1) {
        const newTasks = [];
        for (let i = 0; i < numberOfTasks; i++) {
          newTasks.push({
            name: `${req.data.namePrefix} [${currentTaskCount + i + 1}]`,
            delay: req.data.delay,
            status: "NEW",
            createdAt: new Date(),
            changedAt: new Date(),
          });
        }
        await INSERT.into(Tasks).entries(newTasks);
      } else {
        await INSERT.into(Tasks).entries({
          name: `${req.data.namePrefix} [${currentTaskCount + 1}]`,
          delay: req.data.delay,
          status: "NEW",
        });
      }
    });
    return super.init();
  }
};
