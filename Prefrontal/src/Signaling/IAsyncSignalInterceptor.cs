namespace Prefrontal.Signaling;

/// <summary>
/// <see cref="Module"/>s need to implement
/// <see cref="IAsyncSignalInterceptor{TSignal}" />
/// or <see cref="ISignalInterceptor{TSignal}" />
/// to intercept signals of that type.
///
/// Modules send signals by
/// calling <see cref="Module.SendSignal{TSignal}(TSignal)"/>
/// or <see cref="Module.SendSignalAsync{TSignal}(TSignal)"/>.
///
/// The order of interception is determined by the order in which modules are added to the agent
/// unless you specify a different order using the <see cref="Agent.SetSignalProcessingOrder{TSignal}(Func{Agent, List{Module}})"/> method.
///
/// Interceptors can modify the signal or stop it from getting processed
/// by subsequent modules via <c>return <see cref="Intercept.StopProcessingSignal">Intercept.StopProcessingSignal</see> </c>.
/// </summary>
/// <typeparam name="TSignal">The type of signals to intercept.</typeparam>
public interface IAsyncSignalInterceptor<TSignal> : IBaseSignalProcessor<TSignal>
{
	/// <summary>
	/// Intercepts a signal sent by another module
	/// via <see cref="Module.SendSignal{TSignal}(TSignal)"/>
	/// or <see cref="Module.SendSignalAsync{TSignal}(TSignal)"/>.
	/// <br/>
	/// Interceptors can modify the signal or stop it from getting processed by subsequent modules.
	/// They can return either the same input <paramref name="signal"/>
	/// or a new <typeparamref name="TSignal"/> instance
	/// which will then get processed by subsequent modules.
	/// <br/>
	/// To stop subsequent modules from processing the signal
	/// simply <c>return <see cref="Intercept.StopProcessingSignal">Intercept.StopProcessingSignal</see> </c>.
	/// <br/>
	/// Examples:
	/// <code>
	/// public class MySignalInterceptorModule : Module, IAsyncSignalInterceptor&lt;MySignal&gt;
	/// {
	/// 	public async Task&lt;Intercept&lt;MySignal&gt;&gt; InterceptSignalAsync(MySignal signal)
	/// 	{
	/// 		if(await signal.GetShouldStopAsync())
	/// 			return Intercept.StopProcessingSignal;
	/// 		return signal;
	/// 	}
	/// }
	///
	/// public class SignalABInterceptorModule : Module, ISignalInterceptor&lt;SignalA&gt;, IAsyncSignalInterceptor&lt;SignalB&gt;
	/// {
	/// 	public Intercept&lt;SignalA&gt; InterceptSignal(SignalA signal)
	/// 	{
	/// 		if(signal.ShouldStop)
	/// 			return Intercept.StopProcessingSignal;
	/// 		return signal;
	/// 	}
	/// 	public async Task&lt;Intercept&lt;SignalB&gt;&gt; InterceptSignalAsync(SignalB signal)
	/// 	{
	/// 		await Task.Delay(1000);
	/// 		if(signal.ShouldStop)
	/// 			return Intercept.StopProcessingSignal;
	/// 		return signal;
	/// 	}
	/// }
	/// </code>
	/// </summary>
	/// <param name="signal">The newly intercepted signal.</param>
	Task<Intercept<TSignal>> InterceptSignalAsync(TSignal signal);
}