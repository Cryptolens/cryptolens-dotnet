# Release notes Cryptolens.SKM (v.1.0.1) Beta

## Introduction
Cryptolens.SKM aims to be a replacement of SKM Client API, which will support more platforms by default. This is accomplished by targeting [.NET Standard](https://blogs.msdn.microsoft.com/dotnet/2016/09/26/introducing-net-standard/). Although it can work standalone, independent of our .NET Framework targeting API, it is thought that some features such **User Login Auth**, will be used in combination with our old API (i.e. SKM Client API).

## Installing
The easiest way of installing Cryptolens.SKM is through NuGet ([our NuGet package](https://www.nuget.org/packages/Cryptolens.SKM/1.0.1)).

## Examples

### User Login Auth
User login authentication allows you to use user accounts instead of license keys to protect your application. This is both safer and more convenient to your customers. It can appear harder to get going, so we will put extra focus in this section to make it easier.

#### Prior reading

It is recommended that you read through this article first: [https://serialkeymanager.com/docs/api/v3/GetToken](https://serialkeymanager.com/docs/api/v3/GetToken). We plan to create a step-by-step video soon, which will appear here.

#### Installing
We need two install two packages, **SKGLExtension** and **Cryptolens.SKM**. You can find them on NuGet. 

> Your application has to target at least .NET Framework 4.6.1 or .NET Core 1.0. For other platforms, please see section *What’s new in .NET Standard 2.0?* in [this article](https://blogs.msdn.microsoft.com/dotnet/2016/09/26/introducing-net-standard/).

#### Create customers
For this to work, we need to create a customer in SKM. 

1. Visit [this page](https://serialkeymanager.com/Customer) and create a new customer.
2. Edit the customer and select **Enable Customer Association**.
3. Copy the link in **Customer Link** and send it to your customer. 

Once this is done, the customer in your SKM account will be linked to a user account. You can then add and remove licenses directly for that customer.

#### Insert code

Since User Login Auth is still in beta, we need to perform several extra steps:

First, add the following namespaces:

```
using Cryptolens.SKM.Auth;
using Newtonsoft.Json;
using SKM.V3;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SKGL;
```

Secondly, add the following class to your project (make sure our next code snippet can access it):

```
    public class GetLicenseKeysResult : BasicResult
    {
        public string Results { get; set; }
        public string ActivatedMachineCodes { get; set; }
        public string Signature { get; set; }
    }
```

Finally, we can add the code below (for .NET Framework):

```
 static void Main(string[] args)
 {

     // This is found at https://serialkeymanager.com/User/Security
     var RSAPublicKey = new RSACryptoServiceProvider(2048);
     RSAPublicKey.FromXmlString("Your RSA public key");

     var authRequest = UserLoginAuth.GetLicenseKeys(SKGL.SKM.getMachineCode(SKGL.SKM.getSHA256), "{access token with GetToken permission}", "Test Application", 5, RSAPublicKey.ExportParameters(false), null, new RSACryptoServiceProvider(2048));

     if (authRequest.error == null)
     {
         var data = JsonConvert.DeserializeObject<GetLicenseKeysResult>(authRequest.jsonResult);

         var licenses = JsonConvert.DeserializeObject<List<KeyInfoResult>>(System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(data.Results)));

         var findingLicense = licenses.Where(x => x.LicenseKey.ProductId == 3349 && x.LicenseKey.F1 == true && x.LicenseKey.HasNotExpired().IsValid());

         if(findingLicense.Count() > 1)
         {
             Console.WriteLine("Great, the user has the right license.");
         }
         else
         {
             Console.WriteLine("The user has to buy a new license.");
         }
     }
     else
     {
         Console.WriteLine("An error occurred.");
     }
 }
```
You can also view this [as a part of a project](https://github.com/SerialKeyManager/SKGL-Extension-for-dot-NET/tree/master/SKM/User%20Login%20Auth%20Example).

For .NET Core applications, you don't need to add the last parameter, `RSACryptoServiceProvider(2048)`. We will keep updating this page with more information about .NET Core.