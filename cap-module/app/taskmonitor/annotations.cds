using TasksService as service from '../../srv/tasks-service';

annotate service.Tasks with @( //
    UI.HeaderInfo                  : {
        TypeName      : 'Task',
        TypeNamePlural: 'Tasks',
        Title         : {Value: name}
    },
    UI.LineItem                    : [
        {Value: name},
        {
            Value       : status,
            Criticality : (status = #NEW ? 0 : (status = #FAILED
            or status = #CANCELLED ? 1 : (status = #COMPLETED ? 3 : 5)))
        },
        {Value: duration},
        {Value: delay},
        {Value: processingInstance},
        {Value: createdAt},
        {Value: modifiedAt},
        {
            $Type : 'UI.DataFieldForAction',
            Action: 'TasksService.EntityContainer/quickTask',
            Label : 'Quick Task',

        },
    ],
    UI.Identification              : [
        {Value: name},
        {Value: status},
        {Value: duration},
        {Value: processingInstance}
    ],
    UI.Facets                      : [{
        $Type : 'UI.ReferenceFacet',
        Label : 'General Information',
        Target: '@UI.Identification'
    }],
    Capabilities                   : {FilterRestrictions: {
        $Type                       : 'Capabilities.FilterRestrictionsType',
        FilterExpressionRestrictions: [{
            Property          : createdAt,
            AllowedExpressions: 'SingleRange'
        }]
    }},
    UI.SelectionFields             : [createdAt],
    UI.SelectionPresentationVariant: {
        PresentationVariant: {
            Visualizations: ['@UI.LineItem'],
            SortOrder     : [{
                Property  : createdAt,
                Descending: false
            }]
        },
        SelectionVariant   : {
            $Type        : 'UI.SelectionVariantType',
            SelectOptions: []
        },
    },
);

annotate service.Tasks with {
    createdAt          @UI.HiddenFilter: false;
    name               @title          : 'Name';
    delay              @title          : 'Delay';
    status             @title          : 'Status';
    processingInstance @title          : 'Processed by Instance';
    duration           @title          : 'Duration'
};
