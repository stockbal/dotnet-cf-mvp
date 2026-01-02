using {demo} from '../db/schema';

@requires: 'system-user'
service QueueService {
    action newTask(delay: Integer default-1);
    action completeOldestTask();
    action removeTasks();
    action cancelTasks(appIndex: Integer);

    action reserveOpenTask(instanceIndex: Int32) returns {
        id    : UUID;
        name  : String;
        delay : Integer;
    };

    @readonly
    entity Tasks as projection on demo.Tasks
        actions {
            action complete()
        };
}
