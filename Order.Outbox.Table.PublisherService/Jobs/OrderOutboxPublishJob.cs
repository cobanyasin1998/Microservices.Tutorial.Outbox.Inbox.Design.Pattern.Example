﻿using MassTransit;
using Order.Outbox.Table.PublisherService.Entities;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Order.Outbox.Table.PublisherService.Jobs
{
    public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderOutboxSingletonDatabase.DataReaderState)
            {
                OrderOutboxSingletonDatabase.DataReaderBusy();

                List<OrderOutbox> orderOutboxes = (await OrderOutboxSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM ORDEROUTBOXES WHERE PROCESSEDDATE IS NULL ORDER BY OCCUREDON ASC")).ToList();

                foreach (var orderOutbox in orderOutboxes)
                {
                    if (orderOutbox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);
                        if (orderCreatedEvent != null)
                        {
                            await publishEndpoint.Publish(orderCreatedEvent);
                            OrderOutboxSingletonDatabase.ExecuteAsync($"UPDATE ORDEROUTBOXES SET PROCESSEDDATE = GETDATE() WHERE IdempotentToken = '{orderOutbox.IdempotentToken}'");
                        }
                    }
                }

                OrderOutboxSingletonDatabase.DataReaderReady();
                await Console.Out.WriteLineAsync("Order outbox table checked!");
            }
        }
    }
}
