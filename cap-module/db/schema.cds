using {
    cuid,
    managed
} from '@sap/cds/common';

namespace demo;

@assert.unique: {unique: [name]}
entity Tasks : cuid, managed {
    name               : String(40);
    delay              : Integer;
    duration           : Integer = case
                                       when status    = #COMPLETED
                                            or status = #CANCELLED
                                            or status = #FAILED
                                            then seconds_between(
                                                     createdAt, modifiedAt
                                                 )
                                       else 0
                                   end stored;
    status             : String(15) @changelog enum {
        NEW;
        PROCESSING;
        CANCELLED;
        COMPLETED;
        FAILED;
    };
    processingInstance : Int32;
}
