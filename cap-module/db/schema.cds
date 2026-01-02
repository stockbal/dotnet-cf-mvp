using {
    cuid,
    managed
} from '@sap/cds/common';

namespace demo;

@assert.unique: {unique: [name]}
entity Tasks : cuid, managed {
    name               : String(40);
    delay              : Integer;
    status             : String(15) enum {
        NEW;
        PROCESSING;
        CANCELLED;
        COMPLETED;
        FAILED;
    };
    processingInstance : Int32;
}
