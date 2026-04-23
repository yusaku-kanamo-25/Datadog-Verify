new Vue({
  el: '#app',
  vuetify: new Vuetify(),
  methods: {
    goToHome() {
      // ホームページに遷移する処理（例としてホームのURLを指定）
      window.location.href = 'index.html';
    },
    goToPurchaseHistory() {
      // 購入履歴ページに遷移
      window.location.href = 'BuyHistory.html';
    },
    goToPersonalInfo() {
      // 個人情報編集ページに遷移
      window.location.href = 'RegisterInfo.html';
    },
    goToCart() {
      // カートページに遷移
      window.location.href = 'Shoplist.html';
    },
    goToFavorites() {
      // お気に入りページに遷移
      window.location.href = 'Favorite.html';
    },
   
  }
});
