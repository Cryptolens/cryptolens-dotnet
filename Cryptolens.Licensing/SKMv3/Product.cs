using SKGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that perform operations on a product. A complete list
    /// can be found here: https://app.cryptolens.io/docs/api/v3/Product
    /// </summary>
    public static class ProductMethods
    {
        /// <summary>
        /// This method will return a list of keys for a given product.
        /// Please keep in mind that although each license key will be of
        /// the <see cref="LicenseKey"/> type, the fields related to signing
        /// operations will be left empty. Instead, if you want to get a 
        /// signed license key (for example, to achieve offline key activation), 
        /// please use the <see cref="Key.Activate(string, ActivateModel)"/> instead.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="BasicResult"/> or null.</returns>
        /// <remarks>
        /// * The fields SignDate and Signature will be empty. Please use Activation in order to get each license signed.<br></br>
        /// * The order by field has the following structure: fieldName[ascending | descending]. For example, If you want to order by the feature field 1 (F1), you should use F1.If you want it in descending order, please add the descending keywords right after the field, eg.F1 descending. The ascending keyword is the default, hence optional.<br></br>
        /// * The search query field accepts the same queries as the search box on the product page.You can read about the format here.<br></br>
        /// * The key lock does not have any effect on this method, eg.you will still be able to retrieve all keys even if the key lock is set to a certain key.<br></br>
        /// </remarks>
        /// <example>
        /// <code language="vb" title="Listing the first 99 keys">
        /// Private Sub GetKeysExample()
        /// Dim parameters = New GetKeysModel() With {
        ///     .ProductId = 3,
        ///     .Page = 1
        /// }
        /// Dim auth = "{access token with GetKeys permission and optional product lock}"
        /// Dim result = Product.GetKeys(token:=auth, parameters:=parameters)
        /// If (result IsNot Nothing AndAlso result.Result = ResultType.Success) Then
        ///     ' successful 
        ///     ' displays the first 99 keys of the product.
        ///     ' simply increment Page to 2 in order to get
        ///     ' the rest.
        ///     For Each key As LicenseKey In result.LicenseKeys
        ///         Console.WriteLine(key.Key)
        ///     Next
        /// End If
        /// </code>
        /// </example>
        public static GetKeysResult GetKeys(string token, GetKeysModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetKeysResult>(parameters, "/product/getkeys/", token);
        }


        /// <summary>
        /// This method will return the list of products.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs.</param>
        /// <returns>A <see cref="GetProductsResult"/> or null.</returns>
        /// </example>
        public static GetProductsResult GetProducts(string token, RequestModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetProductsResult>(parameters, "/product/getproducts/", token);
        }

    }
}
