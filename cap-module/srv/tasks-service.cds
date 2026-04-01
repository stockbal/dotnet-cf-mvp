using {demo} from '../db/schema';

@requires: 'TaskMonitor'
service TasksService {
    @odata.draft.enabled
    entity Tasks as projection on demo.Tasks;

    @Common.SideEffects: {TargetEntities: ['/TasksService.EntityContainer/Tasks']}
    action quickTask(namePrefix: String  @UI.ParameterDefaultValue: 'New Task'  @mandatory  @title: 'Name Prefix',  delay: Integer  @title: 'Delay'  @UI.ParameterDefaultValue: 100,  numberOfTasks: Integer  @title: 'Number of Tasks'  @UI.ParameterDefaultValue: 1  );
}
