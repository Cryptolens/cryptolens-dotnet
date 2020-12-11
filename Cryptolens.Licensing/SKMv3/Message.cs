using SKM.V3.Internal;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Methods
{
    /// <summary>
    /// Methods related to the Message API (https://app.cryptolens.io/docs/api/v3/Message).
    /// You can broadcast new messages by visiting https://app.cryptolens.io/Message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// This method will return a list of messages that were broadcasted.You can create new messages here.
        /// Messages can be filtered based on the time and the channel.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <returns>Returns <see cref="GetMessagesResult"/> or null.</returns>
        public static GetMessagesResult GetMessages(string token, GetMessagesModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetMessagesResult>(parameters, "/message/getmessages/", token);
        }

        /// <summary>
        /// This method will create a new message (which you can also manage here). This method requires Edit Messages permission.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <returns>Returns <see cref="GetMessagesResult"/> or null.</returns>
        public static CreateMessageResult CreateMessage(string token, CreateMessageModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<CreateMessageResult>(parameters, "/message/createmessage/", token);
        }

        /// <summary>
        /// This method will remove a message that was previously broadcasted (which you can also manage here). This method requires Edit Messages permission.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <returns>Returns <see cref="GetMessagesResult"/> or null.</returns>
        public static BasicResult RemoveMessage(string token, RemoveMessageModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<BasicResult>(parameters, "/message/removemessage/", token);
        }
    }
}
