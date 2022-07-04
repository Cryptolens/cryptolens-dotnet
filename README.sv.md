# Cryptolens Licensing för .NET

Detta bibliotek är ett interface för Cryptolens Web API (https://app.cryptolens.io/docs/api). Med biblioteket går det att bl.a. verifiera licenser.

> Dokumentationen för biblioteket finns på https://help.cryptolens.io/api/dotnet/.

Läs gärna följande guide om vanliga felmeddelanden: https://help.cryptolens.io/faq/index#troubleshooting-api-errors

## Komma igång

### Installera Nuget package

I Visual Studio package manager
```
PM> Install-Package Cryptolens.Licensing
```

Med dotnet CLI
```
dotnet add package Cryptolens.Licensing
```

**Om din applikation kommer köras på Mac, Linux or Unity/Mono, rekommenderar vi att använda en platform oberoende version av detta bibliotek.**

I Visual Studio package manager kan den installeras på följande sätt:
```
PM> Install-Package Cryptolens.Licensing.CrossPlatform
```

I dotnet CLI
```
dotnet add package Cryptolens.Licensing.CrossPlatform
```

### Exempelkod
* [Key verification](https://help.cryptolens.io/examples/key-verification)
* [Offline verification](https://help.cryptolens.io/examples/offline-verification)

### Rekommenderade artiklar

* [Unity 3D / Mono](https://help.cryptolens.io/getting-started/unity)
* [AutoCAD](https://cryptolens.io/2019/01/autocad-plugin-software-licensing/)
* [Rhinoceros / Grasshoper](https://cryptolens.io/2019/01/protecting-rhinoceros-plugins-with-software-licensing/)
*[Excel / Office Addins](https://help.cryptolens.io/getting-started/excel)

## Kompatibilitet

För att kunna använda alla funktioner i biblioteket behöver .NET Framework 4.6 eller högre användas. Vi har summerat vilken funktionalitet finns med för varje framework. Notera att .NET Standard versionen kan användas på olika versioner av .NET (inkl. .NET Core.), baserat på följande [dokument](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

* **.NET Framework 4.0** - Verifiering av Metadata signaturen stöds inte.
* **.NET Framework 4.6** - Alla funktioner stöds.
* **.NET Standard 2.0** - Beräkning av machine codes stöds inte. Det går dock att använda de hash-funktioner som finns i biblioteket tillsammans med en annan metod för att ta fram enhetsspecifik information.
* **Unity/Linux/Mac** - En platformoberoende version av biblioteket behöver användas. Mer information finns [här](https://help.cryptolens.io/getting-started/unity). 

Cryptolens.Licensing biblioteket fungerar också på Mono (t.ex. i Linux och Unity), men en speciell version av biblioteket behöver användas. Enklast är att installera den genom [NuGet](https://www.nuget.org/packages/Cryptolens.Licensing.CrossPlatform/). Det går också att ladda ner den [här](https://github.com/Cryptolens/cryptolens-dotnet/releases) (man behöver använda den som heter `Cryptolens.Licensing.CrossPlatform`)

## Andra inställningar

### Problem med Newtonsoft.Json på .NET 4.8
Vissa kunder har rapporterat ett problem med att rätt version av Newtonsoft.Json inte hittas. Problemet ser ut att bara påverka dem som använder .NET Framework 4.8, där följande fel visas:

```
System.IO.FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=11.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference.
```

Felet orsakas av att fel version av Newtonsoft.Json är installerad i samma projekt som Cryptolens.Licensing. För att lösa detta behöver du se till att Newtonsoft.Json är avinstallerat helt och sedan installera om Cryptolens.Licensing.

### 'System.MethodAccessException' vid anrop till Helpers.GetMachineCode
I vissa Windows miljöer (t.ex. Excel addins), kan det ibland uppstå problem vid anrop till Helpers.GetMachineCode på .NET Framework 4.6. Anledning är att vi i den metoden anropar `System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform`. Detta kan lösas som i exemplet nedan, dvs. innan anropet till `Helpers.GetMachineCode` eller `Helpers.IsOnRightMachine` behöver man lägga till `Helpers.WindowsOnly=True`.

```cs
Helpers.WindowsOnly = true;
var machineCode = Helpers.GetMachineCode();
```

Om metoden ovan inte fungerar, vänligen testa följande anrop istället:

```cs
var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
```

### Stänga av KeepAlive
För att göra ett API anrop utan KeepAlive, behöver man lägga till `HelperMethods.KeepAlive=False.

### Proxyinställningar
För att ändra proxyinställningar kan följande variabel användas: `HelperMethods.proxy`.

## Gamla exempel

### Se om en användare har ändrat på tiden

För att upptäcka om en användare har ändrat på sin lokala tid går det att använda följande kod.

```cs
public void HasLocalTimeChanged()
{
    bool hasChanged = SKGL.SKM.TimeCheck();

    if(hasChanged)
    {
        Debug.WriteLine("Användaren har ändrat på tiden. Avsluta programmet.");
    }
    else
    {
        Debug.WriteLine("Tiden har inte ändrats. Fortsätt som vanligt.");
    }
}
```

