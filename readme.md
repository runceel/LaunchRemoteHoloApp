# このリポジトリについて

[WindowsDevicePortalWrapper](https://github.com/microsoft/windowsdeviceportalwrapper) を使用して複数台の HoloLens に対して同時に同じアプリを立ち上げるプログラムです。

## 動作

### Manage HoloLens ページ

以下の値を入力して管理対象の HoloLens を追加できます。

- Name: 任意の名前
- Address: 対象の HoloLens の IP アドレス
- User name: HoloLens のデバイスポータルのユーザー名
- Password: HoloLens のデバイスポータルのパスワード

注意：上記情報は平文で保存されます。

### Apps ページ

Manage HoloLens ページで登録した全ての HoloLens にインストールされているアプリケーションのリストです。
例えば HoloA にアプリ A, B, C が入っていて HoloB にアプリ A, C, D が入っている場合にはリストには A, C が表示されます。

リストからアプリケーションの起動が行えます。

ページ上部のフィルタでアプリ名でフィルタリングが出来ます。

### Running processes ページ

HoloLens ごとに起動しているアプリのリストが確認できます。
Apps ページと同様にプロセス名でフィルタリングが出来ます。

## 使用ライブラリ

- System.Reactive
- WindowsDevicePortalWrapper
- Nwetonsoft.Json
