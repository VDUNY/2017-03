using System;
using System.Threading;

using DemoActor.Interfaces;

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ConsoleApp
{
	class Program
	{
		static void Main( string[] args )
		{
			var cancellationToken = new CancellationToken();
			var actorId = ActorId.CreateRandom();

			Console.WriteLine( "Current Counter value: 0\n" );

			var input = -1;
			while ( input != 0 )
			{
				Console.Write( "Enter a new counter value (0 to exit): " );
				input = Int32.Parse( Console.ReadLine() );

				if ( input != 0 )
				{
					// why create a new instance every time we loop?  :)
					var actor = ActorProxy.Create<IDemoActor>( actorId, "fabric:/Demo3" );
					actor.SetCountAsync( input, cancellationToken ).GetAwaiter().GetResult();
					Console.WriteLine( "Current Counter value: {0}\n", actor.GetCountAsync( cancellationToken ).GetAwaiter().GetResult() );
				}
			}

			Console.ReadKey();
		}
	}
}
