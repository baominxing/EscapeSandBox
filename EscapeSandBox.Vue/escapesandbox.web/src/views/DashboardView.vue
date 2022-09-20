<template>


  <el-button type="primary" class="btn" @click="getAll">
    查询
  </el-button>
  <el-button type="primary" class="btn" @click="dialogFormVisible = true; this.form.id = 0;">
    新增
  </el-button>
  <el-button type="primary" class="btn" @click="loginOut">
    登出
  </el-button>

  <el-label>你好：{{this.$store.state.user}}</el-label>

  <el-table :data="tableData" style="width: 100%; margin-top: 5%;">
    <el-table-column fixed="left" label="操作" width="250">
      <template #default="scope">
        <el-button size="mini" @click="dialogFormVisible = true; form = scope.row;">编辑</el-button>
        <el-button size="mini" type="danger" @click="remove(scope.$index, scope.row)">删除</el-button>
      </template>
    </el-table-column>
    <el-table-column prop="code" label="注册用户"> </el-table-column>
    <el-table-column prop="ipAddress" label="注册Ip地址"> </el-table-column>
    <el-table-column prop="applicationName" label="应用程序名称"> </el-table-column>
    <el-table-column prop="applicationPort" label="应用程序端口"> </el-table-column>
    <el-table-column prop="proxyPort" label="代理端口"> </el-table-column>
  </el-table>

  <el-dialog title="新增/编辑" v-model="dialogFormVisible">
    <el-form :model="form">
      <el-form-item label="注册用户" :label-width="formLabelWidth">
        <el-select v-model="form.code" style="width:100%">
          <el-option v-for="item in options" :key="item.code" :label="item.code" :value="item.code">
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item label="应用程序名称" :label-width="formLabelWidth">
        <el-input v-model="form.applicationName" autocomplete="off"></el-input>
      </el-form-item>


      <el-form-item label="应用程序端口" :label-width="formLabelWidth">
        <el-input-number v-model="form.applicationPort" autocomplete="off" :min="0" :max="65535"></el-input-number>
      </el-form-item>

      <el-form-item label="代理端口" :label-width="formLabelWidth">
        <el-input-number v-model="form.proxyPort" autocomplete="off" :min="41000" :max="46000"></el-input-number>
      </el-form-item>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="insertOrUpdate">保 存</el-button>
      </span>
    </template>
  </el-dialog>

</template>

<script>
  import { ElMessage } from "element-plus";
  import { getAllAgent, getAll, get, insert, remove, update } from '@/api/agentApp'

  export default {
    data() {
      return {
        tableData: [],
        options: [],
        value: '',
        dialogFormVisible: false,
        min: 41000,
        max: 46000,
        form: {
          id: 0,
          code: '',
          ipAddress: '',
          applicationName: '',
          applicationPort: 0,
          proxyPort: 0,
        },
        formLabelWidth: '120px',
      }
    },
    created() {
      this.getAll();
      this.getAllAgent();
    },
    methods: {

      getAllAgent() {

        var parameters = { code: this.$store.state.user, ipAddress: "" };

        getAllAgent(parameters)
          .then(r => {

            var result = r.data;

            if (result.status == this.$store.state.responseStatus.success) {

              this.options = result.data;

            } else {
              ElMessage("获取记录失败:" + result.message);
            }
          })
          .catch(e => {
            ElMessage("获取记录失败:" + e);
          });
      },
      getAll() {

        var parameters = { code: this.$store.state.user, ipAddress: "" };

        getAll(parameters)
          .then(r => {

            var result = r.data;

            if (result.status == this.$store.state.responseStatus.success) {
              this.tableData = result.data;
            } else {
              ElMessage("获取记录失败:" + result.message);
            }
          })
          .catch(e => {
            ElMessage("获取记录失败:" + e);
          });
      },
      get() {

        get()
          .then(r => {

            var result = r.data;

            if (result.status == this.$store.state.responseStatus.success) {
              this.tableData = result.data;
            } else {
              ElMessage("获取记录失败:" + result.message);
            }
          })
          .catch(e => {
            ElMessage("获取记录失败:" + e);
          });
      },
      insertOrUpdate() {

        var parameters = this.form;

        if (parameters.id === 0) {
          insert(parameters)
            .then(r => {

              var result = r.data;

              if (result.status == this.$store.state.responseStatus.success) {

                this.dialogFormVisible = false;

                ElMessage("创建成功");

                this.getAll();

              } else {
                ElMessage("创建失败:" + result.message);
              }
            })
            .catch(e => {
              ElMessage("创建失败:" + e);
            });
        } else {
          update(parameters)
            .then(r => {

              var result = r.data;

              if (result.status == this.$store.state.responseStatus.success) {

                this.dialogFormVisible = false;

                ElMessage("更新成功");

                this.getAll();
              } else {
                ElMessage("更新失败:" + result.message);
              }
            })
            .catch(e => {
              ElMessage("更新失败:" + e);
            });
        }

      },
      remove(index, row) {

        var parameters = row;

        remove(parameters)
          .then(r => {

            var result = r.data;

            if (result.status == this.$store.state.responseStatus.success) {
              ElMessage("删除成功");
              this.getAll();
            } else {
              ElMessage("删除失败:" + result.message);
            }
          })
          .catch(e => {
            ElMessage("删除失败:" + e);
          });
      },
      loginOut() {
        // 存储token
        this.$store.commit('loginOut');
        this.$router.push("/");
      },
    },
  }
</script>

<style scoped>
  .btn {
    position: relative;
    left: 40%;
    width: 100px;
    padding: 10px;
  }
</style>