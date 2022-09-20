import service from '../utils/request';

export function getAllAgent(parameters) {
    return service({
        url: '/agentApp/getAllAgent',
        method: 'post',
        data: parameters
    })
}

export function getAll(parameters) {
    return service({
        url: '/agentApp/getAll',
        method: 'post',
        data: parameters
    })
}

export function get(parameters) {
    return service({
        url: '/agentApp/get',
        method: 'post',
        data: parameters
    })
}

export function insert(parameters) {
    return service({
        url: '/agentApp/insert',
        method: 'post',
        data: parameters
    })
}

export function remove(parameters) {
    return service({
        url: '/agentApp/delete',
        method: 'post',
        data: parameters
    })
}

export function update(parameters) {
    return service({
        url: '/agentApp/update',
        method: 'post',
        data: parameters
    })
}