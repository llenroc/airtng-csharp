﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirTNG.Web.Domain.Twilio;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;

namespace AirTNG.Web.Domain.Reservations
{
    public interface INotifier
    {
        Task<MessageResource> SendNotificationAsync(Reservation reservation);
    }

    public class Notifier : INotifier
    {
        
        private readonly IReservationsRepository _repository;

        public Notifier() : this(            
            new ReservationsRepository()) { }

        public Notifier(IReservationsRepository repository, ITwilioRestClient restClient) : this(repository)
        {
            TwilioClient.SetRestClient(restClient);
        }

        public Notifier(IReservationsRepository repository)
        {
            TwilioClient.Init(Credentials.AccountSid, Credentials.AuthToken);
            _repository = repository;
        }

        public async Task<MessageResource> SendNotificationAsync(Reservation reservation)
        {
            var pendingReservations = await _repository.FindPendingReservationsAsync();
            if (pendingReservations.Count() > 1) return null;

            var notification = BuildNotification(reservation);
            return await MessageResource.CreateAsync(notification.To, 
                                                     from: notification.From, 
                                                     body: notification.Messsage);
        }

        private static Notification BuildNotification(Reservation reservation)
        {
            var message = new StringBuilder();
            message.AppendFormat("You have a new reservation request from {0} for {1}:{2}",
                reservation.Name,
                reservation.VacationProperty.Description,
                Environment.NewLine);
            message.AppendFormat("{0}{1}",
                reservation.Message,
                Environment.NewLine);
            message.Append("Reply [accept] or [reject]");

            return new Notification
            {
                From = new PhoneNumber(PhoneNumbers.Twilio),
                To = new PhoneNumber(reservation.PhoneNumber),
                Messsage = message.ToString()
            };
        }
    }
}
