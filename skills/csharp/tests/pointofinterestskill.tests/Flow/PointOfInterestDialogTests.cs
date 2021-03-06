﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PointOfInterestSkill.Dialogs;
using PointOfInterestSkill.Responses.Main;
using PointOfInterestSkill.Responses.Shared;
using PointOfInterestSkill.Tests.Flow.Strings;
using PointOfInterestSkill.Tests.Flow.Utterances;

namespace PointOfInterestSkill.Tests.Flow
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class PointOfInterestDialogTests : PointOfInterestSkillTestBase
    {
        /// <summary>
        /// Find nearest points of interest nearby.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToNearestPointOfInterestTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.FindNearestPoi)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Reprompt current location and find nearest points of interest nearby.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RepromptForCurrentAndRouteToNearestPointOfInterestTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(FindPointOfInterestUtterances.FindNearestPoi)
                .AssertReply(AssertContains(POISharedResponses.PromptForCurrentLocation, null))
                .Send(ContextStrings.Ave)
                .AssertReply(AssertContains(POISharedResponses.CurrentLocationMultipleSelection, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.No)
                .AssertReply(AssertContains(POISharedResponses.PromptForCurrentLocation, null))
                .Send(ContextStrings.Ave)
                .AssertReply(AssertContains(POISharedResponses.CurrentLocationMultipleSelection, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby and get directions to one by index number.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestByIndexTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby and get directions to one by POI name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestByNameTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(ContextStrings.MicrosoftWay)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby and select none to cancel.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestAndSelectNoneTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(GeneralTestUtterances.SelectNone)
                .AssertReply(AssertContains(POISharedResponses.CancellingMessage, null))
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby by category.
        /// TODO by option 3 since they are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestByCategoryTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.FindPharmacy)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionThree)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby then call.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestThenCallTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionTwo)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.Call)
                .AssertReply(CheckForEvent(PointOfInterestDialogBase.OpenDefaultAppType.Telephone))
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby then start navigation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestThenStartNavigationTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby with interruptions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task RouteToPointOfInterestAndInterruptTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.DetailsNoCall }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find points of interest nearby with multiple routes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task MultipleRouteTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindPointOfInterestUtterances.WhatsNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionTwo)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertStartsWith(POISharedResponses.MultipleRoutesFound, new string[] { CardStrings.Route, CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find parking nearby.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ParkingNearbyTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindParkingUtterances.FindParkingNearby)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find nearest parking.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ParkingNearestTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindParkingUtterances.FindParkingNearest)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find nearest parking and cancel.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ParkingNearestAndCancelTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindParkingUtterances.FindParkingNearest)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(GeneralTestUtterances.Cancel)
                .AssertReply(AssertContains(POISharedResponses.CancellingMessage, null))
                .StartTestAsync();
        }

        /// <summary>
        /// Find nearest parking and help.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task ParkingNearestAndHelpTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(FindParkingUtterances.FindParkingNearest)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(GeneralTestUtterances.Help)
                .AssertReply(AssertContains(POIMainResponses.HelpMessage, null))
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Find parking nearby.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task PromptForCurrentAndParkingNearAddressTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(FindParkingUtterances.FindParkingNearAddress)
                .AssertReply(AssertContains(POISharedResponses.PromptForCurrentLocation, null))
                .Send(ContextStrings.Ave)
                .AssertReply(AssertContains(POISharedResponses.CurrentLocationMultipleSelection, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Details }))
                .Send(BaseTestUtterances.ShowDirections)
                .AssertReply(AssertContains(null, new string[] { CardStrings.Route }))
                .Send(BaseTestUtterances.StartNavigation)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Get directions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task GetDirectionsTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(RouteFromXToYUtterances.GetToMicrosoft)
                .AssertReply(AssertContains(POISharedResponses.MultipleLocationsFound, new string[] { CardStrings.Overview }))
                .Send(BaseTestUtterances.OptionOne)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        /// <summary>
        /// Get directions to some nearest place.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task GetDirectionsToNearestTest()
        {
            await GetTestFlow()
                .Send(string.Empty)
                .AssertReplyOneOf(this.ParseReplies(POIMainResponses.PointOfInterestWelcomeMessage))
                .Send(BaseTestUtterances.LocationEvent)
                .Send(RouteFromXToYUtterances.GetToNearestPharmacy)
                .AssertReply(CheckForEvent())
                .StartTestAsync();
        }

        private Action<IActivity> AssertStartsWith(string response, IList<string> cardIds)
        {
            return activity =>
            {
                var messageActivity = activity.AsMessageActivity();

                if (response == null)
                {
                    Assert.IsTrue(string.IsNullOrEmpty(messageActivity.Text));
                }
                else
                {
                    Assert.IsTrue(ParseReplies(response, new StringDictionary()).Any((reply) =>
                    {
                        return messageActivity.Text.StartsWith(reply);
                    }));
                }

                AssertSameId(messageActivity, cardIds);
            };
        }

        private Action<IActivity> AssertContains(string response, IList<string> cardIds)
        {
            return activity =>
            {
                var messageActivity = activity.AsMessageActivity();

                if (response == null)
                {
                    Assert.IsTrue(string.IsNullOrEmpty(messageActivity.Text));
                }
                else
                {
                    CollectionAssert.Contains(ParseReplies(response, new StringDictionary()), messageActivity.Text);
                }

                AssertSameId(messageActivity, cardIds);
            };
        }

        private void AssertSameId(IMessageActivity activity, IList<string> cardIds = null)
        {
            if (cardIds == null)
            {
                return;
            }

            for (int i = 0; i < cardIds.Count; ++i)
            {
                var card = activity.Attachments[i].Content as JObject;
                Assert.AreEqual(card["id"], cardIds[i]);
            }
        }

        /// <summary>
        /// Asserts bot response of Event Activity.
        /// </summary>
        /// <returns>Returns an Action with IActivity object.</returns>
        private Action<IActivity> CheckForEvent(PointOfInterestDialogBase.OpenDefaultAppType openDefaultAppType = PointOfInterestDialogBase.OpenDefaultAppType.Map)
        {
            return activity =>
            {
                var eventReceived = activity.AsEventActivity()?.Value as OpenDefaultApp;
                Assert.IsNotNull(eventReceived, "Activity received is not an Event as expected");
                if (openDefaultAppType == PointOfInterestDialogBase.OpenDefaultAppType.Map)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(eventReceived.MapsUri));
                }
                else if (openDefaultAppType == PointOfInterestDialogBase.OpenDefaultAppType.Telephone)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(eventReceived.TelephoneUri));
                }
            };
        }
    }
}