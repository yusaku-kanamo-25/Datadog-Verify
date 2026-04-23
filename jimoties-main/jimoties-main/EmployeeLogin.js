new Vue({
  el: '#app',
  vuetify: new Vuetify(),
  data() {
    return {
      employee_ID: '', 
      employee_password: '' 
    };
  },
  methods: {
    async login() { 
      try {
        const response = await axios.post('https://m3h-minaki-jimoties.azurewebsites.net/api/EmployeeLogin', { 
          Employee_ID: this.employee_ID, 
          Employee_password: this.employee_password 
        });

        if (response.status === 200) { 
          // レスポンスのステータスコードが200の場合,成功
          sessionStorage.setItem('employee_ID', this.employee_ID); 
         // ⇓完成後ファイル名指定を行う場所
          window.location.href = 'EmployeeMenu.html'; 
        }
      } catch (error) {
        console.error('Login failed', error); 
        // ログイン失敗時のエラーログを出力
        alert('ログインに失敗しました。IDまたはパスワードを確認してください。'); 
      }
    }
  }
});