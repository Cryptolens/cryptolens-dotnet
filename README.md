# Cryptolens Licensing for .NET

View in other languages (beta): [русский](README.ru.md), [svenska](README.sv.md).

This API serves as a layer that simplifies communication with Cryptolens Web API 2 and 3 (https://app.cryptolens.io/docs/api).

> You can access the documentation of the library at https://help.cryptolens.io/api/dotnet/.

Please check out our guide on common errors during integration: https://help.cryptolens.io/faq/index#troubleshooting-api-errors

## Getting started

### Install Nuget package

In Visual Studio package manager
```
PM> Install-Package Cryptolens.Licensing
```

Using dotnet CLI
```
dotnet add package Cryptolens.Licensing
```

**If you are targeting Mac, Linux or Unity/Mono, we recommend to use the cross platform version of that package.**

In Visual Studio package manager
```
PM> Install-Package Cryptolens.Licensing.CrossPlatform
```

Using dotnet CLI
```
dotnet add package Cryptolens.Licensing.CrossPlatform
```

### Example code
* [Key verification](https://help.cryptolens.io/examples/key-verification)
* [Offline verification](https://help.cryptolens.io/examples/offline-verification)

### Recommended articles

* [Unity 3D / Mono](https://help.cryptolens.io/getting-started/unity)
* [AutoCAD](https://cryptolens.io/2019/01/autocad-plugin-software-licensing/)
* [Rhinoceros / Grasshoper](https://cryptolens.io/2019/01/protecting-rhinoceros-plugins-with-software-licensing/)

## Compatibility

To get access to all of the featues in the library, .NET Framework 4.6 or above has to be used. We have summarized the functionality that is included in each framework. Note, .NET Standard means the library can run on multiple platforms (eg. .NET Core), based on the following [document](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

* **.NET Framework 4.0** - Verifying metadata signatures is not supported.
* **.NET Framework 4.6** - All features supported.
* **.NET Standard 2.0** - Computing machine codes is not supported. You can still use the available hash functions to compute a machine code, assuming you can collect machine specific information.
* **Unity/Linux/Mac** - You need to use the packages in the "Without System.Management" folder on the release page. You can find more info [here](https://help.cryptolens.io/getting-started/unity). 

Cryptolens.Licensing library does also work on Mono (eg. when running on Linux or Unity), however you need to use a special build that does not include System.Management. You can either install it through [NuGet](https://www.nuget.org/packages/Cryptolens.Licensing.CrossPlatform/), download [pre-compiled binaries](https://github.com/Cryptolens/cryptolens-dotnet/releases) (you need to use the library with the name `Cryptolens.Licensing.CrossPlatform`) or compile it yourself using the instructions below.

## Compiling without System.Management (Linux, Unity, Mono)
The easiest way to get the platform independent library is by either installing it through [NuGet](https://www.nuget.org/packages/Cryptolens.Licensing.CrossPlatform/) or downloading [pre-compiled binaries](https://github.com/Cryptolens/cryptolens-dotnet/releases) (the ones that are called `Cryptolens.Licensing.CrossPlatform.dll`). In this section, we describe how to compile it yourself:

To compile without System.Management, open `Cryptolens.Licensing.csproj` and remove the `<DefineConstants>SYSTEM_MANAGEMENT</DefineConstants>` below inside the `<PropertyGroup>` tag, i.e.

```
<PropertyGroup>
    ...

    <DefineConstants>SYSTEM_MANAGEMENT</DefineConstants>   <-- remove this
</PropertyGroup>
```

> Make sure there are no other `DefineConstants` definitions later in the file, as these will override the value.

## Other settings

### Issues with Newtonsoft.Json on .NET 4.8
Some customers have reported an error with the right version of Newtonsoft.Json not being found. It seems to be localized to those that target .NET Framework 4.8, and the following error is shown:

```
System.IO.FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=11.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference.
```

The error is thrown when a wrong version of Newtonsoft.Json is installed in the same project as Cryptolens.Licensing library. To fix this, you need to make sure that Newtonsoft.Json is uninstalled completely and then re-install Cryptolens.Licensing.


### 'System.MethodAccessException' when calling Helpers.GetMachineCode
In some Windows environments (e.g. when developing Excel addins), it might not be feasible to call Helpers.GetMachineCode on .NET Framework 4.6. The reason for this is the call we make to `System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform`. To fix this, we have added a boolean flag in `Helpers` class. Before calling `Helpers.GetMachineCode` or `Helpers.IsOnRightMachine`, please set `Helpers.WindowsOnly=True`.

```cs
Helpers.WindowsOnly = true;
var machineCode = Helpers.GetMachineCode();
```

If the approach above does not work, please try the following call instead:

```cs
var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
```

### Turn off KeepAlive
In order to perform API calls with KeepAlive disabled, please set `HelperMethods.KeepAlive=False`. If you want to remove all references to code that uses KeepAlive that is true, you can compile the library with the `KeepAliveDisabled` flag, which is added the same way as `SYSTEM_MANAGEMENT` mentioned earlier. If you have multiple flags, they can be separated with a semicolon.

### Proxy settings
To change proxy settings, you can use the variable `HelperMethods.proxy`.

## Old examples

### Check Against Time Rollback
In order to make sure that the local time (date and time) wasn't changed by the user, the following code can be used.
```
public void HasLocalTimeChanged()
{
    bool hasChanged = SKGL.SKM.TimeCheck();

    if(hasChanged)
    {
        Debug.WriteLine("The local time was changed by the user. Validation fails.");
    }
    else
    {
        Debug.WriteLine("The local time hasn't been changed. Continue validation.");
    }
}
```

### Calculating Machine code
Machine code can be calculated with the function below. Any other hash algorithm will do, as long as it only contains letters and digits only.
```
public void TestingHashes()
{
    //eg. "61843235" (getEightDigitsLongHash)
    //eg. "D38F13CAB8938AC3C393BC111E1A85BB4BA2CCC9" (getSHA1)
    string machineID1 = SKGL.SKM.getMachineCode(SKGL.SKM.getEightDigitsLongHash);
    string machineID2 = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
}
```
