import service from '../utils/request';

export function login(parameters) {
    return service({
        url: '/account/login',
        method: 'post',
        data: parameters
    })
}

