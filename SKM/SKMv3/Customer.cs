using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    public class Customer
    {
        public static AddCustomerResult AddCustomer(string token, AddCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<AddCustomerResult>(parameters, "/customer/addcustomer/", token);
        }

        public static BasicResult RemoveCustomer(string token, RemoveCustomerModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/customer/removecustomer/", token);
        }
    }
}
