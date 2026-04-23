// jQueryでフォームの切り替えをアニメーションで実装
$('.message a').click(function(){
  $('form').animate({height: "toggle", opacity: "toggle"}, "slow");
});

// ログイン機能
function login() {
 const email = document.getElementById("loginEmail").value;
 const password = document.getElementById("loginPassword").value;
 const resultMessage = document.getElementById("resultMessage");

 // メッセージエリアを初期化
 resultMessage.style.display = 'none';
 resultMessage.textContent = '';
 
 // 未入力チェック
 if (!email || !password) {
   showMessage("メールアドレスとパスワードは必須項目です。", "#f44336");
   return;
 }

 // APIを通じてログインを行う
 fetch("https://m3h-minaki-jimoties.azurewebsites.net/api/UserLogin", {
   method: "POST",
   headers: {
     "Content-Type": "application/json"
   },
   body: JSON.stringify({
     Email: email,
     User_password: password
   })
 })
 .then(response => {
   if (response.status === 200) {
     return response.json();
   } else if (response.status === 401) {
     throw new Error("ログイン失敗: 401 Unauthorized");
   } else {
     return response.text().then(text => { throw new Error(text); });
   }
 })
 .then(data => {
   if (data.users && data.users.length > 0) {
     const user = data.users[0];
   console.log("ユーザー配列を表示",user);
     // セッションストレージにログイン情報を保存
     sessionStorage.setItem('user_ID', user.user_ID);
     sessionStorage.setItem('userName', user.user_name);
     sessionStorage.setItem('email', user.email);
     sessionStorage.setItem('mail', user.mail);
     sessionStorage.setItem('address', user.address);
     sessionStorage.setItem('telephone', user.telephone);
     sessionStorage.setItem('userPassword', password);  // パスワードも保存

     // 直接ホームへリダイレクト
     window.location.href = "index.html";
   } else {
     showMessage("メールアドレスまたはパスワードが間違っています。", "#f44336");
   }
 })
 .catch(error => {
   console.error("エラー:", error);
   showMessage("ログイン中にエラーが発生しました。エラー内容: " + error.message, "#f44336");
 });
}

// メッセージ表示用関数
function showMessage(message, backgroundColor) {
 const resultMessage = document.getElementById("resultMessage");
 resultMessage.textContent = message;
 resultMessage.style.backgroundColor = backgroundColor;
 resultMessage.style.display = 'block';
}

// アカウント作成機能
async function register() {
  const spinner = document.getElementById("loadingSpinner");
  spinner.style.display = "block";

 const user_name = document.getElementById("user_name").value;
 const user_password = document.getElementById("user_password").value;
 const mail = document.getElementById("mail").value;
 const address = document.getElementById("address").value;
 const telephone = document.getElementById("telephone").value;
 const email = document.getElementById("email").value;

 // 未入力チェック
 if (!user_name || !user_password || !email || !mail || !address || !telephone) {
   showMessage("全ての項目は必須です。", "#f44336");
   return;
 }

 // 既存のメールアドレスをチェックする
 try {
   const response = await fetch('https://m3h-minaki-jimoties.azurewebsites.net/api/ListUserId');
   const usersJson = await response.text(); // レスポンスをテキストで取得
   const users = JSON.parse(usersJson.replace('SQL RESULT:', '')); // 前のSQL結果部分を削除してJSONにパース

   // メールアドレスが既に存在するか確認
   const existingUser = users.List.find(user => user.Email === email);

   if (existingUser) {
     showMessage("このメールアドレスは既に登録されています。", "#f44336");
     return;  // 重複している場合は処理を中断
   }
 } catch (error) {
   console.error("エラー:", error);
   showMessage("メールアドレスの重複確認中にエラーが発生しました。", "#f44336");
   return;
 }


 // APIを通じてデータベースに登録する
 fetch("https://m3h-nishizawa-jimotiesapi.azurewebsites.net/api/RegisterUser", {
   method: "POST",
   headers: {
     "Content-Type": "application/json"
   },
   body: JSON.stringify({
     user_name: user_name,
     user_password: user_password,
     mail: mail,
     address: address,
     telephone: telephone,
     email: email
   })
 })
 .then(response => {
   if (response.status === 200 || response.status === 201) {
     return response.json();
   } else {
     return response.text().then(text => { throw new Error(text); });
   }
 })
 .then(data => {
   if (data.status === "Success") {
     
     fetch("https://m3h-minaki-jimoties.azurewebsites.net/api/ListUserId")
 .then(response => response.text())
.then(text => {
   console.log("取得した生データ:", text);
   
       let data;
   try {
     // テキストをJSONとしてパース
     data = JSON.parse(text);
   } catch (error) {
     console.error("JSONのパースに失敗しました:", error);
     throw new Error("APIからのレスポンスが無効なJSON形式です。");
   }
       
   // 必要な部分を抽出・整形
   const userList = data.List || []; // "List"部分だけ取得する
   console.log("整形されたユーザーリスト:", userList);
   
   // メールアドレスで一致するユーザーを検索
   const matchedUser = userList.find(user => user.Email === email);
       
       if (matchedUser) {
     sessionStorage.setItem('user_ID', matchedUser.User_ID);
console.log("User_IDが正常に保存されました。", matchedUser.User_ID);
     // セッションストレージにユーザー情報を保存
     sessionStorage.setItem('userName', user_name);
     sessionStorage.setItem('email', email);
     sessionStorage.setItem('mail', mail);
     sessionStorage.setItem('address', address);
     sessionStorage.setItem('telephone', telephone);
     sessionStorage.setItem('userPassword', user_password);  // パスワードも保存

     // 2秒後に「ご登録ありがとうございましたページ」にリダイレクト
     setTimeout(() => {
       console.log("静甲");
       window.location.href = "Thankyou.html";
     }, 2000);
         
         } else {
     console.error("一致するユーザーが見つかりませんでした。");
   }
 })
   } else {
     showMessage("アカウント作成に失敗しました。", "#f44336");
   }
 })
 .catch(error => {
   console.error("エラー:", error);
   showMessage("アカウント作成中にエラーが発生しました。エラー内容: " + error.message, "#f44336");
 });
}

// セッションストレージに保存されたユーザー情報を取得
function loadUserData() {
 const userId = sessionStorage.getItem('user_ID');
 const userName = sessionStorage.getItem('userName');
 const email = sessionStorage.getItem('email');
 const mail = sessionStorage.getItem('mail');
 const address = sessionStorage.getItem('address');
 const telephone = sessionStorage.getItem('telephone');
 const userPassword = sessionStorage.getItem('userPassword');  // パスワードも取得
 
 if (userId) {
   console.log("現在のユーザー情報:", {
     userId,
     userName,
     email,
     mail,
     address,
     telephone,
     userPassword  // パスワードを表示
   });
 } else {
   console.log("ログイン情報がありません。");
 }
}

// ページ読み込み時にユーザーデータを確認
window.onload = loadUserData;