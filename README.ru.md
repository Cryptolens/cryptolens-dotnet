# Cryptolens Licensing для .NET

Эта библиотека служит как интерфейс к Cryptolens Web API (https://app.cryptolens.io/docs/api). К примеру, её можно использовать для проверки лицензий в вашем ПО.

> Документация библиотеки доступна на https://help.cryptolens.io/api/dotnet/.

Пожалуйста прочитайте следующей гид о часто возникающих ошибках при интеграции: https://help.cryptolens.io/faq/index#troubleshooting-api-errors

Если возникнут какие-нибудь вопросы, пожалуйста напишите нам: support@cryptolens.io

## Начать работать с библиотекой

### Установить через Nuget

В Visual Studio package manager
```
PM> Install-Package Cryptolens.Licensing
```

Используя dotnet CLI
```
dotnet add package Cryptolens.Licensing
```
**Если ваша программа будет работать на Mac, Linux или Unity/Mono, мы рекомендуем использовать кросс-платформенную версию библиотеки.**

В Visual Studio package manager, её можно установить следующим образом:

```
PM> Install-Package Cryptolens.Licensing.CrossPlatform
```

Используя dotnet CLI:
```
dotnet add package Cryptolens.Licensing.CrossPlatform
```

### Примеры кода
* [Проверка лицензии (Key verification)](https://help.cryptolens.io/examples/key-verification)
* [Проверка лицензии офлайн (Offline verification)](https://help.cryptolens.io/examples/offline-verification)

### Рекомендуемые статьи

* [Unity 3D / Mono](https://help.cryptolens.io/getting-started/unity)
* [AutoCAD](https://cryptolens.io/2019/01/autocad-plugin-software-licensing/)
* [Rhinoceros / Grasshoper](https://cryptolens.io/2019/01/protecting-rhinoceros-plugins-with-software-licensing/)

## Совместимость

Чтобы получить доступ ко всем функциям библиотеки, нужно использовать .NET Framework 4.6 или выше. В списке снизу мы суммировали разницы между разными версиями .NET. Примечание, версию библиотеки для .NET Standard можно использовать на разных платформах (включая .NET Core), согласно следующему [документу](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

* **.NET Framework 4.0** - Проверка подписи в параметре Metadata не поддерживается.
* **.NET Framework 4.6** - Все функции поддерживаются.
* **.NET Standard 2.0** - Вычисления машинного кода ("machine code") не поддерживается. Чтобы вычислить машинный код можно использовать встроенные хеш-функции, но нужно использовать другой способ сбора информации об устройстве.
* **Unity/Linux/Mac** - Нужно использовать кросс-платформенную версию библиотеки. Больше информации об этом доступно [здесь](https://help.cryptolens.io/getting-started/unity). 

Библиотека Cryptolens.Licensing тоже работает с Mono (к примеру в Linux или Unity), но нужно будет использовать специальную версию библиотеки. Её можно установить либо через [NuGet](https://www.nuget.org/packages/Cryptolens.Licensing.CrossPlatform/), скачать [предварительно компилированную библиотеку](https://github.com/Cryptolens/cryptolens-dotnet/releases) (нужно использовать библиотеку под названием `Cryptolens.Licensing.CrossPlatform`) или самим компилировать библиотеку (инструкции есть в английской версии документации).

## Другие настройки
### 'System.MethodAccessException' при вызове метода Helpers.GetMachineCode

В некоторых Windows средах (к примеру в Excel addins), может возникнуть проблема с Helpers.GetMachineCode в .NET Framework 4.6. Причина заключается в том, что тот метод вызывает `System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform`. Чтобы решить эту проблему, мы добавили способ обойти этот метод. Перед тем как вызывать методы `Helpers.GetMachineCode` or `Helpers.IsOnRightMachine`, нужно добавить`Helpers.WindowsOnly=True`.

```cs
Helpers.WindowsOnly = true;
var machineCode = Helpers.GetMachineCode();
```

### Выключить KeepAlive

Чтобы сделать вызов API без KeepAlive, его можно выключить с помощью `HelperMethods.KeepAlive=False`.

### Прокси настройки
Чтобы изменить прокси-настройки, можно использовать переменную `HelperMethods.proxy`.

## Старые примеры

### Проверка отката времени
Чтобы проверить если пользователь поменял своё локальное время, можно использовать следующий код:

```cs
public void HasLocalTimeChanged()
{
    bool hasChanged = SKGL.SKM.TimeCheck();

    if(hasChanged)
    {
        Debug.WriteLine("Пользователь поменял своё время. Ошибка.");
    }
    else
    {
        Debug.WriteLine("Временя не было изменено. Продолжать дальше.");
    }
}
```