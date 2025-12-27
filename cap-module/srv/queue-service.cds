using {demo} from '../db/schema';

@requires: 'system-user'
service QueueService {
    action newTask();
    action completeOldestTask();
    action removeTasks();

    action reserveOpenTask(instanceIndex: Int32) returns {
        id   : UUID;
        name : String;
    };

    @readonly
    entity Tasks as projection on demo.Tasks
        actions {
            action complete()
        };
}
