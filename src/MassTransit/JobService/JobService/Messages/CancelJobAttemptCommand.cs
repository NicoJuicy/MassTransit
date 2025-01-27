#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class CancelJobAttemptCommand :
    CancelJobAttempt
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public string? Reason { get; set; }
}
