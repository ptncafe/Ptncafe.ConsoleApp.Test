namespace Ptncafe.ConsoleApp.Test.RabbitMq
{
    public static class Constant
    {
        public static readonly string RabbitMqConnectionString = "amqp://unnshbpd:U3dN66ZQ8GY0muSa3HWp9XRR1keAVI4x@reindeer.rmq.cloudamqp.com/unnshbpd";

        public static readonly string TopicExchangeName = "demo.test.topic";
        public static readonly string FanoutExchangeName = "demo.test.fanout";
        public static readonly string DirectExchangeName = "demo.test.direct";

        #region Topic
        public static readonly string Topic_Noti_Order_Publish_RoutingKey = "demo.test.topic.order.noti";
        public static readonly string Topic_Noti_Product_Publish_RoutingKey = "demo.test.topic.product.noti";


        public static readonly string Topic_Noti_Queue_RoutingKey = "demo.test.topic.*.noti";
        #endregion

        #region Fanout
        public static readonly string Fanout_Noti_Queue_RoutingKey = "demo.test.fanout.noti";
        #endregion
        #region Direct
        public static readonly string Fanout_Noti_Queue_RoutingKey = "demo.test.fanout.noti";
        #endregion
    }
}