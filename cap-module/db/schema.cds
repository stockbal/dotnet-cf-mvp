using {
    cuid,
    managed
} from '@sap/cds/common';

namespace demo;

@assert.unique: {unique: [name]}
entity Tasks : cuid, managed {
    name   : String(40);
    status : String(15) enum {
        NEW;
        PROCESSING;
        COMPLETED;
        FAILED;
    }
}
