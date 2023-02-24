# BATCAM FX C# Example 

한국어 README는 [여기](README-KR.md)에서 확인할 수 있습니다.<br>
日本語のREADMEは[こちら](README-JP.md)

## Summary

This repository provides an example of a WebSocket connection required for BATCAM FX development, and method for visualizing BF Data sent from BATCAM FX.

Please refer to the code with provided documentation. 

## Execute

Requires a PC with .NET SDK and Rider or Visual Studio installed.

Open `batcam-fx-csharp-example.sln` project solution file,
use 'NuGet Package Restore' for restoring NuGet Libraries if needed.

Before executing the code, you should edit parameters in `MainWindow.xaml.cs`.

```csharp
private const string CameraIp = "";
private const string Username = "";
private const string Password = "";
```

Edit above information with provided username, password and camera ip address,
execute code with following options:

* Run or Debug on Visual Studio
* Run or Debug on Rider

## Limitation

The example code provided in this repository has following limitations:

* `ToWritableBitmap` blocks UI thread in OpenCVSharp4 WPF Extension

## Development Environment

* Windows 11 Version 22H2 ( OS Build: 22621.1265 )
* Visual Studio 2022 Enterprise
* Rider 2022.3.2
* .NET SDK 7.0 with Windows Presentation Foundation