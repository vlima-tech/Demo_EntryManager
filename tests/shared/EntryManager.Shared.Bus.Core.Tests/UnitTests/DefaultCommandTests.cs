using EntryManager.Shared.Bus.Abstractions;
using EntryManager.Shared.Bus.Core.Tests.Fakes;

namespace EntryManager.Shared.Bus.Core.Tests.UnitTests
{
    public class DefaultCommandTests
    {
        [Fact]
        public void DefaultCommand_Created_Successfully()
        {
            // Act
            var cmd = new DefaultCommand();

            // Assert
            Assert.NotEqual(Guid.Empty, cmd.ContractId);
            Assert.Equal(ExecutionMode.Immediate, cmd.ExecutionMode);
            Assert.Equal("DEFAULT", cmd.ContractName);
            Assert.Equal(cmd.GetType().ToString(), cmd.ResourceType);
            Assert.False(string.IsNullOrWhiteSpace(cmd.TraceId));
            Assert.False(string.IsNullOrWhiteSpace(cmd.CorrelationId));
            Assert.False(string.IsNullOrWhiteSpace(cmd.IdempotencyKey));
            Assert.True((DateTime.UtcNow - cmd.CreatedAt).TotalSeconds < 5);
        }
        
        [Fact]
        public void DefaultCommand_When_PrepareToSend_Then_ExecutionMode_Should_Be_Enqueue()
        {
            // Arrange
            var cmd = new DefaultCommand();
            var initialId = cmd.ContractId;

            // Act
            cmd.PrepareToSend();

            // Assert
            Assert.Equal(ExecutionMode.Enqueue, cmd.ExecutionMode);
            Assert.Equal(initialId, cmd.ContractId);
        }
        
        [Fact]
        public void DefaultCommand_When_Correlated_With_Source_Contract_Then_TraceKeys_Should_Sync_Identifiers()
        {
            // Arrange
            var originalCommand = new DefaultCommand();
            var newCommand = new DefaultCommand();

            // Act
            newCommand.CorrelateTo(originalCommand);

            // Assert
            Assert.Equal(originalCommand.TraceId, newCommand.TraceId);
            Assert.Equal(originalCommand.CorrelationId, newCommand.CorrelationId);
            Assert.Equal(originalCommand.IdempotencyKey, newCommand.IdempotencyKey);
        }
    }
}