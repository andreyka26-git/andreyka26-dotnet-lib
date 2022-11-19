namespace GoogleWithoutCookies.Models
{
    public class CustomRemoteAuthenticationEvents
    {
        /// <summary>
        /// Invoked when an access denied error was returned by the remote server.
        /// </summary>
        public Func<CustomAccessDeniedContext, Task> OnAccessDenied { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when there is a remote failure.
        /// </summary>
        public Func<CustomRemoteFailureContext, Task> OnRemoteFailure { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked after the remote ticket has been received.
        /// </summary>
        public Func<CustomTicketReceivedContext, Task> OnTicketReceived { get; set; } = context => Task.CompletedTask;

        /// <summary>
        /// Invoked when an access denied error was returned by the remote server.
        /// </summary>
        public virtual Task AccessDenied(CustomAccessDeniedContext context) => OnAccessDenied(context);

        /// <summary>
        /// Invoked when there is a remote failure.
        /// </summary>
        public virtual Task RemoteFailure(CustomRemoteFailureContext context) => OnRemoteFailure(context);

        /// <summary>
        /// Invoked after the remote ticket has been received.
        /// </summary>
        public virtual Task TicketReceived(CustomTicketReceivedContext context) => OnTicketReceived(context);
    }
}
