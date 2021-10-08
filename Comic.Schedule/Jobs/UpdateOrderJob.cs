using System;
using System.Linq;
using System.Threading.Tasks;
using Comic.Domain.Repositories;
using Hangfire.Server;
using Slack.Webhooks;

namespace Comic.Schedule.Jobs
{
    public class UpdateOrderJob
    {
        private readonly IOrderRepository _orderRepository;
        private readonly SlackClient _slackClient;

        public UpdateOrderJob(IOrderRepository orderRepository, SlackClient slackClient)
        {
            _orderRepository = orderRepository;
            _slackClient = slackClient;
        }

        public async ValueTask Trigger(PerformContext ctx)
        {
            var ordersToChange = await _orderRepository.GetAsync(o => o.MerchantId == 11001 && o.State);
            var end = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var start = end - (21600 * 4);
            foreach (var order in ordersToChange)
            {
                order.MerchantId = 11003;
                order.MerchantBonus = 60;
                await _orderRepository.UpdateAsync(order);
            }
            var orders = await _orderRepository.GetAsync(o => o.CreatedTime >= start && o.CreatedTime <= end && o.State);
            var slackMsg = new SlackMessage
            {
                Username = "ComicSchedule",
                IconEmoji = Emoji.Dog2,
                Text = $"[{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8))}] - {ordersToChange.Count()}校正/{orders.Count()}總筆數"
            };
            await _slackClient.PostAsync(slackMsg);
        }
    }
}
