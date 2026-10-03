using Icod.LiteRogue.Terminal;
namespace Icod.LiteRogue.Tests;
public sealed class TerminalTests {
    [Fact] public async Task QuitDisposesHost() {
        var host=new TestHost(new ScriptedInput('q'));
        int exit=await Application.RunAsync(["--seed","7"],new StringWriter(),new StringWriter(),_=>ValueTask.FromResult<ITerminalHost>(host));
        Assert.Equal(0,exit); Assert.True(host.Disposed);
    }
    [Fact] public async Task InputFailureDisposesHost() {
        var host=new TestHost(new FailingInput()); var error=new StringWriter();
        int exit=await Application.RunAsync(["--seed","7"],new StringWriter(),error,_=>ValueTask.FromResult<ITerminalHost>(host));
        Assert.Equal(1,exit); Assert.True(host.Disposed); Assert.Contains("Disconnected",error.ToString());
    }
    [Fact] public async Task CancellationDisposesHost() {
        using var cancel=new CancellationTokenSource(); cancel.Cancel();
        var host=new TestHost(new ScriptedInput('q'));
        int exit=await Application.RunAsync(["--seed","7"],new StringWriter(),new StringWriter(),_=>ValueTask.FromResult<ITerminalHost>(host),cancel.Token);
        Assert.Equal(0,exit); Assert.True(host.Disposed);
    }
}
