# Cryptolens Licensing for .NET

This API serves as a layer that simplifies communication with Serial Key Manager Web API 2 and 3 (https://app.cryptolens.io/docs/api).

### Web API versions

In Web API 2, authentication usually contains variables such as "pid", "uid" and "hsum". In Web API 3, authentication is performed using access tokens (https://app.cryptolens.io/docs/api/v3/Auth). Please always use Web API 3 for newer projects.

You can access the documentation of the library at https://help.cryptolens.io/api/dotnet/.

### Examples

* [Most common examples](https://github.com/SerialKeyManager/SKGL-Extension-for-dot-NET/blob/master/Tutorials/v401.md) - this contains our core licensing methods.
* [Experimental features](https://github.com/SerialKeyManager/SKGL-Extension-for-dot-NET/blob/master/Tutorials/v.101-beta.md) - this includes user login auth and other experimental features.


The original repository of this project was: https://skgl.codeplex.com/.
NB: This is only SKGL.SKM, not the entire SKGL API.


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
