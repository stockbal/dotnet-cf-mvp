@rest
service ProxyService {

    @requires: 'User'
    action executeHello() returns String;

}
