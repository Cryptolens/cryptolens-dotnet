# SKGL Extension for .NET

This API is intergrated with the Serial Key Manager Web API 2.0 (http://docs.serialkeymanager.com/)

The original repository of this project was: https://skgl.codeplex.com/.
NB: This is only SKGL.SKM, not the entire SKGL API.

##How to use

###Key Validation
For *pid*, *uid* and *hsum*, please see https://serialkeymanager.com/ext/val
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
For *pid*, *uid* and *hsum*, please see https://serialkeymanager.com/ext/val
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
