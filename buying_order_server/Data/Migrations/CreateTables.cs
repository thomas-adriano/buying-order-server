using FluentMigrator;
using buying_order_server.Data.Entity;

namespace buying_order_server.Data.Migrations
{
    [Migration(0)]
    public class CreateTables : Migration
    {

        public override void Up()
        {
            Create.Table("AppConfiguration")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("AppEmailName").AsString()
                .WithColumn("AppEmailUser").AsString()
                .WithColumn("AppEmailPassword").AsString()
                .WithColumn("AppSMTPAddress").AsString()
                .WithColumn("AppSMTPPort").AsInt32()
                .WithColumn("AppEmailFrom").AsString()
                .WithColumn("AppEmailSubject").AsString()
                .WithColumn("AppCronPattern").AsString()
                .WithColumn("AppNotificationTriggerDelta").AsInt32()
                .WithColumn("AppEmailText").AsString().Nullable()
                .WithColumn("AppEmailHtml").AsString().Nullable()
                .WithColumn("AppReplyLink").AsString().Nullable()
                .WithColumn("AppSMTPSecure").AsBoolean().Nullable()
                .WithColumn("AppBlacklist").AsString().Nullable()
                .WithColumn("RowCreatedById").AsString().Nullable()
                .WithColumn("RowModifiedById").AsString().Nullable()
                .WithColumn("RowCreatedDateTimeUtc").AsString().Nullable()
                .WithColumn("RowModifiedDateTimeUtc").AsString().Nullable();

            Create.Table("PostponedOrder")
                 .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                 .WithColumn("OrderId").AsInt32().Unique()
                 .WithColumn("Count").AsInt32()
                 .WithColumn("Date").AsDateTime();

            Insert.IntoTable("AppConfiguration").Row(
                new AppConfigurationEntity
                {
                    AppEmailName = "Inspire Home",
                    AppEmailUser = "viola.von@ethereal.email",
                    AppEmailPassword = "Q61Z2qsRsmg7nUEzNG",
                    AppSMTPAddress = "smtp.ethereal.email",
                    AppSMTPPort = 587,
                    AppSMTPSecure = false,
                    AppEmailFrom = "inspirehome@mail.com",
                    AppEmailSubject = "Olá, ${providerName}!",
                    AppReplyLink = "https://buying-order-reply.web.app/",
                    AppCronPattern = "0/59 * * * * *",
                    AppNotificationTriggerDelta = 5,
                    AppEmailHtml = "Olá ${providerName}, como o pedido numero ${orderNumber}, " +
                                    "data ${orderDate} está prevista para ${previewOrderDate}. " +
                                    "Favor contatar ${orderContactName} ou informar nova data ${replyLinkBegin}aqui${replyLinkEnd}",
                    AppEmailText = "Olá ${providerName}, como o pedido numero ${orderNumber}, " +
                                   "data ${orderDate} está prevista para ${previewOrderDate}. " +
                                   "Favor contatar ${orderContactName} ou informar nova data em ${replyLinkBegin}",
                }
            );
        }

        public override void Down()
        {
            Delete.Table("AppConfiguration");
            Delete.Table("PostponedOrder");
        }

    }
}
