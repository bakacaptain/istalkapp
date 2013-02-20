using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using CommonUtilities;

namespace ServerNetworkConnections.Protocol
{
    /// <summary>
    /// Utility class that helps parse AbstractMessages from a given XML String.
    /// </summary>
    public class XMLMessageParser : AbstractMessageParser
    {
        /// <summary>
        /// Static method that takes in an xml String and converts it to an AbstractMessage.
        /// 
        /// Will throw an XmlException if the xml String was in an unrecognized format.
        /// </summary>
        /// <param name="xml">A String with xml representing an AbstractMessage</param>
        /// <returns>The AbstractMessage parsed from the XML String</returns>
        public override AbstractMessage Parse(String xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            AbstractMessage returnee = null;
            Int32 chatId = 0;

            String element = "";
            String textMessage = "";
            String username = "";
            String password = "";
            String sender = "";

            Boolean accepted = false;
            Boolean isAlreadyLoggedOn = true;
            Boolean alreadyExisted = false;
            Boolean authenticationDisabled = false;

            GroupChat chat = null;
            ExtendedHashSet<String> hashSet = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.CDATA:
                            switch (element)
                            {
                                case "Message":
                                    textMessage = reader.Value;
                                    break;
                                case "Username":
                                    username = reader.Value;
                                    break;
                                case "Password":
                                    password = reader.Value;
                                    break;
                                case "User":
                                    hashSet.Add(reader.Value);
                                    break;
                                case "Participant":
                                    hashSet.Add(reader.Value);
                                    break;
                                case "Sender":
                                    sender = reader.Value;
                                    break;
                                case "Name":
                                    hashSet.Add(reader.Value);
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case XmlNodeType.Element:
                            element = reader.Name; // define last known element
                            switch (element)
                            {
                                case "UserList":
                                    hashSet = new ExtendedHashSet<String>();
                                    break;
                                case "Participants":
                                    hashSet = new ExtendedHashSet<String>();
                                    break;
                                case "ToBeAdded":
                                    hashSet = new ExtendedHashSet<String>();
                                    break;
                                case "ToBeRemoved":
                                    hashSet = new ExtendedHashSet<String>();
                                    break;
                            }
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "TextMessage":
                                    returnee = new TextMessage(textMessage);
                                    break;
                                case "LoginMessage":
                                    returnee = new LoginMessage(username, password);
                                    break;
                                case "LoginResponseMessage":
                                    returnee = new LoginResponseMessage(accepted, isAlreadyLoggedOn, username);
                                    break;
                                case "AddToUserListMessage":
                                    returnee = new AddToUserListMessage(hashSet);
                                    break;
                                case "RemoveFromUserListMessage":
                                    returnee = new RemoveFromUserListMessage(hashSet);
                                    break;
                                case "GroupChatMessage":
                                    returnee = new GroupChatMessage(chat, textMessage, sender);
                                    break;
                                case "AddMemberToGroupChatMessage":
                                    returnee = new AddMemberToGroupChatMessage(chatId, hashSet);
                                    break;
                                case "RemoveMemberFromGroupChatMessage":
                                    returnee = new RemoveMemberFromGroupChatMessage(chatId, hashSet);
                                    break;
                                case "StartGroupChatRequestMessage":
                                    returnee = new StartGroupChatRequestMessage(hashSet);
                                    break;
                                case "StartGroupChatResponseMessage":
                                    returnee = new StartGroupChatResponseMessage(chatId, alreadyExisted);
                                    break;
                                case "GroupChatInfoRequestMessage":
                                    returnee = new GroupChatInfoRequestMessage(chatId);
                                    break;
                                case "GroupChatInfoResponseMessage":
                                    returnee = new GroupChatInfoResponseMessage(chat);
                                    break;
                                case "SignupRequestMessage":
                                    returnee = new SignupRequestMessage(username, password);
                                    break;
                                case "SignupResponseMessage":
                                    returnee = new SignupResponseMessage(accepted, alreadyExisted, authenticationDisabled);
                                    break;
                                case "GroupChat":
                                    chat = new GroupChat(hashSet, chatId);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (element)
                            {
                                case "Accepted":
                                    accepted = Boolean.Parse(reader.Value);
                                    break;
                                case "IsAlreadyLoggedOn":
                                    isAlreadyLoggedOn = Boolean.Parse(reader.Value);
                                    break;
                                case "ChatID":
                                    chatId = int.Parse(reader.Value);
                                    break;
                                case "AlreadyExisted":
                                    alreadyExisted = Boolean.Parse(reader.Value);
                                    break;
                                case "AuthenticationDisabled":
                                    authenticationDisabled = Boolean.Parse(reader.Value);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                throw new XmlException("Unrecognized XML Format...");
            }
            reader.Close();
            if (returnee != null)
            {
                return returnee;
            }
            else
            {
                throw new XmlException("Unrecognized XML Format...");
            }
        }
    }
}
