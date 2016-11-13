using System.Web.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Web.Http.Description;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types


namespace Microsoft.Bot.Sample.AnnotatedSandwichBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<SandwichOrder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildLocalizedForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        // Actually process the sandwich order...
                        queueBotMessage(order.ToString());
                        await context.PostAsync("Processed your order!");
                    }
                    catch (FormCanceledException<SandwichOrder> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last}--maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit.  Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        internal static IDialog<JObject> MakeJsonRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(SandwichOrder.BuildJsonForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        string sAnswers = setMessageSentiment(completed.Root.ToString());
                        queueBotMessage(sAnswers);
                        CookieQuotes replies = new CookieQuotes();
                        await context.PostAsync(replies.getRandomQuote());
                    }
                    catch (FormCanceledException<JObject> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last}--maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit.  Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        private static string setMessageSentiment(string msg)
        {
            JObject jo = JObject.Parse(msg);
            string comment = jo["Comments"].ToString();
            SentimentAnalysis snt = new SentimentAnalysis();
            float fSnt = snt.GetSentiment(comment);
            string retMessage;
            if (fSnt > 0.7)
            {
                retMessage = $"Positive";
            }
            else if (fSnt < 0.3)
            {
                retMessage = $"Negative";
            }
            else
            {
                retMessage = $"Indifferent";
            }
            retMessage = "{'Sentiment' : '" + retMessage + "'}";
            JObject jo1 = JObject.Parse(retMessage);
            jo.Merge(jo1, new JsonMergeSettings{
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return jo.ToString();
        }

        private static void queueBotMessage(string msg)
        {
            if (CloudConfigurationManager.GetSetting("UseQueue") == "false")
                return;
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("bot-message-queue");
            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();
            CloudQueueMessage message = new CloudQueueMessage(msg);
            queue.AddMessage(message);
        }

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, MakeJsonRootDialog);
                        break;
                /*    
                        if (activity.Text.ToLower() == "provide feedback")
                        {
                        }
                        if (activity.Text.ToLower() == "list sessions")
                        {
                            SessionList sessions = new SessionList();
                            var connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(sessions.getSessions()));
                            break;
                        }
                        else
                        {
                            CookieQuotes replies = new CookieQuotes();
                            var connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(replies.getRandomQuote()));
                            break;
                        }
                    */
                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}