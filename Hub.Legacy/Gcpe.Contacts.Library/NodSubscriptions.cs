using System.Linq;
using System.Collections.Generic;
using Gcpe.Hub.Services.Legacy;
using Gcpe.Hub.Services.Legacy.Models;
using MediaRelationsLibrary.Configuration;

namespace MediaRelationsLibrary
{
    static public class NodSubscriptions
    {
        public static IList<KeyValuePairStringString> GetMediaDistributionLists()
        {
            using (var client = new SubscribeClient(App.Settings.SubscribeBaseUri.ToUri()))
            {
                return client.Subscribe.SubscriptionItems("media-distribution-lists");
            }
        }

        public static SubscriberInfo GetSubscriberInfo(string emailAddress)
        {
            using (var client = new SubscribeClient(App.Settings.SubscribeBaseUri.ToUri()))
            {
                return client.Subscribe.SubscriberInformation(emailAddress);
            }
        }

        public static void SaveSubscriberInfo(string emailAddress, IList<string> mediaDistributionLists = null, string newEmailAddress = null)
        {
            using (var client = new SubscribeClient(App.Settings.SubscribeBaseUri.ToUri()))
            {
                SubscriberInfo info = client.Subscribe.SubscriberInformation(emailAddress);
                bool isNewSubscriber = (info == null);
                if (isNewSubscriber)
                {
                    if (newEmailAddress != null && newEmailAddress != emailAddress)
                    {
                        info = client.Subscribe.SubscriberInformation(newEmailAddress);
                    }
                    isNewSubscriber = (info == null);
                    if (isNewSubscriber) //TODO some refactoring needed here.
                    {
                        info = new SubscriberInfo() { SubscribedCategories = new Dictionary<string, IList<string>>() };
                    }
                }
                if (mediaDistributionLists != null)
                {
                    // new list selection
                    info.SubscribedCategories["media-distribution-lists"] = mediaDistributionLists;
                }
                info.EmailAddress = newEmailAddress ?? emailAddress;
                info.IsAsItHappens = true;
                if (isNewSubscriber || info.ExpiredLinkOrUnverifiedEmail == true)
                {
                    client.Subscribe.CreateNewsOnDemandEmailSubscriptionWithPreferences(info);
                }
                else
                {
                    client.Subscribe.UpdateNewsOnDemandEmailSubscriptionWithPreferences(info, emailAddress);
                }
            }
        }
    }
}