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
* [Excel / Office Addins](https://help.cryptolens.io/getting-started/excel)

## Совместимость

Чтобы получить доступ ко всем функциям библиотеки, нужно использовать .NET Framework 4.6 или выше. В списке снизу мы суммировали разницы между разными версиями .NET. Примечание, версию библиотеки для .NET Standard можно использовать на разных платформах (включая .NET Core), согласно следующему [документу](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

* **.NET Framework 4.0** - Проверка подписи в параметре Metadata не поддерживается.
* **.NET Framework 4.6** - Все функции поддерживаются.
* **.NET Standard 2.0** - Вычисления машинного кода ("machine code") не поддерживается. Чтобы вычислить машинный код можно использовать встроенные хеш-функции, но нужно использовать другой способ сбора информации об устройстве.
* **Unity/Linux/Mac** - Нужно использовать кросс-платформенную версию библиотеки. Больше информации об этом доступно [здесь](https://help.cryptolens.io/getting-started/unity). 

Библиотека Cryptolens.Licensing тоже работает с Mono (к примеру в Linux или Unity), но нужно будет использовать специальную версию библиотеки. Её можно установить либо через [NuGet](https://www.nuget.org/packages/Cryptolens.Licensing.CrossPlatform/), скачать [предварительно компилированную библиотеку](https://github.com/Cryptolens/cryptolens-dotnet/releases) (нужно использовать библиотеку под названием `Cryptolens.Licensing.CrossPlatform`) или самим компилировать библиотеку (инструкции есть в английской версии документации).

## Другие настройки

### Ошибка с Newtonsoft.Json на .NET 4.8
Некоторые клиенты сообщили об ошибке, из-за которой нашей библиотеке не удаётся найти нужную версию Newtonsoft.Json. Это проблема локализована для тех, кто использует NET Framework 4.8. Снизу приведён пример ошибки:

```
System.IO.FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=11.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference.
```

Ошибка возникает, если неправильная версия Newtonsoft.Json установлена в том же проекте, что и библиотека Cryptolens.Licensing. Чтобы исправить это, вам необходимо убедиться, что Newtonsoft.Json полностью удален, а затем переустановить Cryptolens.Licensing.

### Проблема с активацией, даже если api.cryptolens.io доступен в браузере

Если ваши клиенты могут посещать `app.cryptolens.io` и `api.cryptolens.io` в Microsoft Edge, но не могут активировать приложение, проблема может заключаться в том, что они используют прокси-сервер, подключаются к Active Directory или потому, что их ИТ-специалист отдел заблокировал TLS определенных версий.

Чтобы устранить эти проблемы, мы рекомендуем:

1. Обновите SDK до последней версии.
2. Если вы используете версию .NET Framework до .NET Framework 4.7, мы рекомендуем вручную указать, какой TLS следует использовать. Перед любым вызовом API (например, Key.Activate выполняет вызов API) мы рекомендуем добавить следующую строку:

```cs
System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
```

В идеале вам следует попытаться выбрать самую высокую доступную версию TLS, но важно также проверить, поддерживается ли такая версия TLS в используемой вами версии .NET Framework.

Если возможно, лучше всего использовать последнюю версию .NET Framework. Если это невозможно, используйте как минимум .NET Framework 4.7. В других случаях можно использовать описанный выше обходной путь (т. е. указать версию TLS вручную).

### 'System.MethodAccessException' при вызове метода Helpers.GetMachineCode

В некоторых Windows средах (к примеру в Excel addins), может возникнуть проблема с Helpers.GetMachineCode в .NET Framework 4.6. Причина заключается в том, что тот метод вызывает `System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform`. Чтобы решить эту проблему, мы добавили способ обойти этот метод. Перед тем как вызывать методы `Helpers.GetMachineCode` or `Helpers.IsOnRightMachine`, нужно добавить`Helpers.WindowsOnly=True`.

```cs
Helpers.WindowsOnly = true;
var machineCode = Helpers.GetMachineCode();
```

Если метод сверху не будет работать, попробуйте пожалуйста использовать следующий метод:
```cs
var machineCode = SKGL.SKM.getMachineCode(SKGL.SKM.getSHA1);
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