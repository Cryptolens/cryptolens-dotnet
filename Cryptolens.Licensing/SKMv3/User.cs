using SKM.V3.Models;
using SKM.V3.Internal;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods that perform operations on a user object. A complete list can be found here: https://app.cryptolens.io/docs/api/v3/UserAuth
    /// </summary>
    public class UserAuth
    {
        /// <summary>
        /// This method will return all licenses that belong to the user. 
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthNormal and UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static LoginUserResult Login(string token, LoginUserModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<LoginUserResult>(parameters, "/userauth/login/", token);
        }

        /// <summary>
        /// This method will register a new user.  
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult Register(string token, RegisterUserModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/userauth/register/", token);
        }


        /// <summary>
        /// Associates a user with a customer object.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult Associate(string token, AssociateUserModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/userauth/Associate/", token);
        }

        /// <summary>
        /// Dissociates a user from a customer customer object.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult Dissociate(string token, DissociateModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/userauth/Dissociate/", token);
        }


        /// <summary>
        /// List all registered users.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static GetUsersResult GetUsers(string token, GetUsersModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetUsersResult>(parameters, "/userauth/GetUsers/", token);
        }

        /// <summary>
        /// This method will change the password of a user. It supports 3 modes of operation. With an access token that has UserAuthNormal permission (i.e. without admin permission), the password can either be changed by providing the old password or a password reset token, which can be generated using Reset Password Token method. Finally, if you call this method with an access token that has UserAuthAdmin permission, it will allow you to set AdminMode to True and only provide the NewPassword.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthNormal or UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult ChangePassword(string token, ChangePasswordModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/userauth/ChangePassword/", token);
        }

        /// <summary>
        /// This method allows you to retrive the password reset token that you can use when calling Change Password method.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static ResetPasswordResult ResetPasswordToken(string token, ResetPasswordModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<ResetPasswordResult>(parameters, "/userauth/ResetPasswordToken/", token);
        }

        /// <summary>
        /// This method removes a user.
        /// </summary>
        /// <param name="token">This method can be called with an access token that has UserAuthAdmin permission. More info: https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">Parameters of the method.</param>
        /// <returns></returns>
        public static BasicResult RemoveUser(string token, RemoveUserModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/userauth/RemoveUser/", token);
        }
    }
}
