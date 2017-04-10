using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    public class CustomerMethods
    {
        public static AddCustomerResult AddCustomer(string token, AddCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<AddCustomerResult>(parameters, "/customer/addcustomer/", token);
        }

        public static BasicResult RemoveCustomer(string token, RemoveCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/customer/removecustomer/", token);
        }

        public static GetCustomerLicensesResult GetCustomerLicenses(string token, GetCustomerLicensesModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetCustomerLicensesResult>(parameters, "/customer/getcustomerlicenses/", token);
        }
    }
}
