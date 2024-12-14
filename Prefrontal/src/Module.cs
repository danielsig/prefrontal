namespace Prefrontal;

/// <summary>
/// A base class for all modules that can be added to an <see cref="Prefrontal.Agent"/>.
/// <para>
/// 	Each module's constructor can have injectable dependencies in its parameters
/// 	which are injected by the agent using its <see cref="Agent.ServiceProvider"/>.
/// 	The module's constructor can also take the agent itself
/// 	as a parameter and even other modules that it requires.
/// </para>
/// <list type="bullet">
/// 	<item>
///			Modules can be added to an agent
///			using the <see cref="Agent.AddModule{T}"/> method.
///		</item>
/// 	<item>
///			Modules can be removed from an agent
///			using the <see cref="Agent.RemoveModule{T}"/> method.
///		</item>
///		<item>
///			Call <see cref="Agent.Initialize"/> on the agent
///			after adding all modules to it in order to initialize all of its modules.
///		</item>
///		<item>
///			Modules can send signals to other modules
///			using the <see cref="SendSignal{TSignal}(TSignal)"/> method
///			or the <see cref="SendSignalAsync{TSignal}(TSignal)"/> method.
///			These signals can be of any type,
///			but it is recommended to create your own signal types.
///		</item>
///		<item>
///			Signals are processed by modules
///			that implement one of the following:
/// 		<list type="bullet">
/// 			<item><see cref="ISignalReceiver{TSignal}"/></item>
/// 			<item><see cref="ISignalInterceptor{TSignal}"/></item>
/// 			<item><see cref="IAsyncSignalReceiver{TSignal}"/></item>
/// 			<item><see cref="IAsyncSignalInterceptor{TSignal}"/></item>
/// 		</list>
///			These interfaces have a single method that gets called with the signal as a parameter.
///		</item>
///		<item>
///			Modules can implement <see cref="IDisposable"/>
/// 		if they need to clean up resources when they are removed from the agent.
/// 		Modules can also block the agent from removing them
/// 		by throwing an <see cref="InvalidOperationException"/>
/// 		in their <see cref="IDisposable.Dispose"/> method.
/// 		However, doing so when the agent itself is being disposed of has no effect.
/// 	</item>
/// </list>
/// </summary>
public abstract class Module
{
	/// <summary>
	/// The agent that this module belongs to.
	/// <em><b>Beware that this can be null if the module has been removed from the agent.</b></em>
	/// To check if the module is still part of the agent, you can <see cref="implicit operator bool">implicitly cast the module to a boolean</see>.
	/// </summary>
	public Agent Agent { get; internal set; } = null!;

	/// <summary>
	/// The logger that this module can use to log messages.
	/// </summary>
	protected internal ILogger Debug
		=> Agent?.Debug
		?? throw new InvalidOperationException("The module has been removed from the agent.");

	/// <summary>
	/// Initializes the module.
	/// This is where you should set up any connections to other modules.
	/// <br/>
	/// <em>Do not call this method directly. It gets called when you call <see cref="Agent.Initialize"/> on the agent.</em>
	/// </summary>
	protected internal virtual void Initialize()
	{
		
	}
	
	/// <inheritdoc cref="Agent.SendSignalAsync{TSignal}(TSignal)"/>
	protected Task SendSignalAsync<TSignal>(TSignal signal)
		=> Agent?.SendSignalAsync(signal)
		?? Task.FromException(new InvalidOperationException("The module has been removed from the agent."));

	/// <inheritdoc cref="Agent.SendSignal{TSignal}(TSignal)"/>
	protected void SendSignal<TSignal>(TSignal signal)
	{
		if(Agent is null)
			throw new InvalidOperationException("The module has been removed from the agent.");
		Task.Run(() => Agent.SendSignalAsync(signal));
	}

	public override string ToString()
	{
		var name = GetType().Name;
		if(name.EndsWith("Module"))
			name = name[..^6];
		
		return name;
	}

	/// <summary>
	/// True if the module still part of the agent, false otherwise.
	/// </summary>
	public static implicit operator bool(Module module)
		=> module?.Agent is not null;
}
