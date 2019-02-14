using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that perform operations on a customer object. A complete list can be found here: https://app.cryptolens.io/docs/api/v3/Customer
    /// </summary>
    public class CustomerMethods
    {
        /// <summary>
        /// This method will add new customer. To remove an existing customer, please see <see cref="RemoveCustomer(string, RemoveCustomerModel)"/>
        /// </summary>
        /// <param name="token">An access token with AddCustomer permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static AddCustomerResult AddCustomer(string token, AddCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<AddCustomerResult>(parameters, "/customer/addcustomer/", token);
        }


        /// <summary>
        /// This method will return the list of customers, with the newest customers shown first.
        /// </summary>
        /// <param name="token">An access token with AddCustomer permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static GetCustomersResult GetCustomers(string token, GetCustomersModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetCustomersResult>(parameters, "/customer/getcustomers/", token);
        }


        /// <summary>
        /// This method will remove an existing customer given the customerId. To add a new customer, please see <see cref="RemoveCustomer(string, RemoveCustomerModel)"/>
        /// </summary>
        /// <param name="token">An access token with RemoveCustomer permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult RemoveCustomer(string token, RemoveCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/customer/removecustomer/", token);
        }

        /// <summary>
        /// This method will return a list of license keys that belong to a certain customer.
        /// </summary>
        /// <param name="token">An access token with GetCustomerLicenses permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static GetCustomerLicensesResult GetCustomerLicenses(string token, GetCustomerLicensesModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetCustomerLicensesResult>(parameters, "/customer/getcustomerlicenses/", token);
        }
    }
}
