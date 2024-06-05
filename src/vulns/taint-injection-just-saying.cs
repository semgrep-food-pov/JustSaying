using JustSaying;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.Security.Application;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddJustSaying(config =>
        {
            config.Client(x => { });
            config.Messaging(x => x.WithRegion("us-west-2"));
            config.Subscriptions(x => x.ForQueue<MyMessage>("MyMessageQueue"));
            config.Publications(x => x.WithQueue<MyMessage>());
        });

        services.AddJustSayingHandler<MyMessage, MyMessageHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var bus = serviceProvider.GetRequiredService<IMessagingBus>();

        await bus.StartAsync();

        var publisher = serviceProvider.GetRequiredService<IMessagePublisher>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        string input1 = new UserInputGenerator().GetUserInput();
        var myMessage = new MyMessage(input1);

        //ruleid: taint-injection-just-saying
        await publisher.PublishAsync(myMessage);

        string input2 = new UserInputGenerator().GetUserInput();
        string sanitizedInput = Sanitizer.GetSafeHtmlFragment(input2);

        var mySafeMessage = new MyMessage(sanitizedInput);

        //ok: taint-injection-just-saying
        await publisher.PublishAsync(mySafeMessage);  

        int input3 = new UserInputGenerator().GetUserInput();
        var mySafeMessageInt = new MyMessage(input3);

        //ok: taint-injection-just-saying
        await publisher.PublishAsync(mySafeMessageInt);  

        bool input4 = new UserInputGenerator().GetUserInput();
        var mySafeMessageBool = new MyMessage(input4);

        //ok: taint-injection-just-saying
        await publisher.PublishAsync(mySafeMessageBool);  

        Console.ReadKey();
    }
}
