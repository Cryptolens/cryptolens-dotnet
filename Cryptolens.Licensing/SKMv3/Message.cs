using SKM.V3.Internal;
using SKM.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKM.V3.Methods
{
    public class Message
    {
        /// <summary>
        /// This method will return a list of messages that were broadcasted.You can create new messages here.
        /// Messages can be filtered based on the time and the channel.
        /// </summary>
        /// <param name="token">The access token. Read more at https://app.cryptolens.io/docs/api/v3/Auth </param>
        /// <param name="parameters">The parameters that the method needs</param>
        /// <returns>Returns <see cref="GetMessagesResult"/> or null.</returns>
        public static BasicResult GetMessages(string token, GetMessagesModel parameters)
        {
            return HelperMethods.SendRequestToWebAPI3<GetMessagesResult>(parameters, "/message/getmessages/", token);
        }
    }
}
