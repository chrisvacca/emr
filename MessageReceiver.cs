using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using System.Timers;
using TT;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace TigerText_EMR_Service
{
    public partial class MessageReceiver : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public MessageReceiver()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);

                TT.Win.SDK.Global.Init(ConfigurationManager.AppSettings.Get("API_KEY"), ConfigurationManager.AppSettings.Get("API_SECRET"));
                TT.Win.SDK.Api.Events.MessageReceivedEvent += Events_MessageReceivedEvent;
                TT.Win.SDK.Api.Events.StartListening();

                eventLog1.WriteEntry("The TigerTextMessageReceiverService has started.  MessageReceiver is listening for configured key " + ConfigurationManager.AppSettings.Get("API_KEY"));
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            eventLog1.WriteEntry("The TigerTextMessageReceiverService is terminating.  The background timer has been stopped and disposed.");
        }

        private async void Events_MessageReceivedEvent(object sender, TT.Win.SDK.Events.MessageEventArgs e)
        {
            try
            {
                // Step 1.  Get the group token or user token for the recipient
                string replyToken = string.IsNullOrEmpty(e.MessageData.group_token) ? e.MessageData.sender : e.MessageData.group_token;

                // Step 2.  Validate the input
                // PSUEDOCODE - Check list of supported commands
                //if (e.MessageData.body.ToLower() == "help")
                //{
                //    returnList = "Type a deparment name to see who is oncall for today.  Example - cardio, urology, etc";
                //}

                // Step 3.  Business logic here to update CRM, etc
                // UPDATE CRM DB based on logic, input from TT message, etc.

                // Step 4.  Reply to the user (if required). FOr now, just echo the message back.
                await TT.Win.SDK.Api.Message.SendMessageAsync(string.Format("Echo of your message: {0}", e.MessageData.body), replyToken);
                
                // Write event log
                string logMessage = string.Format("TigerText message received at {0} for key {1}.  Message Sender is {2} (token={3}).  Message_ID is {4}.  Message Body is {5}.", DateTime.Now.ToLongTimeString(), ConfigurationManager.AppSettings.Get("API_KEY"), e.MessageData.sender_name, e.MessageData.sender_token, e.MessageData.message_id, e.MessageData.body);
                eventLog1.WriteEntry(logMessage);

                // Write to rolling log file (make this a configurable option)
                // http://logging.apache.org/log4net/release/config-examples.html
                // log.Info(logMessage);
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
