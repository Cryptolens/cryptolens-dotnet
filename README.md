# SKGL Extension for .NET

This API is intergrated with the Serial Key Manager Web API 2.0 (http://docs.serialkeymanager.com/)

The original repository of this project was: https://skgl.codeplex.com/.
NB: This is only SKGL.SKM, not the entire SKGL API.

##How to use
1. [Key Validation](#key-validation)
2. [Key Activation](#key-activation)
3. [Offline Key Validation](#offline-key-validation)
4. [List User Products](#list-user-products)
5. [Get Product Variables](#get-product-variables)
6. [Check Against Time Rollback](#check-against-time-rollback)
7. [Other Methods in Web API](#other-methods-in-web-api)
8. [Calculating Machine code](#calculating-machine-code)

###Key Validation
For *pid*, *uid* and *hsum*, please see https://serialkeymanager.com/Ext/Val. You can retreive them using  [Get Product Variables](#get-product-variables).
```
public void KeyValiation()
{
    var validationResult = SKGL.SKM.KeyValidation("pid", "uid", "hsum", "serial key to validate");

    var newKey = validationResult.NewKey;

    if (validationResult != null)
    {
        //valid key
        var created = validationResult.CreationDate;
        var expires = validationResult.ExpirationDate;
        var setTime = validationResult.SetTime;
        var timeLeft = validationResult.TimeLeft;
        var features = validationResult.Features;

    }
    else
    {
        //invalid key
        Assert.Fail();
    }
}
```

###Key Activation
For *pid*, *uid* and *hsum*, please see https://serialkeymanager.com/Ext/Val.

NB: If trial activation is configured, the API can return a new key (read more [here](http://support.serialkeymanager.com/howto/trial-activation/)).
```
public void KeyActivation()
{
    var validationResult = SKGL.SKM.KeyActivation("pid", "uid", "hsum", "serial key to validate", "machine code", {sign the data}, {sign machine code});

    if (validationResult != null)
    {
        //valid key
        var created = validationResult.CreationDate;
        var expires = validationResult.ExpirationDate;
        var setTime = validationResult.SetTime;
        var timeLeft = validationResult.TimeLeft;
        var features = validationResult.Features;
    }
    else
    {
        //invalid key
        Assert.Fail();
    }
}
```
###Offline key validation
Read more about offline key validation [here](http://support.serialkeymanager.com/howto/passive-key-validation-offline/).
```
public void SecureKeyValidation()
{
    // your public key can be found at http://serialkeymanager.com/Account/Manage.
    string rsaPublicKey = "<RSAKeyValue><Modulus>pL01ClSf7U5kp2E7C9qFecZGiaV8rFpET1u9QvuBrLNkCRB5mQFiaCqHyJd8Wj5o/vkBAenQO+K45hLQakve/iAmr4NX/Hca9WyN8DVhif6p9wD+FIGWeheOkbcrfiFgMzC+3g/w1n73fK0GCLF4j2kqnWrDBjaB4WfzmtA5hmrBFX3u9xcYed+dXWJW/I4MYmG0cQiBqR/P5xTTE+zZWOXwvmSZZaMvBh884H9foLgPWWsLllobQTHUqRq6pr48XrQ8GjV7oGigTImolenMLSR59anDCIhZy59PPsi2WE7OoYP8ecNvkdHWr1RlEFtx4bUZr3FPNWLm7QIq7AWwgw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    
    SKGL.KeyInformation keyInfo = new SKGL.KeyInformation();
    bool fileLoaded = false;

    try
    {
        keyInfo = SKGL.SKM.LoadKeyInformationFromFile("file.txt");
        fileLoaded = true;
    }
    catch{}


    if(fileLoaded)
    {
        if (SKGL.SKM.IsKeyInformationGenuine(keyInfo, rsaPublicKey))
        {
            // if we've come so far, we know that
            // * the key has been checked against the database once
            // * the file with the key infromation has not been modified.

            // check the key
            if (keyInfo.Valid)
            {
                // here we can retrive some useful info
                Console.WriteLine(keyInfo.CreationDate);
                //... etc.
            }
        }
        else
        {
            Assert.Fail();
        }
    }
    else
    {
        // it's crucial that both json and secure are set to true
        keyInfo = SKGL.SKM.KeyValidation("pid", "uid", "hsum", "serial key", {sign key information file}); // KeyActivation method works also.

        if(keyInfo != null)
        {
            SKGL.SKM.SaveKeyInformationToFile(keyInfo, "file.txt");
        }
    }
}
```

###List User Products
```
public void ListAllProducts()
{
    var listOfProducts = SKGL.SKM.ListUserProducts("username", "password");

    foreach (var product in listOfProducts)
    {
        Debug.WriteLine("The product with the name \""+ product.Key + "\" has the pid \"" + product.Value + "\"");
    }
}
```

###Get Product Variables
This will get *pid*, *uid* and *hsum*.
```
public void GetProductVariables()
{
    var listOfProducts = SKGL.SKM.ListUserProducts("username", "password");

    //variables needed in for instance validation/activation
    //note, First requires System.Linq.
    var productVar = SKGL.SKM.GetProductVariables("username","password", listOfProducts.First().Value);

    Debug.WriteLine("The uid=" + productVar.UID + ", pid=" + productVar.PID + " and hsum=" + productVar.HSUM);
}
```

###Check Against Time Rollback
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

###Other Methods in Web API
If you would like to access a method in the Web API manually, please use *GetParameters* method. A list of them can be found [here](http://docs.serialkeymanager.com/web-api/).
```
public void GetParamtersTest()
{
    var input = new System.Collections.Generic.Dictionary<string, string>();
    input.Add("uid", "1");
    input.Add("pid", "1");
    input.Add("hsum", "11111");
    input.Add("sid", "ABCD-EFGHI-GKLMN-OPQRS");
    input.Add("sign","true");

    var result = SKGL.SKM.GetParameters(input, "Validate");

    var keyinfo = SKGL.SKM.GetKeyInformationFromParameters(result);

    if(result.ContainsKey("error") && result["error"] != "")
    {
        Assert.Fail();
    }
}
```

###Calculating Machine code
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
