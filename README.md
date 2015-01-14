# SKGL Extension for .NET

The original repository of this project is: https://skgl.codeplex.com/.
NB: This is only SKGL.SKM, not the entire SKGL API.

##Purpose

This API is intergrated with the Serial Key Manager Web API 2.0 (http://docs.serialkeymanager.com/)

##How to use:

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
