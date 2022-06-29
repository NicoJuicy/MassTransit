#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Middleware;
    using Middleware.Outbox;


    public class DbContextOutboxConsumeContext<TDbContext, TMessage> :
        OutboxConsumeContextProxy<TMessage>,
        DbTransactionContext
        where TDbContext : DbContext
        where TMessage : class
    {
        readonly TDbContext _dbContext;
        readonly InboxState _inboxState;
        readonly IDbContextTransaction _transaction;

        public DbContextOutboxConsumeContext(ConsumeContext<TMessage> context, OutboxConsumeOptions options, TDbContext dbContext,
            IDbContextTransaction transaction, InboxState inboxState)
            : base(context, options)
        {
            _dbContext = dbContext;
            _transaction = transaction;
            _inboxState = inboxState;
        }

        public override Guid? MessageId => _inboxState.MessageId;

        public override bool ContinueProcessing { get; set; } = true;

        public override bool IsMessageConsumed => _inboxState.Consumed.HasValue;
        public override bool IsOutboxDelivered => _inboxState.Delivered.HasValue;
        public override int ReceiveCount => _inboxState.ReceiveCount;
        public override long? LastSequenceNumber => _inboxState.LastSequenceNumber;

        public Guid TransactionId => _transaction.TransactionId;

        public override async Task SetConsumed()
        {
            _inboxState.Consumed = DateTime.UtcNow;
            _dbContext.Update(_inboxState);

            await _dbContext.SaveChangesAsync(CancellationToken);

            LogContext.Debug?.Log("Outbox Consumed: {MessageId} {Consumed}", MessageId, _inboxState.Consumed);
        }

        public override async Task SetDelivered()
        {
            _inboxState.Delivered = DateTime.UtcNow;
            _dbContext.Update(_inboxState);

            await _dbContext.SaveChangesAsync(CancellationToken);

            LogContext.Debug?.Log("Outbox Delivered: {MessageId} {Delivered}", MessageId, _inboxState.Delivered);
        }

        public override async Task<List<OutboxMessageContext>> LoadOutboxMessages()
        {
            var lastSequenceNumber = LastSequenceNumber ?? 0;

            List<OutboxMessage> messages = await _dbContext.Set<OutboxMessage>()
                .Where(x => x.InboxMessageId == MessageId && x.InboxConsumerId == ConsumerId && x.SequenceNumber > lastSequenceNumber)
                .OrderBy(x => x.SequenceNumber)
                .Take(Options.MessageDeliveryLimit + 1)
                .AsNoTracking()
                .ToListAsync(CancellationToken);

            for (var i = 0; i < messages.Count; i++)
                messages[i].Deserialize(SerializerContext);

            return messages.Cast<OutboxMessageContext>().ToList();
        }

        public override Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
        {
            _inboxState.LastSequenceNumber = message.SequenceNumber;
            _dbContext.Update(_inboxState);

            return _dbContext.SaveChangesAsync(CancellationToken);
        }

        public override async Task RemoveOutboxMessages()
        {
            List<OutboxMessage> messages = await _dbContext.Set<OutboxMessage>()
                .Where(x => x.InboxMessageId == MessageId && x.InboxConsumerId == ConsumerId)
                .ToListAsync(CancellationToken);

            _dbContext.RemoveRange(messages);

            await _dbContext.SaveChangesAsync(CancellationToken);

            if (messages.Count > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {MessageId}", messages.Count, MessageId);
        }

        public override Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            return _dbContext.Set<OutboxMessage>().AddSend(context, SerializerContext, MessageId, ConsumerId);
        }
    }
}