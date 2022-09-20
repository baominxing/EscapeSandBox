import { createStore } from 'vuex'

const store = createStore({
  state: {
    user: '',
    responseStatus: {
      success: 1,
      failure: 0,
    },
    token: ''  //初始化token
  },
  mutations: {
    //存储token方法
    //设置token等于外部传递进来的值
    loginIn(state, info) {
      state.token = info.token
      state.user = info.user
      localStorage.token = info.token //同步存储token至localStorage
    },

    loginOut(state) {
      state.user = "";
      state.token = "";
      localStorage.clear("token");
    },
  },
  getters: {
    //获取token方法
    //判断是否有token,如果没有重新赋值，返回给state的token
    getToken(state) {
      if (!state.token) {
        state.token = localStorage.getItem('token')
      }
      return state.token
    }
  },
  actions: {

  },
  modules: {
  }
});

export default store;
