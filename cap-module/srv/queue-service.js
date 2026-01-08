const cds = require("@sap/cds");
// const { Tasks } = require("#cds-models/demo");
const {
  Task,
  Tasks,
  completeOldestTask,
  newTask,
  removeTasks,
  reserveOpenTask,
  cancelTasks,
} = require("#cds-models/QueueService");

module.exports = class QueueService extends cds.ApplicationService {
  #logger = cds.log("queue-service");
  init() {
    this.on(reserveOpenTask, async (req) => {
      const nextTask = await SELECT.one
        .from(Tasks)
        .where({ status: "NEW" })
        .orderBy("createdAt ASC")
        .forUpdate();
      // TODO: prevent reserving tasks if queue is in the downscaling range (<= 3 NEW tasks)
      if (nextTask) {
        // set task to in processing
        await UPDATE(Tasks, nextTask.ID).set({
          status: "PROCESSING",
          processingInstance: req.data.instanceIndex,
        });
        this.#logger.info(`Task with ID ${nextTask.ID} is now in processing.`);
        return { ID: nextTask.ID, name: nextTask.name, delay: nextTask.delay };
      } else {
        this.#logger.info("No open tasks available to reserve.");
        return null;
      }
    });

    this.on(cancelTasks, async (req) => {
      this.#logger.info(
        "Cancelling all tasks that are being processed by instance " +
          req.data.appIndex
      );
      await UPDATE(Tasks)
        .where({ processingInstance: req.data.appIndex, status: "PROCESSING" })
        .set({ status: "CANCELLED" });
    });

    this.on(Task.actions.complete, async (req) => {
      await UPDATE(req.subject).set({ status: "COMPLETED" });
    });

    this.on(newTask, async (req) => {
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

    this.on(removeTasks, async (_) => {
      return DELETE.from(Tasks);
    });

    this.on(completeOldestTask, async (req) => {
      const task = await SELECT.one
        .from(Tasks)
        .orderBy("createdAt ASC")
        .where({ status: ["NEW", "PROCESSING"] });
      if (!task) {
        req.reject(404, `Task with name '${req.data.name}' not found.`);
      }
      await UPDATE(Tasks, task.ID).set({ status: "COMPLETED" });
    });

    return super.init();
  }
};
