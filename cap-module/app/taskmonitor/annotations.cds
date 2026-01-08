using TasksService as service from '../../srv/tasks-service';

annotate service.Tasks with @( //
    UI.HeaderInfo    : {
        TypeName      : 'Task',
        TypeNamePlural: 'Tasks',
        Title         : {Value: name}
    },
    UI.LineItem      : [
        {Value: name},
        {
            Value       : status,
            Criticality : (status = #NEW ? 0 : (status = #FAILED
            or status = #CANCELLED ? 1 : (status = #COMPLETED ? 3 : 5)))
        },
        {Value: duration},
        {Value: delay},
        {Value: processingInstance},
        {
            $Type : 'UI.DataFieldForAction',
            Action : 'TasksService.EntityContainer/quickTask',
            Label : 'Quick Task',

        },
    ],
    UI.Identification: [
        {Value: name},
        {Value: status},
        {Value: duration},
        {Value: processingInstance}
    ],
    UI.Facets        : [{
        $Type : 'UI.ReferenceFacet',
        Label : 'General Information',
        Target: '@UI.Identification'
    }]
);

annotate service.Tasks with {
    name               @title: 'Name';
    delay              @title: 'Delay';
    status             @title: 'Status';
    processingInstance @title: 'Processed by Instance';
    duration           @title: 'Duration'
};
