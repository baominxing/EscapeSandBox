import axios from 'axios'
import { ElMessage } from 'element-plus'

const service = axios.create({
    baseURL: process.env.VUE_APP_BASE_API, // 所有请求的公共地址部分
    timeout: 30000 // 请求超时时间 这里的意思是当请求时间超过5秒还未取得结果时 提示用户请求超时
})

// 请求相关处理 请求拦截 在请求拦截中可以补充请求相关的配置
// interceptors axios的拦截器对象
service.interceptors.request.use(config => {
    // config 请求的所有信息
    // 常见需求：在每一次发送请求的时候，通过请求头把token信息传递给服务器
    const token = localStorage.getItem('token');
    if (token) {
        config.headers['Authorization'] = "Bearer " + token;
    }
    return config;
}, err => {
    // 请求发生错误时的相关处理 抛出错误
    Promise.reject(err)
})

service.interceptors.response.use(res => {
    // 我们一般在这里处理，请求成功后的错误状态码 例如状态码是500，404，403
    // res 是所有相应的信息
    console.log(res)
    return Promise.resolve(res)
}, err => {
    // 服务器响应发生错误时的处理
    // 请求失败:一般我们会做统一的错误提示
    if (err && err.response) {
        let response = err.response;
        // 有响应信息，但是状态码不对，我们根据不同的状态码做不同的提示
        switch (response.status) {
            case 400:
                // ...
                break;
            case 401:
                ElMessage(err.message);
                window.location.href = '/';
                localStorage.clear("token");

                return;

            case 404:
                // ...
                break;
        }
    } else {
        if (err && err.code === 'ECONNABORTED') {
            // 请求超时或者中断
        }
        if (!navigator.onLine) {
            // 断网了
        }
    }
    Promise.reject(err)
})



export default service
