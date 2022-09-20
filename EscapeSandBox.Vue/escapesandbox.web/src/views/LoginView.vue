<template>
    <div class="layout" :title="title">
        <el-form label-position="right" label-width="60px" style="max-width: 460px" class="loginForm">
            <el-form-item label="账号：">
                <el-input v-model="UsernameOrEmail" />
            </el-form-item>
            <el-form-item label="密码：">
                <el-input type="password" v-model="Password" />
            </el-form-item>
            <el-button type="primary" class="loginBtn" @click="login">
                登录
            </el-button>
        </el-form>
    </div>
</template>
<script>
    import { ElMessage } from "element-plus";
    import { login } from '@/api/account'
    export default {
        name: 'LoginView',
        data() {
            return {
                title: "登录页面",
                UsernameOrEmail: "",
                Password: "",
            };
        },
        created() {

        },
        methods: {
            login() {
                var parameters = {
                    UsernameOrEmail: this.UsernameOrEmail,
                    Password: this.Password
                };

                login(parameters)
                    .then(r => {

                        var result = r.data;

                        if (result.status == this.$store.state.responseStatus.success) {

                            // 存储token

                            var info = {
                                token: result.data.tokenContent, user: this.UsernameOrEmail
                            };

                            this.$store.commit('loginIn', info);

                            this.$router.push("dashboard");
                        } else {
                            ElMessage("登录失败:" + result.message);
                        }
                    })
                    .catch(e => {
                        ElMessage("登录失败:" + e);
                    });
            }
        },
    }
</script>

<style scoped>
    .layout {
        position: absolute;
        left: calc(50% - 200px);
        top: 40%;
        width: 400px;
    }

    .loginBtn {
        position: absolute;
        left: 15%;
        width: 340px;
    }

    .loginForm {
        text-align: center;
    }

    .checkBox {
        margin-left: 7px;
    }

    .inpWidth {
        width: 165px;
    }
</style>