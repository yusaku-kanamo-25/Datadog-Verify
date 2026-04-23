new Vue({
  el: '#app',
  vuetify: new Vuetify(),
  methods: {
    logout() {
      sessionStorage.removeItem('employee_ID');
      window.location.href = 'EmployeeLogin.html';
      //window.location.href = 'https://codepen.io/minakichi552/pen/XWLZGrJ?editors=1010'; // ログイン画面のURLに変更
    },
    stockCheck() {
      
      window.location.href = 'EmployeeStock.html';
      //window.location.href = 'https://codepen.io/minakichi552/pen/XWLZGrJ?editors=1010'; // ログイン画面のURLに変更
    },

    itemRegister() {
      
      window.location.href = 'EmployeeRegister.html';
      //window.location.href = 'https://codepen.io/minakichi552/pen/XWLZGrJ?editors=1010'; // ログイン画面のURLに変更
    },

    orderCheck() {
      
      window.location.href = 'EmployeeOrder.html';
      //window.location.href = 'https://codepen.io/minakichi552/pen/XWLZGrJ?editors=1010'; // ログイン画面のURLに変更
    }
  }
});