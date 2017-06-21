## Changes since previous release

* Support for VPN and proxy added by default.
* SHA-256 support for machine codes has been added with `getSHA256` method. 
* A customer can be added with `AddCustomer`method and removed with `RemoveCustomer` method.
* The maximum number of machine codes can be updated using `MachineLockLimit` method.
* To list existing customers, you can use `GetCustomerLicenses` method.
* To get the machine code of the current device, `GetMachineCode` can be used. The default hash algorithm is SHA-256.
* To check if the license key belongs to the current device, `IsOnRightMachine` can be used. The default hash algorithm is SHA-256.