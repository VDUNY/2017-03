using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

using EmailService.Interfaces;

namespace EmailService
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class EmailService : StatefulService, IEmailService
	{
		private readonly string QueueName = "Email";

		public EmailService( StatefulServiceContext context ) : base( context )
		{
		}

		/// <summary>
		/// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
		/// </summary>
		/// <remarks>
		/// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
		/// </remarks>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			return new[]
			{
				// add a basic RPC listener
				new ServiceReplicaListener( context => this.CreateServiceRemotingListener(context))
			};
		}

		/// <summary>
		/// This is the main entry point for your service replica.
		/// This method executes when this replica of your service becomes primary and has write status.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		protected override async Task RunAsync( CancellationToken cancellationToken )
		{
			var queue = await GetQueueAsync();

			while ( true )
			{
				cancellationToken.ThrowIfCancellationRequested();

				using ( var tx = StateManager.CreateTransaction() )
				{
					try
					{
						// get the first item in the queue
						var result = await queue.TryDequeueAsync( tx );
						if ( result.HasValue )
						{
							var email = result.Value;

							// ENHANCEMENT: Send the mail message through a service provider

							ServiceEventSource.Current.ServiceMessage(
									Context, "Message sent with subject {0}", email.Subject );

							// If an exception is thrown before calling CommitAsync, the
							// transaction aborts, all changes are discarded, and nothing
							// is saved to the secondary replicas.
							await tx.CommitAsync();
						}
					}
					catch ( Exception e )
					{
						ServiceEventSource.Current.ServiceMessage( Context, "Error during Dequeue: {0}", e.Message );
						throw;
					}
				}

				await Task.Delay( TimeSpan.FromSeconds( 5 ), cancellationToken );
			}
		}

		#region IEmailService implementation

		public async Task<bool> SendEmail( string fromAddress, string subject, string body, IEnumerable<string> toAddresses )
		{
			var result = false;

			try
			{
				var email = new Email( fromAddress, subject, body, toAddresses );

				// add the message to the queue to be processed out-of-band by RunAsync()
				var queue = await GetQueueAsync();
				using ( var tx = StateManager.CreateTransaction() )
				{
					await queue.EnqueueAsync( tx, email );
					await tx.CommitAsync();
				}

				ServiceEventSource.Current.ServiceMessage( Context, "New mail message queued (Subject = {0})", subject );

				result = true;
			}
			catch ( Exception e )
			{
				ServiceEventSource.Current.ServiceMessage( Context, "Error enqueing mail message. Error: {0}", e.Message );
			}

			return result;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the email queue instaqnce from the state manager.
		/// </summary>
		/// <returns></returns>
		private async Task<IReliableQueue<Email>> GetQueueAsync()
		{
			return await StateManager.GetOrAddAsync<IReliableQueue<Email>>( QueueName );
		}

		#endregion
	}
}
