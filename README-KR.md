# BATCAM FX C# Example 

## 개요

이 레포지터리는 BATCAM FX 개발에 필요한 WebSocket 연결과 BATCAM FX 에서 전달되는 BF Data 를 시각화하는 메소드를 포함하는 예제입니다.

제공된 문서와 같이 코드를 참조하시기 바랍니다.

## 실행

.NET SDK 및 Rider 또는 Visual Studio 가 설치된 PC가 필요합니다. 

프로젝트 파일인 `batcam-fx-csharp-example.sln` 파일을 열고, 
필요한 경우 Nuget 패키지 복원 기능을 이용하여 설치된 패키지를 복원합니다.

코드를 실행하기 전에, `MainWindow.xaml.cs` 내의 인자들을 수정해야 합니다.

```csharp
private const string CameraIp = "";
private const string Username = "";
private const string Password = "";
```

제공받은 사용자명과 비밀번호, 그리고 카메라의 주소를 입력하고 편한 방법을 통해 예제를 실행합니다.

* Visual Studio 에서 실행 또는 디버그
* Rider 에서 실행 또는 디버그

## 한계

이 레포지터리에서 제공하는 예제 코드는 다음과 같은 한계 사항을 가지고 있습니다:

* OpenCVSharp4 WPF Extension 의 ToWritableBitmap 의 UI 쓰레드 블락 이슈

## 개발 환경

* Windows 11 Version 22H2 ( OS Build: 22621.1265 )
* Visual Studio 2022 Enterprise
* Rider 2022.3.2
* .NET SDK 7.0 with Windows Presentation Foundation