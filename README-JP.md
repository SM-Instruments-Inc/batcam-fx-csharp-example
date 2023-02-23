# BATCAM FX C# Example

## 概要

このリポジトリは「BATCAM FX」開発に必要なWebSocket接続および、送信されるBF Dataを視覚化するメソットを含む例です。

コード作成時に添付の資料を参考までにお願いいたします。

## 実行

.NET SDKとRiderまたはVisual StudioがインストールされたPCが必要です。

プロジェクトファイルである `batcam-fx-csharp-example.sln` ファイルを開けて、必要に応じて Nuget のパッケージ復元機能を利用してインストールされたパッケージを復元します。

コードを実行する前に、`MainWindow.xaml.cs`内のパラメータを修正する必要があります。

```csharp
private const string CameraIp = "";
private const string Username = "";
private const string Password = "";
```

提供されたユーザー名とパスワード、そしてカメラのアドレスを入力して、簡単な方法で例を実行します。

* Visual Studio で実行またはデバッグ
* Rider で実行またはデバッグ

## 注意

このリポジトリで提供するサンプルコードには以下の限界事項がございます。

* OpenCVSharp4 WPF Extension の ToWritableBitmap の UI スレッドブロック問題

## 開発環境

* Windows 11 Version 22H2 ( OS Build: 22621.1265 )
* Visual Studio 2022 Enterprise
* Rider 2022.3.2
* .NET SDK 7.0 with Windows Presentation Foundation