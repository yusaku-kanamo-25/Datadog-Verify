new Vue({
  el: '#app',
  vuetify: new Vuetify(),
  data() {
    return {
      selectedDate: '', // カレンダーから選択された日付
      dateMenu: false, // カレンダー表示のトグル
      searchResults: [], // 検索結果を格納する配列
      userNames: {}, // User_IDとUser_nameのマッピング
      editingItem: null // 編集中の商品
    };
  },
  methods: {
    async searchBuyByDate() {
      try {
        console.log('検索開始: ', this.selectedDate);
        const param = { Buy_date: this.selectedDate };

        console.log('API呼び出し開始');
        const response = await axios.post('https://m3h-minaki-jimoties.azurewebsites.net/api/SearchBuyByDate', param);

        console.log('API呼び出し成功: ', response.data);
        this.searchResults = response.data.BuyTableList || [];
        console.log('検索結果: ', this.searchResults);

        // User_IDを使ってListUser APIを呼び出す
        await this.fetchUserDetails();
      } catch (error) {
        console.error('検索に失敗しました:', error);
        if (error.response) {
          console.error('レスポンスエラー: ', error.response.status, error.response.data);
        } else if (error.request) {
          console.error('リクエストエラー: 応答がありません。', error.request);
        } else {
          console.error('設定エラー: ', error.message);
        }
        console.error('エラーデバッグ情報: ', error.config);
      }
    },

   async fetchUserDetails() {
  try {
    console.log('ユーザー情報の取得開始');
    
    // 検索結果から全てのUser_IDを抽出
    const userIds = [...new Set(this.searchResults.map(item => item.User_ID))];
    console.log('抽出したUser_ID: ', userIds);

    // User_IDごとにListUser APIを呼び出してユーザー情報を取得
    const userPromises = userIds.map(userId => 
      axios.get(`https://m3h-minaki-jimoties.azurewebsites.net/api/ListUser?User_ID=${userId}`)
    );
    const userResponses = await Promise.all(userPromises);
    
    console.log('ListUser API呼び出し成功');
    
    // User_IDとユーザー情報のマッピングを作成
    const userDetails = {};
    userResponses.forEach(response => {
      console.log('ListUser APIのレスポンス: ', response.data);
      const users = response.data.List || [];
      users.forEach(user => {
        console.log(`User_ID: ${user.User_ID}, User_name: ${user.User_name}`);
        // ユーザー情報のマッピングを追加
        userDetails[user.User_ID] = {
          User_name: user.User_name,
          Mail: user.Mail,
          Address: user.Address,
          Telephone: user.Telephone,
          Email: user.Email
        };
      });
    });

    // マッピングをデータに保存
    this.userDetails = userDetails;
    console.log('ユーザー情報のマッピング: ', this.userDetails);

    // searchResultsにユーザー情報を追加
    this.searchResults = this.searchResults.map(item => ({
      ...item,
      User_name: this.userDetails[item.User_ID]?.User_name || '不明',
      Mail: this.userDetails[item.User_ID]?.Mail || '不明',
      Address: this.userDetails[item.User_ID]?.Address || '不明',
      Telephone: this.userDetails[item.User_ID]?.Telephone || '不明',
      Email: this.userDetails[item.User_ID]?.Email || '不明'
    }));
    console.log('最終的な検索結果: ', this.searchResults);
  } catch (error) {
    console.error('ユーザー情報の取得に失敗しました:', error);
  }
},
goBack() {
  window.location.href = 'EmployeeMenu.html';
}

}});