# SKGL Extension for .NET

This API is intergrated with the Serial Key Manager Web API 2.0 (http://docs.serialkeymanager.com/)

The original repository of this project was: https://skgl.codeplex.com/.
NB: This is only SKGL.SKM, not the entire SKGL API.

##How to use
1. [Key Validation](#key-validation)
2. [Key Activation](#key-activation)
3. [Offline Key Validation](#offline-key-validation)
###Key Validation
For *pid*, *uid* and *hsum*, please see https://serialkeymanager.com/Ext/Val.
```
public void KeyValiation()
{
    var validationResult = SKGL.SKM.KeyValidation("pid", "uid", "hsum", "serial key to validate");

    var newKey = validationResult.NewKey;

    if (validationResult.Valid)
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

    if (validationResult.Valid)
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

        if(keyInfo.Valid)
        {
            SKGL.SKM.SaveKeyInformationToFile(keyInfo, "file.txt");
        }
    }
}
```
