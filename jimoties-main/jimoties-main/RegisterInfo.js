// ページ読み込み時にユーザーデータをセッションストレージから取得して表示
window.onload = function() {
    console.log("ページロード完了 - セッションストレージからデータを取得開始");

    const userId = sessionStorage.getItem('user_ID');
    const userName = sessionStorage.getItem('userName');
    const mail = sessionStorage.getItem('mail');  // 郵便番号をmailとして取得
    const address = sessionStorage.getItem('address');
    const phone = sessionStorage.getItem('telephone');
    const email = sessionStorage.getItem('email');
    const password = sessionStorage.getItem('userPassword');  // パスワードも取得

    console.log("取得したユーザーデータ:", { userId, userName, mail, address, phone, email, password });

    if (userId) {
        // セッションストレージから取得したデータをフォームに反映
        document.getElementById('name-edit').value = userName;
        document.getElementById('email-edit').value = email;
        document.getElementById('address-edit').value = address;
        document.getElementById('phone-edit').value = phone;
        document.getElementById('password-edit').value = password;  // パスワードもフォームに反映
        document.getElementById('zip-edit').value = mail;  // 郵便番号もフォームに反映
        console.log("フォームにデータを設定完了");
    } else {
        console.log("ログイン情報がありません。");
    }
};

// 保存ボタンが押されたときの処理
function saveChanges() {
    console.log("保存ボタンが押されました");

    const userId = sessionStorage.getItem('user_ID');
    
    if (userId) {
        console.log("ログイン情報あり - ユーザID:", userId);

        // 既存データを取得
        const currentName = sessionStorage.getItem('userName');
        const currentMail = sessionStorage.getItem('mail');
        const currentAddress = sessionStorage.getItem('address');
        const currentPhone = sessionStorage.getItem('telephone');
        const currentEmail = sessionStorage.getItem('email');
        const currentPassword = sessionStorage.getItem('userPassword');

        // 編集したデータを取得
        const name = document.getElementById('name-edit').value;
        const mail = document.getElementById('zip-edit').value;  // 郵便番号をmailとして扱う
        const address = document.getElementById('address-edit').value;
        const phone = document.getElementById('phone-edit').value;
        const email = document.getElementById('email-edit').value;
        const password = document.getElementById('password-edit').value;

        console.log("取得した編集データ:", { name, mail, address, phone, email, password });

        // フィールドごとに変更があった場合のみ赤文字クラスを追加
        if (name !== currentName) {
            document.getElementById('name-edit').classList.add('updated');
        }
        if (mail !== currentMail) {
            document.getElementById('zip-edit').classList.add('updated');
        }
        if (address !== currentAddress) {
            document.getElementById('address-edit').classList.add('updated');
        }
        if (phone !== currentPhone) {
            document.getElementById('phone-edit').classList.add('updated');
        }
        if (email !== currentEmail) {
            document.getElementById('email-edit').classList.add('updated');
        }
        if (password !== currentPassword) {
            document.getElementById('password-edit').classList.add('updated');
        }

        // セッションストレージにも保存（パスワードが更新された場合）
        sessionStorage.setItem('userName', name);
        sessionStorage.setItem('mail', mail);  // 郵便番号をmailとして保存
        sessionStorage.setItem('address', address);
        sessionStorage.setItem('telephone', phone);
        sessionStorage.setItem('email', email);
        sessionStorage.setItem('userPassword', password);  // パスワードをセッションストレージに再保存
        console.log("セッションストレージにデータを保存完了");

        // 更新データをAPIに送信
        const bodyData = JSON.stringify({
            user_ID: userId,
            user_name: name,
            mail: mail,  // APIでの郵便番号はmailとして扱う
            address: address,
            telephone: phone,
            email: email,
            user_password: password
        });

        console.log("送信データ:", bodyData);

        fetch('https://m3h-nishizawa-jimotiesapi.azurewebsites.net/api/UpdateUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: bodyData
        })
        .then(response => {
            console.log("APIレスポンスステータス:", response.status);
            return response.json();  // レスポンスをJSON形式で受け取る
        })
        .then(data => {
            console.log("APIレスポンスデータ:", data);
            if (data.status === "Success") {
                Swal.fire({  // SweetAlert2を使用して通知
                    title: '更新完了',
                    text: '個人情報が正常に更新されました。',
                    icon: 'success',
                    confirmButtonText: 'OK'
                });
            } else {
                Swal.fire({
                    title: '更新失敗',
                    text: '個人情報の更新に失敗しました。',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error("APIリクエストエラー:", error);
            Swal.fire({
                title: 'エラー',
                text: 'APIリクエスト中にエラーが発生しました。',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        });

    } else {
        console.log("ログイン情報が見つかりません。");
        Swal.fire({
            title: 'ログインエラー',
            text: 'ログイン情報が見つかりません。再度ログインしてください。',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
    }
}