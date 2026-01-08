using {demo} from '../db/schema';

@requires: 'TaskMonitor'
service TasksService {
    @odata.draft.enabled
    entity Tasks as projection on demo.Tasks;

    @Common.SideEffects: {TargetEntities: ['/TasksService.EntityContainer/Tasks']}
    action quickTask(delay: Integer @UI.ParameterDefaultValue: 100 );
}
