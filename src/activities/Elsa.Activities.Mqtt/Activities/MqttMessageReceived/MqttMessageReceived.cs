using System.Net.Mqtt;
using System.Threading.Tasks;
using Elsa.Activities.Mqtt.Options;
using Elsa.Activities.Mqtt.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services.Models;

namespace Elsa.Activities.Mqtt.Activities.MqttMessageReceived
{
    [Trigger(
        Category = "MQTT",
        DisplayName = "MQTT Message Received",
        Description = "Triggers when MQTT message matching specified topic is received",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class MqttMessageReceived : MqttBaseActivity
    {
        private readonly IMessageReceiverClientFactory _messageReceiver;

        public MqttMessageReceived(IMessageReceiverClientFactory messageReceiver)
        {
            _messageReceiver = messageReceiver;
        }

        [ActivityOutput(Hint = "Received message")]
        public object? Output { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context) => context.WorkflowExecutionContext.IsFirstPass ? ExecuteInternalAsync(context) : Suspend();

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context) => ExecuteInternalAsync(context);

        private IActivityExecutionResult ExecuteInternalAsync(ActivityExecutionContext context)
        {
            if (context.Input != null)
            {
                var message = (MqttApplicationMessage)context.Input;
                Output = System.Text.Encoding.UTF8.GetString(message.Payload);
            }

            context.LogOutputProperty(this, nameof(Output), Output);

            return Done();
        }
    }
}